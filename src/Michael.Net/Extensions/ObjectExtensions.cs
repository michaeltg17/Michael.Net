﻿using System.Runtime.CompilerServices;

namespace Michael.Net.Extensions
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T? @object, [CallerArgumentExpression(nameof(@object))] string? name = null)
        {
            ArgumentNullException.ThrowIfNull(@object, name);
            return @object;
        }
    }
}
