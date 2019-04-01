using System;

namespace GraphQLParser.AST
{
    public class GraphQLScalarValue : GraphQLValue
    {
        private ASTNodeKind kindField;

        public GraphQLScalarValue(ASTNodeKind kind)
        {
            this.kindField = kind;
        }

        public override ASTNodeKind Kind
        {
            get
            {
                return this.kindField;
            }
        }

        public ReadOnlyMemory<char> Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}