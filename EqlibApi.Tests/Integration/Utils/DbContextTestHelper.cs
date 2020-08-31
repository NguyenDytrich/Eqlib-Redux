using EqlibApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EqlibApi.Tests.Integration.Utils
{
    public static class DbContextTestHelper
    {
        public static void ClearEntities(ApplicationContext context)
        {
            using var transaction = context.Database.BeginTransaction();
            try
            {
                context.Database.ExecuteSqlRaw("delete from \"Items\";");
                context.Database.ExecuteSqlRaw("delete from \"ItemGroups\";");
                context.Database.ExecuteSqlRaw("delete from \"Checkouts\";");
                context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
