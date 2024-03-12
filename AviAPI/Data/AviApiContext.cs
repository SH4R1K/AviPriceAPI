using System;
using System.Collections.Generic;
using AviAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AviAPI.Data;

public partial class AviApiContext : DbContext
{
    public AviApiContext()
    {
    }

    public AviApiContext(DbContextOptions<AviApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryTreePath> CategoryTreePaths { get; set; }

    public virtual DbSet<CellMatrix> CellMatrices { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<LocationTreePath> LocationTreePaths { get; set; }

    public virtual DbSet<Matrix> Matrices { get; set; }

    public virtual DbSet<UserSegment> UserSegments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=AviDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory);

            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<CategoryTreePath>(entity =>
        {
            entity.HasKey(e => new { e.Ancestor, e.Descendant });

            entity.ToTable("CategoryTreePath");

            entity.Property(e => e.Ancestor).HasColumnName("ancestor");
            entity.Property(e => e.Descendant).HasColumnName("descendant");
            entity.Property(e => e.Depth).HasColumnName("depth");

            entity.HasOne(d => d.AncestorNavigation).WithMany(p => p.CategoryTreePathAncestorNavigations)
                .HasForeignKey(d => d.Ancestor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CategoryTreePath_Category");

            entity.HasOne(d => d.DescendantNavigation).WithMany(p => p.CategoryTreePathDescendantNavigations)
                .HasForeignKey(d => d.Descendant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CategoryTreePath_Category1");
        });

        modelBuilder.Entity<CellMatrix>(entity =>
        {
            entity.HasKey(e => e.IdCellMatrix);

            entity.ToTable("CellMatrix");

            entity.HasIndex(e => new { e.IdLocation, e.IdCategory, e.IdMatrix }, "UQ_CellMatrix").IsUnique();

            entity.Property(e => e.Price).HasColumnType("decimal(7, 2)");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.CellMatrices)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CellMatrix_Category");

            entity.HasOne(d => d.IdLocationNavigation).WithMany(p => p.CellMatrices)
                .HasForeignKey(d => d.IdLocation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CellMatrix_Location");

            entity.HasOne(d => d.IdMatrixNavigation).WithMany(p => p.CellMatrices)
                .HasForeignKey(d => d.IdMatrix)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CellMatrix_Matrix");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.IdLocation);

            entity.ToTable("Location");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<LocationTreePath>(entity =>
        {
            entity.HasKey(e => new { e.Ancestor, e.Descendant });

            entity.ToTable("LocationTreePath");

            entity.Property(e => e.Ancestor).HasColumnName("ancestor");
            entity.Property(e => e.Descendant).HasColumnName("descendant");
            entity.Property(e => e.Depth).HasColumnName("depth");

            entity.HasOne(d => d.AncestorNavigation).WithMany(p => p.LocationTreePathAncestorNavigations)
                .HasForeignKey(d => d.Ancestor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LocationTreePath_Location");

            entity.HasOne(d => d.DescendantNavigation).WithMany(p => p.LocationTreePathDescendantNavigations)
                .HasForeignKey(d => d.Descendant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LocationTreePath_Location1");
        });

        modelBuilder.Entity<Matrix>(entity =>
        {
            entity.HasKey(e => e.IdMatrix);

            entity.ToTable("Matrix");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.IdUserSegmentNavigation).WithMany(p => p.Matrices)
                .HasForeignKey(d => d.IdUserSegment)
                .HasConstraintName("FK_Matrix_UserSegment");
        });

        modelBuilder.Entity<UserSegment>(entity =>
        {
            entity.HasKey(e => e.IdUserSegment);

            entity.ToTable("UserSegment");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
