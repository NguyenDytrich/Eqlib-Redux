using System;
using System.Collections.Generic;
using System.Linq;
using EqlibApi.Models;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace EqlibApi.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class ItemGroupQueries
    {
        public IEnumerable<ItemGroup> GetItemGroups([Service]IApplicationContext _context)
        {
            return _context.ItemGroups;
        }
    }

    public class ItemGroupType : ObjectType<ItemGroup>
    {
        protected override void Configure(IObjectTypeDescriptor<ItemGroup> descriptor)
        {
            // Temporarily ignore this field
            descriptor.Field(c => c.Inventory)
                .Ignore();
            descriptor.Field<ItemResolver>(t => t.GetInventory(default, default));
        }
    }

    public class ItemResolver
    {
        private readonly IApplicationContext _context;

        public ItemResolver([Service]IApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<Item> GetInventory(ItemGroup group, IResolverContext ctx)
        {
            return _context.Items.Where(i => i.ItemGroupId == group.Id);
        }
    }
}
