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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LineItem>(entity =>
        {
            entity.ToTable("LineItem");

            entity.HasKey(e => e.ID).HasName("PK__LineItem__3214EC27FC1306F5");

            entity.Property(li => li.ID).HasColumnName("ID");
            entity.Property(li => li.RequestID).HasColumnName("RequestId");
            entity.Property(li => li.ProductID).HasColumnName("ProductId");
            entity.Property(li => li.Quantity);

            entity.HasIndex(li => new { li.RequestID, li.ProductID })
                .IsUnique()
                .HasDatabaseName("req_pdt");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.LineItems)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LineItem__Produc__3B75D760")
                .IsRequired();

            entity.HasOne(d => d.Request)
                .WithMany(p => p.LineItems)
                .HasForeignKey(d => d.RequestID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LineItem__Reques__3A81B327")
                .IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.HasKey(e => e.ID).HasName("PK__Product__3214EC27050DF5A2");

            entity.Property(p => p.ID).HasColumnName("ID");
            entity.Property(p => p.VendorID).HasColumnName("VendorID");
            entity.Property(p => p.PartNumber).HasMaxLength(50).IsUnicode(false);
            entity.Property(p => p.Name).HasMaxLength(150).IsUnicode(false);
            entity.Property(p => p.Unit).HasMaxLength(255).IsUnicode(false);
            entity.Property(p => p.PhotoPath).HasMaxLength(255).IsUnicode(false);
            entity.Property(p => p.Price).HasColumnType("decimal(10, 2)");

            entity.HasIndex(p => new { p.VendorID, p.PartNumber })
                .IsUnique()
                .HasDatabaseName("vendor_part");

            entity.HasOne(d => d.Vendor)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.VendorID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__VendorI__31EC6D26")
                .IsRequired();
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable("Request");

            entity.HasKey(e => e.ID).HasName("PK__Request__3214EC274088B678");

            entity.Property(e => e.Status).HasDefaultValue("NEW");

            entity.Property(r => r.ID).HasColumnName("ID");
            entity.Property(r => r.UserID).HasColumnName("UserID");
            entity.Property(r => r.RequestNumber).HasMaxLength(20).IsUnicode(false);
            entity.Property(r => r.Description).HasMaxLength(100).IsUnicode(false);
            entity.Property(r => r.Justification).HasMaxLength(255).IsUnicode(false);
            entity.Property(r => r.DeliveryMode).HasMaxLength(25).IsUnicode(false);
            entity.Property(r => r.Status).HasMaxLength(20).IsUnicode(false).HasDefaultValue("NEW");
            entity.Property(r => r.Total).HasColumnType("decimal(10, 2)");
            entity.Property(r => r.SubmittedDate).HasColumnType("datetime");
            entity.Property(r => r.ReasonForRejection).HasMaxLength(100).IsUnicode(false);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Requests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("requesttouser")
                .IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(e => e.ID).HasName("PK__User__3214EC27CD979925");

            entity.Property(e => e.Admin).HasDefaultValue(false);
            entity.Property(e => e.Reviewer).HasDefaultValue(false);
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Vendor__3214EC27BDFA64C3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
