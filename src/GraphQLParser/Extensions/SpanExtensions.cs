using System;
using System.Runtime.CompilerServices;

namespace GraphQLParser.Extensions
{
    internal static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual(this ReadOnlySpan<char> span, string other)
        {
            return span.SequenceEqual(other.AsSpan());
        }
    }
}