using System;
using System.Text.RegularExpressions;

namespace GraphQLParser
{
    public class Source : ISource
    {
        public Source(string body) : this(body, "GraphQL")
        {
        }

        public Source(string body, string name)
        {
            this.Name = name;
            this.Body = MonetizeLineBreaks(body).AsMemory();
        }

        public ReadOnlyMemory<char> Body { get; set; }
        public string Name { get; set; }

        private static string MonetizeLineBreaks(string input)
        {
            return (input ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }
    }
}