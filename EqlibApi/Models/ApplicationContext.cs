using EqlibApi.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EqlibApi.Models
{
    public interface IApplicationContext : IDisposable
    {
        DbSet<ItemGroup> ItemGroups { get; set; }
        DbSet<Item> Items { get; set; }
        DbSet<Checkout> Checkouts { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cToken);
    }
    public class ApplicationContext : DbContext, IApplicationContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public virtual DbSet<ItemGroup> ItemGroups { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Checkout> Checkouts { get; set; }
    }
}
