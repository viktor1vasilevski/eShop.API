﻿using Domain.Models;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Context
{
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
                    ((AuditableBaseEntity)entityEntry.Entity).Created = DateTime.UtcNow;
                    ((AuditableBaseEntity)entityEntry.Entity).CreatedBy = "Admin";
                }
                else
                {
                    Entry((AuditableBaseEntity)entityEntry.Entity).Property(p => p.Created).IsModified = false;
                    Entry((AuditableBaseEntity)entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;
                }

                if (entityEntry.State == EntityState.Modified)
                {
                    ((AuditableBaseEntity)entityEntry.Entity).LastModified = DateTime.UtcNow;
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

            //modelBuilder.Entity<Order>()
            //    .HasOne(x => x.User)
            //    .WithMany(x => x.Orders)
            //    .HasForeignKey(x => x.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Order>()
            //    .HasOne(x => x.Product)
            //    .WithMany(x => x.Orders)
            //    .HasForeignKey(x => x.ProductId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<UserBasket>()
            //    .HasOne(x => x.User)
            //    .WithMany(x => x.UserBaskets)
            //    .HasForeignKey(x => x.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<UserBasket>()
            //    .HasOne(x => x.Product)
            //    .WithMany(x => x.UserBaskets)
            //    .HasForeignKey(x => x.ProductId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.User)
            //    .WithMany(u => u.Comments)
            //    .HasForeignKey(c => c.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Comment>()
            //    .HasOne(c => c.Product)
            //    .WithMany(p => p.Comments)
            //    .HasForeignKey(c => c.ProductId)
            //    .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
