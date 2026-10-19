using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyChiTieuApp.Models;

public partial class QlChiTieuContext : DbContext
{
    public QlChiTieuContext()
    {
    }

    public QlChiTieuContext(DbContextOptions<QlChiTieuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTieu> ChiTieus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connStr = Configuration.ConfigurationHelper.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connStr))
            {
                connStr = "Server=localhost;Database=QL_ChiTieu;Integrated Security=True;TrustServerCertificate=True";
            }
            optionsBuilder.UseSqlServer(connStr);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTieu>(entity =>
        {
            entity.HasKey(e => e.MaCt).HasName("PK__ChiTieu__27258E74E3301CDB");

            entity.ToTable("ChiTieu");

            entity.Property(e => e.MaCt).HasColumnName("MaCT");
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.LoaiChi).HasMaxLength(30);
            entity.Property(e => e.NoiDung).HasMaxLength(100);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
