using EqlibApi.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace EqlibApi.Models
{
    public interface IApplicationContext
    {
        DbSet<ItemGroup> ItemGroups { get; set; }
        DbSet<Item> Items { get; set; }
        DbSet<Checkout> Checkouts { get; set; }
    }
    public class ApplicationContext : DbContext, IApplicationContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public virtual DbSet<ItemGroup> ItemGroups { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Checkout> Checkouts { get; set; }
    }
}
