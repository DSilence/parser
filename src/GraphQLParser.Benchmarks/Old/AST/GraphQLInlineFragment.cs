﻿using System.Collections.Generic;

namespace GraphQLParser.Benchmarks.Old.AST
{
    public class GraphQLInlineFragment : ASTNode
    {
        public IEnumerable<GraphQLDirective> Directives { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.InlineFragment;
            }
        }

        public GraphQLSelectionSet SelectionSet { get; set; }
        public GraphQLNamedType TypeCondition { get; set; }
    }
}