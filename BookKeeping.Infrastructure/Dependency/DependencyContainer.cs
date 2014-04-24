using Autofac;
using BookKeeping.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BookKeeping.Infrastructure.Dependency
{
    public class DependencyContainer
    {
        public static IContainer Instance { get; private set; }

        public static IContainer Configure(params Assembly[] assemblies)
        {
            ContainerBuilder builder = new ContainerBuilder();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypesWithAttribute<SuppressDependencyAttribute>())
                    {
                        string fullyQualifiedName = type.GetSingleAttribute<SuppressDependencyAttribute>().FullyQualifiedName;
                        string assembly2 = type.GetSingleAttribute<SuppressDependencyAttribute>().Assembly;
                        try
                        {
                            builder.RegisterType(type).As(new Type[]
                            {
                                Assembly.LoadFrom(Path.Combine(path, assembly2 + ".dll")).GetType(fullyQualifiedName)
                            }).InstancePerLifetimeScope();
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(string.Concat("The registration of ", type, " went wrong trying to suppress the Tea Commerce dependency ", fullyQualifiedName), ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException)
                    {
                        if (ex.Message.StartsWith("The registration of"))
                            throw;
                    }
                }
            }
            builder.RegisterAssemblyModules(assemblies);
            return DependencyContainer.Instance = builder.Build();
        }
    }
}
