using Domain.Models;
using Domain.Models.Base;
using eShop.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Infrastructure.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
            
    }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableBaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((AuditableBaseEntity)entityEntry.Entity).Created = DateTime.Now;
                ((AuditableBaseEntity)entityEntry.Entity).CreatedBy = "Admin";
            }
            else
            {
                Entry((AuditableBaseEntity)entityEntry.Entity).Property(p => p.Created).IsModified = false;
                Entry((AuditableBaseEntity)entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                ((AuditableBaseEntity)entityEntry.Entity).LastModified = DateTime.Now;
                ((AuditableBaseEntity)entityEntry.Entity).LastModifiedBy = "Admin";
            }
        }

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
    public override int SaveChanges() =>
          SaveChangesAsync().GetAwaiter().GetResult();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasMany(x => x.Subcategories)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(x => x.Subcategory)
            .WithMany(x => x.Products)
            .HasForeignKey(u => u.SubcategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Basket)
            .WithOne(b => b.User)
            .HasForeignKey<Basket>(b => b.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Basket>()
            .HasMany(b => b.Items)
            .WithOne(i => i.Basket)
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BasketItem>()
            .HasOne(i => i.Product)
            .WithMany(p => p.BasketItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
