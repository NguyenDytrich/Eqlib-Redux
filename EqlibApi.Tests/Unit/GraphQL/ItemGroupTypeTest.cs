using System;
using System.Threading.Tasks;
using EqlibApi.GraphQL;
using EqlibApi.Models;
using HotChocolate;
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
        public void ItemGroup_Snapshot_Matches()
        {
            ISchema schema = Schema.Create(c =>
            {
                c.RegisterQueryType<ItemGroupType>();
            });

            var schemaDSL = schema.ToString();

            schemaDSL.ShouldMatchSnapshot();
        }
    }
}
