using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRSWebApi.Models;

public partial class PrsDbContext : DbContext
{
    public PrsDbContext()
    {
    }

    public PrsDbContext(DbContextOptions<PrsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LineItem> LineItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }


    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<LineItem>(entity =>
    //    {
    //        entity.HasKey(e => e.Id).HasName("PK__LineItem__3214EC27FC1306F5");

    //        entity.HasOne(d => d.Product).WithMany(p => p.LineItems)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__LineItem__Produc__3B75D760");

    //        entity.HasOne(d => d.Request).WithMany(p => p.LineItems)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__LineItem__Reques__3A81B327");
    //    });

    //    modelBuilder.Entity<Product>(entity =>
    //    {
    //        entity.HasKey(e => e.Id).HasName("PK__Product__3214EC27050DF5A2");

    //        entity.HasOne(d => d.Vendor).WithMany(p => p.Products)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Product__VendorI__31EC6D26");
    //    });

    //    modelBuilder.Entity<Request>(entity =>
    //    {
    //        entity.HasKey(e => e.Id).HasName("PK__Request__3214EC274088B678");

    //        entity.Property(e => e.Status).HasDefaultValue("NEW");

    //        entity.HasOne(d => d.User).WithMany(p => p.Requests)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("requesttouser");
    //    });

    //    modelBuilder.Entity<User>(entity =>
    //    {
    //        entity.HasKey(e => e.Id).HasName("PK__User__3214EC27CD979925");

    //        entity.Property(e => e.Admin).HasDefaultValue(false);
    //        entity.Property(e => e.Reviewer).HasDefaultValue(false);
    //    });

    //    modelBuilder.Entity<Vendor>(entity =>
    //    {
    //        entity.HasKey(e => e.Id).HasName("PK__Vendor__3214EC27BDFA64C3");
    //    });

    //    OnModelCreatingPartial(modelBuilder);
    //}

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
