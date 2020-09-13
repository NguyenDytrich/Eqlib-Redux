using System;
using HotChocolate.Types;

namespace EqlibApi.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class HelloType
    {
        public HelloType()
        {
        }

        public string Hello() => "Hello world.";
    }
}
