using System;
using System.Collections.Generic;
using EqlibApi.Models;
using HotChocolate;
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
            descriptor.Field(c => c.Inventory)
                .Ignore();
        }
    }
}
