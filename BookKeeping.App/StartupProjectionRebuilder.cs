using BookKeeping.Domain.Contracts;
using BookKeeping.Infrastructure;
using BookKeeping.Persistent.AtomicStorage;
using BookKeeping.Persistent.Storage;
using Mono.Cecil;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace BookKeeping.App
{
    public static class StartupProjectionRebuilder
    {
        public static void Rebuild(CancellationToken token, IDocumentStore targetContainer, IEventStore stream, Func<IDocumentStore, IEnumerable<object>> projectors, Func<bool> needRebuildAction = null)
        {
            var strategy = targetContainer.Strategy;

            var memoryDocumentStore = new MemoryDocumentStore(new ConcurrentDictionary<string, ConcurrentDictionary<string, byte[]>>(), targetContainer.Strategy);
            var tracked = new ProjectionInspectingStore(memoryDocumentStore);

            var projections = new List<object>();
            projections.AddRange(projectors(tracked));

            if (tracked.Projections.Count != projections.Count())
                throw new InvalidOperationException("Count mismatch");
            tracked.ValidateSanity();

            var persistedHashes = new Dictionary<string, string>();
            var name = "domain";
            targetContainer.GetReader<object, ProjectionHash>().Get(name).IfValue(v => persistedHashes = v.BucketHashes);

            var activeMemoryProjections = projections.Select((projection, i) =>
            {
                var proj = tracked.Projections[i];
                var bucketName = proj.StoreBucket;
                var viewType = proj.EntityType;

                var projectionHash =
                    "Global change on 2012-08-24\r\n" +
                    GetClassHash(projection.GetType()) +
                    "\r\n " + GetClassHash(viewType) + "\r\n" + GetClassHash(strategy.GetType());

                bool needsRebuild = !persistedHashes.ContainsKey(bucketName) || persistedHashes[bucketName] != projectionHash;
                return new
                {
                    bucketName,
                    projection,
                    hash = projectionHash,
                    needsRebuild
                };
            }).ToArray();

            foreach (var memoryProjection in activeMemoryProjections)
            {
                if (memoryProjection.needsRebuild)
                {
                    SystemObserver.Notify("[warn] {0} needs rebuild", memoryProjection.bucketName);
                }
                else
                {
                    SystemObserver.Notify("[good] {0} is up-to-date", memoryProjection.bucketName);
                }
            }


            var needRebuild = activeMemoryProjections.Where(x => x.needsRebuild).ToArray();

            if (needRebuild.Length == 0)
            {
                return;
            }

            if (needRebuildAction != null)
            {
                if (!needRebuildAction())
                    return;
            }


            var watch = Stopwatch.StartNew();

            var wire = new RedirectToDynamicEvent();
            Array.ForEach(needRebuild, x => wire.WireToWhen(x.projection));


            var handlersWatch = Stopwatch.StartNew();



            ObserveWhileCan(stream.LoadEventStream(0, int.MaxValue), wire, token);

            if (token.IsCancellationRequested)
            {
                SystemObserver.Notify("[warn] Aborting projections before anything was changed");
                return;
            }

            var timeTotal = watch.Elapsed.TotalSeconds;
            var handlerTicks = handlersWatch.ElapsedTicks;
            var timeInHandlers = Math.Round(TimeSpan.FromTicks(handlerTicks).TotalSeconds, 1);
            SystemObserver.Notify("Total Elapsed: {0}sec ({1}sec in handlers)", Math.Round(timeTotal, 0), timeInHandlers);


            // update projections that need rebuild
            foreach (var b in needRebuild)
            {
                // server might shut down the process soon anyway, but we'll be
                // in partially consistent mode (not all projections updated)
                // so at least we blow up between projection buckets
                token.ThrowIfCancellationRequested();

                var bucketName = b.bucketName;
                var bucketHash = b.hash;

                // wipe contents
                targetContainer.Reset(bucketName);
                // write new versions
                var contents = memoryDocumentStore.EnumerateContents(bucketName);
                targetContainer.WriteContents(bucketName, contents);

                // update hash
                targetContainer.GetWriter<object, ProjectionHash>().UpdateEnforcingNew(name, x =>
                {
                    x.BucketHashes[bucketName] = bucketHash;
                });

                SystemObserver.Notify("[good] Updated View bucket {0}.{1}", name, bucketName);
            }

            // Clean up obsolete views
            var allBuckets = new HashSet<string>(activeMemoryProjections.Select(p => p.bucketName));
            var obsoleteBuckets = persistedHashes.Where(s => !allBuckets.Contains(s.Key)).ToArray();
            foreach (var hash in obsoleteBuckets)
            {
                // quit at this stage without any bad side effects
                if (token.IsCancellationRequested)
                    return;

                var bucketName = hash.Key;
                SystemObserver.Notify("[warn] {0} is obsolete", bucketName);
                targetContainer.Reset(bucketName);

                targetContainer.GetWriter<object, ProjectionHash>().UpdateEnforcingNew(name, x => x.BucketHashes.Remove(bucketName));

                SystemObserver.Notify("[good] Cleaned up obsolete view bucket {0}.{1}", name, bucketName);
            }
        }

        [DataContract]
        public sealed class ProjectionHash
        {
            [DataMember(Order = 1)]
            public Dictionary<string, string> BucketHashes { get; set; }

            public ProjectionHash()
            {
                BucketHashes = new Dictionary<string, string>();
            }
        }


        sealed class ProjectionInspectingStore : IDocumentStore
        {
            readonly IDocumentStore _real;

            public ProjectionInspectingStore(IDocumentStore real)
            {
                _real = real;
            }

            public readonly List<Projection> Projections = new List<Projection>();


            public sealed class Projection
            {
                public Type EntityType;
                public string StoreBucket;
            }

            public void ValidateSanity()
            {
                if (Projections.Count == 0)
                    throw new InvalidOperationException("There were no projections registered");

                var viewsWithMultipleProjections = Projections.GroupBy(e => e.EntityType).Where(g => g.Count() > 1).ToList();
                if (viewsWithMultipleProjections.Count > 0)
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("Please, define only one projection per view. These views were referenced more than once:");
                    foreach (var projection in viewsWithMultipleProjections)
                    {
                        builder.AppendLine("  " + projection.Key);
                    }
                    builder.AppendLine("NB: you can use partials or dynamics in edge cases");
                    throw new InvalidOperationException(builder.ToString());
                }

                var viewsWithSimilarBuckets = Projections
                    .GroupBy(e => e.StoreBucket.ToLowerInvariant())
                    .Where(g => g.Count() > 1)
                    .ToArray();

                if (viewsWithSimilarBuckets.Length > 0)
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("Following views will be stored in same location, which will cause problems:");
                    foreach (var i in viewsWithSimilarBuckets)
                    {
                        var @join = string.Join(",", i.Select(x => x.EntityType));
                        builder.AppendFormat(" {0} : {1}", i.Key, @join).AppendLine();
                    }
                    throw new InvalidOperationException(builder.ToString());
                }

            }

            public IDocumentWriter<TKey, TEntity> GetWriter<TKey, TEntity>()
            {
                Projections.Add(new Projection()
                {
                    EntityType = typeof(TEntity),
                    StoreBucket = _real.Strategy.GetEntityBucket<TEntity>()
                });

                return _real.GetWriter<TKey, TEntity>();
            }

            public IDocumentReader<TKey, TEntity> GetReader<TKey, TEntity>()
            {
                return _real.GetReader<TKey, TEntity>();
            }

            public IDocumentStrategy Strategy
            {
                get { return _real.Strategy; }
            }

            public IEnumerable<DocumentRecord> EnumerateContents(string bucket)
            {
                return _real.EnumerateContents(bucket);
            }

            public void WriteContents(string bucket, IEnumerable<DocumentRecord> records)
            {
                _real.WriteContents(bucket, records);
            }

            public void Reset(string bucket)
            {
                _real.Reset(bucket);
            }
        }



        static string GetClassHash(Type type1)
        {
            var location = type1.Assembly.Location;
            var mod = ModuleDefinition.ReadModule(location);
            var builder = new StringBuilder();
            var type = type1;


            var typeDefinition = mod.GetType(type.FullName);
            builder.AppendLine(typeDefinition.Name);
            ProcessMembers(builder, typeDefinition);

            // we include nested types
            foreach (var nested in typeDefinition.NestedTypes)
            {
                ProcessMembers(builder, nested);
            }

            return builder.ToString();
        }

        static void ProcessMembers(StringBuilder builder, TypeDefinition typeDefinition)
        {
            foreach (var md in typeDefinition.Methods.OrderBy(m => m.ToString()))
            {
                builder.AppendLine("  " + md);

                foreach (var instruction in md.Body.Instructions)
                {
                    // we don't care about offsets
                    instruction.Offset = 0;
                    builder.AppendLine("    " + instruction);
                }
            }
            foreach (var field in typeDefinition.Fields.OrderBy(f => f.ToString()))
            {
                builder.AppendLine("  " + field);
            }
        }


        static void ObserveWhileCan(EventStream records, RedirectToDynamicEvent wire, CancellationToken token)
        {
            var watch = Stopwatch.StartNew();
            int count = 0;
            foreach (var record in records.Events)
            {
                count += 1;

                if (token.IsCancellationRequested)
                    return;
                if (count % 50000 == 0)
                {
                    SystemObserver.Notify("Observing {0} {1}", count,
                        Math.Round(watch.Elapsed.TotalSeconds, 2));
                    watch.Restart();
                }
                if (record is IEvent)
                {
                    wire.InvokeEvent(record);
                }
            }
        }
    }

    /// <summary>
    /// Creates convention-based routing rules
    /// </summary>
    public sealed class RedirectToDynamicEvent
    {
        public readonly IDictionary<Type, List<Wire>> Dict = new Dictionary<Type, List<Wire>>();


        public sealed class Wire
        {
            public Action<object> Call;
            public Type ParameterType;
        }

        static readonly MethodInfo InternalPreserveStackTraceMethod =
            typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);



        public void WireToWhen(object o)
        {
            var infos = o.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().Length == 1);

            foreach (var methodInfo in infos)
            {
                if (null == methodInfo)
                    throw new InvalidOperationException();

                var wires = new HashSet<Type>();
                var parameterType = methodInfo.GetParameters().First().ParameterType;
                wires.Add(parameterType);


                // if this is an interface, then we wire up to all inheritors in loaded assemblies
                // TODO: make this explicit
                if (parameterType.IsInterface)
                {
                    throw new InvalidOperationException("We don't support wiring to interfaces");
                    //var inheritors = typeof(StartProjectRun).Assembly.GetExportedTypes().Where(parameterType.IsAssignableFrom);
                    //foreach (var inheritor in inheritors)
                    //{
                    //    wires.Add(inheritor);
                    //}
                }

                foreach (var type in wires)
                {

                    List<Wire> list;
                    if (!Dict.TryGetValue(type, out list))
                    {
                        list = new List<Wire>();
                        Dict.Add(type, list);
                    }
                    var wire = BuildWire(o, type, methodInfo);
                    list.Add(wire);
                }
            }


        }

        static Wire BuildWire(object o, Type type, MethodInfo methodInfo)
        {
            var info = methodInfo;
            var dm = new DynamicMethod("MethodWrapper", null, new[] { typeof(object), typeof(object) });
            var il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, o.GetType());
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Castclass, type);
            il.EmitCall(OpCodes.Call, info, null);
            il.Emit(OpCodes.Ret);

            var call = (Action<object, object>)dm.CreateDelegate(typeof(Action<object, object>));
            var wire = new Wire
            {
                Call = o1 => call(o, o1),
                ParameterType = type
            };
            return wire;
        }

        public void WireTo<TMessage>(Action<TMessage> msg)
        {
            var type = typeof(TMessage);

            List<Wire> list;
            if (!Dict.TryGetValue(type, out list))
            {
                list = new List<Wire>();
                Dict.Add(type, list);
            }
            list.Add(new Wire
            {
                Call = o => msg((TMessage)o)
            });
        }



        [DebuggerNonUserCode]
        public void InvokeEvent(object @event)
        {
            var type = @event.GetType();
            List<Wire> info;
            if (!Dict.TryGetValue(type, out info))
            {
                return;
            }
            try
            {
                foreach (var wire in info)
                {
                    wire.Call(@event);
                }
            }
            catch (TargetInvocationException ex)
            {
                if (null != InternalPreserveStackTraceMethod)
                    InternalPreserveStackTraceMethod.Invoke(ex.InnerException, new object[0]);
                throw ex.InnerException;
            }
        }
    }


}