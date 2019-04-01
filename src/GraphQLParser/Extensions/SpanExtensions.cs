using System;
using System.Runtime.CompilerServices;

namespace GraphQLParser.Extensions
{
    internal static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SequenceEqual(this ReadOnlyMemory<char> memory, string other)
        {
            return memory.Span.SequenceEqual(other.AsSpan());
        }
    }
}