using System;
using EqlibApi.Models;
using HotChocolate;
using HotChocolate.Types;

namespace EqlibApi.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class ItemGroupQueries
    {
        private readonly ApplicationContext _context;
        public ItemGroupQueries([Service]ApplicationContext context)
        {
            _context = context;
        }

        public string ItemGroupPlaceholder() => "Placeholder";
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
