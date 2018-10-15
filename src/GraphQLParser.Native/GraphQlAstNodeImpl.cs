using System;
using System.Runtime.InteropServices;

namespace GraphQLParser.Native
{
    public static class GraphQlAstNodeImpl
    {
        [DllImport("graphqlparser", EntryPoint = "graphql_node_get_location")]
        public static extern void GetLocation(IntPtr node, out GraphQLAstLocation location);

        [DllImport("graphqlparser", EntryPoint = "graphql_node_free")]
        internal static extern void Free(IntPtr node);
    }
}