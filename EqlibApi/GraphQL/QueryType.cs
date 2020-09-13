using System;
using HotChocolate.Types;

namespace EqlibApi.GraphQL
{
    public class QueryType : ObjectType<GraphQLHello>
    {
        public QueryType()
        {
        }
    }

    public class GraphQLHello
    {
        public string Hello() => "Hello world";
    }
}
