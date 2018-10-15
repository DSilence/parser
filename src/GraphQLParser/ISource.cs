using System;

namespace GraphQLParser
{
    public interface ISource
    {
        ReadOnlyMemory<char> Body { get; set; }
        string Name { get; set; }
    }
}