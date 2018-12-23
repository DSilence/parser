﻿using Xunit;

namespace GraphQLParser.Tests
{
    using GraphQLParser;

    public class SourceTests
    {
        [Fact]
        public void CreateSourceFromString_BodyEqualsToProvidedSource()
        {
            var source = new Source("somesrc");

            Assert.Equal("somesrc", source.Body.Span.ToString());
        }

        [Fact]
        public void CreateSourceFromString_SourceNameEqualsToGraphQL()
        {
            var source = new Source("somesrc");

            Assert.Equal("GraphQL", source.Name);
        }

        [Fact]
        public void CreateSourceFromStringWithName_SourceNameEqualsToProvidedName()
        {
            var source = new Source("somesrc", "somename");

            Assert.Equal("somename", source.Name);
        }
    }
}