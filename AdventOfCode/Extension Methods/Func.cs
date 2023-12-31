﻿namespace AdventOfCode
{
    public static partial class FuncExtensions
    {
        public static Func<T, R> Memoize<T, R>(this Func<T, R> func)
            where T : notnull
        {
            var d = new Dictionary<T, R>();

            return t => d.TryGetValue(t, out var r) ? r : d[t] = func(t);
        }

        public static Func<T, R> Memoize<T, R>(this Func<T, R> func, IEqualityComparer<T> comparer)
            where T : notnull
        {
            var d = new Dictionary<T, R>(comparer);

            return t => d.TryGetValue(t, out var r) ? r : d[t] = func(t);
        }

        public static Func<T, R> Memoize<T, R, K>(this Func<T, R> func, Func<T, K> keySelector)
            where K : notnull 
        {
            var d = new Dictionary<K, R>();

            return t => {
                
                var k = keySelector(t);

                if (!d.TryGetValue(k, out var r)) r = d[k] = func(t);

                return r;
            };
        }
    }
}
