using System;
using System.Collections.Generic;
using AviPriceUI.Models;
using Microsoft.EntityFrameworkCore;

namespace AviPriceUI.Data;

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

    public virtual DbSet<CellMatrix> CellMatrices { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Matrix> Matrices { get; set; }

    public virtual DbSet<UserSegment> UserSegments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=AvitoTest;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory);

            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(30);

            entity.HasOne(d => d.IdParentCategoryNavigation).WithMany(p => p.InverseIdParentCategoryNavigation)
                .HasForeignKey(d => d.IdParentCategory)
                .HasConstraintName("FK_Category_Category");
        });

        modelBuilder.Entity<CellMatrix>(entity =>
        {
            entity.HasKey(e => e.IdCellMatrix);

            entity.ToTable("CellMatrix");

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

            entity.Property(e => e.Name).HasMaxLength(30);

            entity.HasOne(d => d.IdParentLocationNavigation).WithMany(p => p.InverseIdParentLocationNavigation)
                .HasForeignKey(d => d.IdParentLocation)
                .HasConstraintName("FK_Location_Location");
        });

        modelBuilder.Entity<Matrix>(entity =>
        {
            entity.HasKey(e => e.IdMatrix);

            entity.ToTable("Matrix");

            entity.Property(e => e.Name).HasMaxLength(30);

            entity.HasOne(d => d.IdUserSegmentNavigation).WithMany(p => p.Matrices)
                .HasForeignKey(d => d.IdUserSegment)
                .HasConstraintName("FK_Matrix_UserSegment");
        });

        modelBuilder.Entity<UserSegment>(entity =>
        {
            entity.HasKey(e => e.IdUserSegment);

            entity.ToTable("UserSegment");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
