using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {

    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public virtual DbSet<ChiTietHd> ChiTietHds { get; set; }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<TrangThai> TrangThais { get; set; }

    public virtual DbSet<VChiTietHoaDon> VChiTietHoaDons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietHd>(entity =>
        {
            entity.HasKey(e => e.MaCt).HasName("PK_OrderDetails");

            entity.ToTable("ChiTietHD");

            entity.Property(e => e.MaCt).HasColumnName("MaCT");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaHh).HasColumnName("MaHH");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaHd)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.MaHhNavigation).WithMany(p => p.ChiTietHds)
                .HasForeignKey(d => d.MaHh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");
        });

        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.HasKey(e => e.MaHh).HasName("PK_Products");

            entity.ToTable("HangHoa");

            entity.Property(e => e.MaHh).HasColumnName("MaHH");
            entity.Property(e => e.DonGia).HasDefaultValue(0.0);
            entity.Property(e => e.Hinh).HasMaxLength(50);
            entity.Property(e => e.TenHh)
                .HasMaxLength(50)
                .HasColumnName("TenHH");

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.HangHoas)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK_Products_Categories");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK_Orders");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.CachThanhToan)
                .HasMaxLength(50)
                .HasDefaultValue("Cash");
            entity.Property(e => e.CachVanChuyen)
                .HasMaxLength(50)
                .HasDefaultValue("Airline");
            entity.Property(e => e.DiaChi).HasMaxLength(60);
            entity.Property(e => e.DienThoai).HasMaxLength(24);
            entity.Property(e => e.GhiChu).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.MaTrangThaiNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaTrangThai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_TrangThai");
        });

        modelBuilder.Entity<TrangThai>(entity =>
        {
            entity.HasKey(e => e.MaTrangThai);

            entity.ToTable("TrangThai");

            entity.Property(e => e.MaTrangThai).ValueGeneratedNever();
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenTrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK_Customers");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiaChi).HasMaxLength(60);
            entity.Property(e => e.DienThoai).HasMaxLength(24);
            entity.Property(e => e.HieuLuc).HasDefaultValue(true);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MatKhau).HasMaxLength(50);
            entity.Property(e => e.VaiTro).HasDefaultValue(1);
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK_Categories");

            entity.ToTable("Loai");

            entity.Property(e => e.TenLoai).HasMaxLength(50);
        });

        modelBuilder.Entity<VChiTietHoaDon>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vChiTietHoaDon");

            entity.Property(e => e.MaCt).HasColumnName("MaCT");
            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.MaHh).HasColumnName("MaHH");
            entity.Property(e => e.TenHh)
                .HasMaxLength(50)
                .HasColumnName("TenHH");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
