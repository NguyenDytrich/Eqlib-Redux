using System;
using System.Threading.Tasks;
using EqlibApi.GraphQL;
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
        public async Task ItemGroup_ResolvesFromService()
        {
            ISchema schema = Schema.Create(c =>
            {
                c.RegisterQueryType<ItemGroupType>();
            });

            Mock<IResolverContext> mockResolverContext = new Mock<IResolverContext>();
        }
    }
}
