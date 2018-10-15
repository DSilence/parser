using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GraphQLParser.Native
{
    public static class GraphQlParser
    {
        [DllImport("graphqlparser", EntryPoint = "graphql_parse_string", SetLastError = true)]
        private static extern IntPtr GraphqlParseString(string text, out string error);

        public static GraphQlDocument ParseString(string text)
        {
            var result = GraphqlParseString(text, out var error);
            return new GraphQlDocument(result, error);
        }

        [DllImport("graphqlparser", EntryPoint = "graphql_parse_string_with_experimental_schema_support", SetLastError = true)]
        public static extern IntPtr ParseStringWithExperimentalSchemaSupport(string text, string[] error);
    }
}