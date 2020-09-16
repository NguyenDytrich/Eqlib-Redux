using System;
using System.Threading.Tasks;
using EqlibApi.GraphQL;
using FluentAssertions;
using HotChocolate;
using NUnit.Framework;
using Snapper;
using Snapper.Attributes;

namespace EqlibApi.Tests.Unit.GraphQL
{
    [Category("Unit")]
    public class QueryTest
    {
        [Test]
        public async Task ExpectedQueries()
        {
            ISchema schema = SchemaBuilder
                .New()
                .AddQueryType(d => d.Name("Query"))
                .AddType<ItemGroupQueries>()
                .Create();

            var schemaDSL = schema.ToString();

            schemaDSL.Should().Contain("itemGroups: [ItemGroup]");
        }

        [Test]
        [Category("Snapshot")]
        [UpdateSnapshots]
        public async Task Queries_MatchSnapshot()
        {
            ISchema schema = SchemaBuilder
                .New()
                .AddQueryType(d => d.Name("Query"))
                .AddType<ItemGroupQueries>()
                .Create();
            var schemaDSL = schema.ToString();

            schemaDSL.ShouldMatchSnapshot();
        } 
    }
}
