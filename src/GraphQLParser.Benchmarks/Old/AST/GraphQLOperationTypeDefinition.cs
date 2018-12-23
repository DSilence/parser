namespace GraphQLParser.Benchmarks.Old.AST
{
    public class GraphQLOperationTypeDefinition : ASTNode
    {
        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.OperationTypeDefinition;
            }
        }

        public OperationType Operation { get; set; }
        public GraphQLNamedType Type { get; set; }
    }
}