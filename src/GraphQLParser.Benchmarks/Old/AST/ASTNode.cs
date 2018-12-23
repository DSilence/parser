namespace GraphQLParser.Benchmarks.Old.AST
{
    public abstract class ASTNode
    {
        public abstract ASTNodeKind Kind { get; }
        public GraphQLLocation Location { get; set; }
    }
}