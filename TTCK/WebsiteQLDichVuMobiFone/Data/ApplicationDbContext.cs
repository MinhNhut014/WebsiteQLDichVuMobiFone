﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebsiteQLDichVuMobiFone.Models;

namespace WebsiteQLDichVuMobiFone.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BinhLuanBaiViet> BinhLuanBaiViets { get; set; }

    public virtual DbSet<ChuDe> ChuDes { get; set; }

    public virtual DbSet<CthoaDonDichVu> CthoaDonDichVus { get; set; }

    public virtual DbSet<CthoaDonDoanhNghiep> CthoaDonDoanhNghieps { get; set; }

    public virtual DbSet<CthoaDonSim> CthoaDonSims { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<DichVuDoanhNghiep> DichVuDoanhNghieps { get; set; }

    public virtual DbSet<GoiDangKy> GoiDangKies { get; set; }

    public virtual DbSet<GoiDangKyDichVuKhac> GoiDangKyDichVuKhacs { get; set; }

    public virtual DbSet<GoiDichVu> GoiDichVus { get; set; }

    public virtual DbSet<HoaDonDichVu> HoaDonDichVus { get; set; }

    public virtual DbSet<HoaDonDoanhNghiep> HoaDonDoanhNghieps { get; set; }

    public virtual DbSet<HoaDonSim> HoaDonSims { get; set; }

    public virtual DbSet<LoaiDichVuDiDong> LoaiDichVuDiDongs { get; set; }

    public virtual DbSet<LoaiDichVuKhac> LoaiDichVuKhacs { get; set; }

    public virtual DbSet<LoaiGoiDangKy> LoaiGoiDangKies { get; set; }

    public virtual DbSet<LoaiSo> LoaiSos { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhomDichVuDoanhNghiep> NhomDichVuDoanhNghieps { get; set; }

    public virtual DbSet<PhuongThucVanChuyen> PhuongThucVanChuyens { get; set; }

    public virtual DbSet<SanPhamDichVuKhac> SanPhamDichVuKhacs { get; set; }

    public virtual DbSet<Sim> Sims { get; set; }

    public virtual DbSet<TinTuc> TinTucs { get; set; }

    public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }

    public virtual DbSet<TrangThaiSim> TrangThaiSims { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2108PGG\\SQLEXPRESS01;Initial Catalog=qldichvumobifone;TrustServerCertificate=True;Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinhLuanBaiViet>(entity =>
        {
            entity.HasKey(e => e.IdbinhLuan).HasName("PK__BinhLuan__5CDBC03CCB3E70BA");

            entity.Property(e => e.NgayBinhLuan).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdTinTucNavigation).WithMany(p => p.BinhLuanBaiViets).HasConstraintName("FK__BinhLuanB__IdTin__367C1819");

            entity.HasOne(d => d.NguoiDung).WithMany(p => p.BinhLuanBaiViets).HasConstraintName("FK__BinhLuanB__Nguoi__37703C52");
        });

        modelBuilder.Entity<ChuDe>(entity =>
        {
            entity.HasKey(e => e.IdchuDe).HasName("PK__ChuDe__5C130CB89C64FDFE");
        });

        modelBuilder.Entity<CthoaDonDichVu>(entity =>
        {
            entity.HasKey(e => e.IdcthoaDonDv).HasName("PK__CTHoaDon__758AFF354C0836F7");

            entity.Property(e => e.DonGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue((short)1);

            entity.HasOne(d => d.IdgoiDangKyNavigation).WithMany(p => p.CthoaDonDichVus).HasConstraintName("FK__CTHoaDonD__IDGoi__1332DBDC");

            entity.HasOne(d => d.IdgoiDangKyDvkNavigation).WithMany(p => p.CthoaDonDichVus).HasConstraintName("FK__CTHoaDonD__IDGoi__14270015");

            entity.HasOne(d => d.IdhoaDonDvNavigation).WithMany(p => p.CthoaDonDichVus).HasConstraintName("FK__CTHoaDonD__IDHoa__123EB7A3");
        });

        modelBuilder.Entity<CthoaDonDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IdcthoaDonDn).HasName("PK__CTHoaDon__758AFF3DFBED2FFA");

            entity.HasOne(d => d.IdgoiDichVuNavigation).WithMany(p => p.CthoaDonDoanhNghieps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHoaDonD__IDGoi__2BFE89A6");

            entity.HasOne(d => d.IdhoaDonDnNavigation).WithMany(p => p.CthoaDonDoanhNghieps).HasConstraintName("FK__CTHoaDonD__IDHoa__2B0A656D");
        });

        modelBuilder.Entity<CthoaDonSim>(entity =>
        {
            entity.HasKey(e => e.IdcthoaDonSim).HasName("PK__CTHoaDon__447141A0F0345623");

            entity.Property(e => e.DonGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue((short)1);

            entity.HasOne(d => d.IdgoiDangKyNavigation).WithMany(p => p.CthoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHoaDonS__IDGoi__2180FB33");

            entity.HasOne(d => d.IdhoaDonSimNavigation).WithMany(p => p.CthoaDonSims).HasConstraintName("FK__CTHoaDonS__IDHoa__1F98B2C1");

            entity.HasOne(d => d.IdsimNavigation).WithMany(p => p.CthoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHoaDonS__IDSim__208CD6FA");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.IddichVu).HasName("PK__DichVu__C0C95928E69AC61B");
        });

        modelBuilder.Entity<DichVuDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IddichVuDn).HasName("PK__DichVuDo__64ECF549CC2C531A");

            entity.HasOne(d => d.IdnhomDichVuNavigation).WithMany(p => p.DichVuDoanhNghieps).HasConstraintName("FK__DichVuDoa__IDNho__656C112C");
        });

        modelBuilder.Entity<GoiDangKy>(entity =>
        {
            entity.HasKey(e => e.IdgoiDangKy).HasName("PK__GoiDangK__72344F022D0AAAA2");

            entity.Property(e => e.GiaGoi).HasDefaultValue(0);

            entity.HasOne(d => d.IdloaiGoiNavigation).WithMany(p => p.GoiDangKies).HasConstraintName("FK__GoiDangKy__IDLoa__5DCAEF64");
        });

        modelBuilder.Entity<GoiDangKyDichVuKhac>(entity =>
        {
            entity.HasKey(e => e.IdgoiDangKy).HasName("PK__GoiDangK__72344F02E1AE3E77");

            entity.Property(e => e.GiaGoi).HasDefaultValue(0);

            entity.HasOne(d => d.IdsanPhamNavigation).WithMany(p => p.GoiDangKyDichVuKhacs).HasConstraintName("FK__GoiDangKy__IDSan__02084FDA");
        });

        modelBuilder.Entity<GoiDichVu>(entity =>
        {
            entity.HasKey(e => e.IdgoiDichVu).HasName("PK__GoiDichV__A74519A3CD5950B5");

            entity.HasOne(d => d.IddichVuDnNavigation).WithMany(p => p.GoiDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoiDichVu__IDDic__68487DD7");
        });

        modelBuilder.Entity<HoaDonDichVu>(entity =>
        {
            entity.HasKey(e => e.IdhoaDonDv).HasName("PK__HoaDonDi__C9D2DCF2E029FA9B");

            entity.Property(e => e.NgayDatHang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0);

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.HoaDonDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDic__IDNgu__0C85DE4D");

            entity.HasOne(d => d.IdtrangThaiNavigation).WithMany(p => p.HoaDonDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDic__IDTra__0F624AF8");
        });

        modelBuilder.Entity<HoaDonDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IdhoaDonDn).HasName("PK__HoaDonDo__C9D2DCFA78AD5FB8");

            entity.Property(e => e.NgayDatHang).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.HoaDonDoanhNghieps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDoa__IDNgu__2645B050");

            entity.HasOne(d => d.IdtrangThaiNavigation).WithMany(p => p.HoaDonDoanhNghieps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDoa__IDTra__282DF8C2");
        });

        modelBuilder.Entity<HoaDonSim>(entity =>
        {
            entity.HasKey(e => e.IdhoaDonSim).HasName("PK__HoaDonSi__364462F786A8F31D");

            entity.Property(e => e.NgayDatHang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0);

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.HoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonSim__IDNgu__18EBB532");

            entity.HasOne(d => d.IdphuongThucVcNavigation).WithMany(p => p.HoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonSim__IDPhu__1CBC4616");

            entity.HasOne(d => d.IdtrangThaiNavigation).WithMany(p => p.HoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonSim__IDTra__1BC821DD");
        });

        modelBuilder.Entity<LoaiDichVuDiDong>(entity =>
        {
            entity.HasKey(e => e.IdloaiDichVu).HasName("PK__LoaiDich__7911AD8A696D8F92");

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.LoaiDichVuDiDongs).HasConstraintName("FK__LoaiDichV__IDDic__5535A963");
        });

        modelBuilder.Entity<LoaiDichVuKhac>(entity =>
        {
            entity.HasKey(e => e.IdloaiDichVuKhac).HasName("PK__LoaiDich__680849BB32B5C950");

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.LoaiDichVuKhacs).HasConstraintName("FK__LoaiDichV__IDDic__797309D9");
        });

        modelBuilder.Entity<LoaiGoiDangKy>(entity =>
        {
            entity.HasKey(e => e.IdloaiGoi).HasName("PK__LoaiGoiD__1B03B7B1A00FEC88");

            entity.HasOne(d => d.IdloaiDichVuNavigation).WithMany(p => p.LoaiGoiDangKies).HasConstraintName("FK__LoaiGoiDa__IDLoa__59063A47");
        });

        modelBuilder.Entity<LoaiSo>(entity =>
        {
            entity.HasKey(e => e.IdloaiSo).HasName("PK__LoaiSo__B57A5AFA3E9366C7");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.IdnguoiDung).HasName("PK__NguoiDun__FCD7DB097525C3EE");

            entity.Property(e => e.Quyen).HasDefaultValue(0);
            entity.Property(e => e.Trangthai).HasDefaultValue(1);
        });

        modelBuilder.Entity<NhomDichVuDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IdnhomDichVu).HasName("PK__NhomDich__9E06E4F3C3586DC0");

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.NhomDichVuDoanhNghieps).HasConstraintName("FK__NhomDichV__IDDic__619B8048");
        });

        modelBuilder.Entity<PhuongThucVanChuyen>(entity =>
        {
            entity.HasKey(e => e.IdphuongThucVc).HasName("PK__PhuongTh__A8430B57D8018B1A");

            entity.Property(e => e.GiaVanChuyen).HasDefaultValue(0);
        });

        modelBuilder.Entity<SanPhamDichVuKhac>(entity =>
        {
            entity.HasKey(e => e.IdsanPham).HasName("PK__SanPhamD__9D45E58A89FB6DB5");

            entity.HasOne(d => d.IdloaiDichVuKhacNavigation).WithMany(p => p.SanPhamDichVuKhacs).HasConstraintName("FK__SanPhamDi__IDLoa__7D439ABD");
        });

        modelBuilder.Entity<Sim>(entity =>
        {
            entity.HasKey(e => e.Idsim).HasName("PK__Sim__A5CFB83CD68532C9");

            entity.Property(e => e.PhiHoaMang).HasDefaultValue(0);

            entity.HasOne(d => d.GoiDangKyDiKemNavigation).WithMany(p => p.Sims).HasConstraintName("FK__Sim__GoiDangKyDi__75A278F5");

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.Sims).HasConstraintName("FK__Sim__IDDichVu__71D1E811");

            entity.HasOne(d => d.IdloaiSoNavigation).WithMany(p => p.Sims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sim__IDLoaiSo__72C60C4A");

            entity.HasOne(d => d.IdtrangThaiSimNavigation).WithMany(p => p.Sims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sim__IDTrangThai__74AE54BC");
        });

        modelBuilder.Entity<TinTuc>(entity =>
        {
            entity.HasKey(e => e.IdTinTuc).HasName("PK__TinTuc__B782967688D443B1");

            entity.Property(e => e.LuotXem).HasDefaultValue(0);
            entity.Property(e => e.NgayDang).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdTheLoaiNavigation).WithMany(p => p.TinTucs).HasConstraintName("FK__TinTuc__IdTheLoa__32AB8735");
        });

        modelBuilder.Entity<TrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.IdtrangThai).HasName("PK__TrangTha__556586006ACDA113");
        });

        modelBuilder.Entity<TrangThaiSim>(entity =>
        {
            entity.HasKey(e => e.IdtrangThaiSim).HasName("PK__TrangTha__22D8CDF4C0EDC9CD");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
