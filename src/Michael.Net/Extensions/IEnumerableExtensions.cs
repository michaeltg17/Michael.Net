﻿namespace Michael.Net.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach(var item in items)
            {
                action(item);
            }
        }
    }
}
