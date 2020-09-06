using EqlibApi.Models.Db;
using EqlibApi.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Checkout>()
                .Property(c => c.CheckoutDate)
                .HasDefaultValueSql("NOW()")
                .ValueGeneratedOnAdd();
            mb.Entity<Checkout>()
                .Property(c => c.CheckoutStatus)
                .HasDefaultValue(ECheckoutStatus.Outstanding);
            mb.Entity<Checkout>()
                .Property(c => c.ApprovalStatus)
                .HasDefaultValue(ECheckoutApproval.Pending);
        }
    }
}
