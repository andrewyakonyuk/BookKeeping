using System;
using System.Collections.Generic;

namespace BookKeeping.Core
{
    public static class SequenceExtensions
    {
        public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> items, Func<T, TResult> func)
        {
            foreach (var item in items)
                yield return func(item);
        }

        public static IEnumerable<TResult> Select<TArg1, TArg2, TResult>(this IEnumerable<TArg1> arg1, IEnumerable<TArg2> arg2, Func<TArg1, TArg2, TResult> func)
        {
            var e1 = arg1.GetEnumerator();
            var e2 = arg2.GetEnumerator();
            var s = new SequenceMapEnumerator<TArg1, TArg2, TResult>(e1, e2, func);

            while (s.MoveNext())
                yield return s.Current;
        }

        public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> items, Func<int, T, TResult> func)
        {
            var index = 0;
            foreach (var item in items)
            {
                yield return func(index, item);
                index++;
            }
        }

        public static IEnumerable<Tuple<TArg1, TArg2>> Zip<TArg1, TArg2>(this IEnumerable<TArg1> arg1, IEnumerable<TArg2> arg2, Func<TArg1, TArg2, Tuple<TArg1, TArg2>> func)
        {
            return arg1.Select(arg2, func);
        }
    }

    public class SequenceMapEnumerator<T, U, V> : IEnumerator<V>
    {
        private readonly IEnumerator<T> e1;
        private readonly IEnumerator<U> e2;
        private readonly Func<T, U, V> f;

        public SequenceMapEnumerator(IEnumerator<T> e1, IEnumerator<U> e2, Func<T, U, V> f)
        {
            this.e1 = e1;
            this.e2 = e2;
            this.f = f;
        }

        public V Current
        {
            get { return f(e1.Current, e2.Current); }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return f(e1.Current, e2.Current); }
        }

        public void Dispose()
        {
            e1.Dispose();
            e2.Dispose();
        }

        public bool MoveNext()
        {
            var n1 = e1.MoveNext();
            var n2 = e2.MoveNext();
            if (n1 != n2)
                throw new InvalidOperationException("Inequal list lengths");
            return n1;
        }

        public void Reset()
        {
            e1.Reset();
            e2.Reset();
        }
    }
}