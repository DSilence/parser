using GraphQLParser.Extensions;

namespace GraphQLParser
{
    using System.Text.RegularExpressions;

    public class Location
    {
        private static readonly Regex LineRegex = new Regex("\r\n|[\n\r]", RegexOptions.Compiled | RegexOptions.ECMAScript);

        public Location(ISource source, int position)
        {
            this.Line = 1;
            this.Column = position + 1;

            var matches = LineRegex.Matches(source.Body.Span.ToString());
            foreach (Match match in matches)
            {
                if (match.Index >= position)
                    break;

                this.Line++;
                this.Column = position + 1 - (match.Index + matches[0].Length);
            }
        }

        public int Column { get; private set; }
        public int Line { get; private set; }
    }
}