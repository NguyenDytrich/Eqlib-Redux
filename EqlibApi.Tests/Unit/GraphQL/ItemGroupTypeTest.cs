using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EqlibApi.GraphQL;
using EqlibApi.Models;
using EqlibApi.Tests.Unit.Utils;
using FluentAssertions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Snapper;
using Snapper.Attributes;

namespace EqlibApi.Tests.Unit.GraphQL
{
    [Category("Unit")]
    public class ItemGroupTypeTest
    {
        [Test]
        /// <summary>
        /// Snapshot test to notify us if the schema changes
        /// </summary>
        public async Task ItemGroup_Snapshot_Matches()
        {
            ISchema schema = Schema.Create(c =>
            {
                c.RegisterQueryType<ItemGroupType>();
            });

            var schemaDSL = schema.ToString();

            schemaDSL.ShouldMatchSnapshot();
        }

        [Test]
        public async Task ItemGroup_Query_GetsAllFromContext()
        {
            var fixture = new Fixture();
            var mockContext = new Mock<IApplicationContext>();
            var itemGroupsList = fixture.CreateMany<ItemGroup>().ToList();
            mockContext.Setup(c => c.ItemGroups)
                .Returns(DbSetProvider.CreateSet(itemGroupsList));

            var queries = new ItemGroupQueries();
            var result = queries.GetItemGroups(mockContext.Object);

            result.Should().BeEquivalentTo(itemGroupsList);
        }
    }
}
