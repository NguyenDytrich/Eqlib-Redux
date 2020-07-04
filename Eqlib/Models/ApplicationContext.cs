using EqlibApi.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace EqlibApi.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public virtual DbSet<ItemGroup> ItemGroups { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Checkout> Checkouts { get; set; }
    }
}
