﻿using System.Collections.Generic;

namespace GraphQLParser.Benchmarks.Old.AST
{
    public class GraphQLEnumTypeDefinition : GraphQLTypeDefinition
    {
        public IEnumerable<GraphQLDirective> Directives { get; set; }

        public override ASTNodeKind Kind
        {
            get
            {
                return ASTNodeKind.EnumTypeDefinition;
            }
        }

        public GraphQLName Name { get; set; }
        public IEnumerable<GraphQLEnumValueDefinition> Values { get; set; }
    }
}