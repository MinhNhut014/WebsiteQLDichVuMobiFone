using System;
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

    public virtual DbSet<GiaoDichNapTien> GiaoDichNapTiens { get; set; }

    public virtual DbSet<GoiDangKy> GoiDangKies { get; set; }

    public virtual DbSet<GoiDangKyDichVuKhac> GoiDangKyDichVuKhacs { get; set; }

    public virtual DbSet<GoiDichVu> GoiDichVus { get; set; }

    public virtual DbSet<HoaDonDichVu> HoaDonDichVus { get; set; }

    public virtual DbSet<HoaDonDoanhNghiep> HoaDonDoanhNghieps { get; set; }

    public virtual DbSet<HoaDonSim> HoaDonSims { get; set; }

    public virtual DbSet<LienHe> LienHes { get; set; }

    public virtual DbSet<LoaiDichVuDiDong> LoaiDichVuDiDongs { get; set; }

    public virtual DbSet<LoaiDichVuKhac> LoaiDichVuKhacs { get; set; }

    public virtual DbSet<LoaiGoiDangKy> LoaiGoiDangKies { get; set; }

    public virtual DbSet<LoaiSo> LoaiSos { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhomDichVuDoanhNghiep> NhomDichVuDoanhNghieps { get; set; }

    public virtual DbSet<PhuongThucVanChuyen> PhuongThucVanChuyens { get; set; }

    public virtual DbSet<SanPhamDichVuKhac> SanPhamDichVuKhacs { get; set; }

    public virtual DbSet<Sim> Sims { get; set; }

    public virtual DbSet<SimGoiDangKy> SimGoiDangKies { get; set; }

    public virtual DbSet<SimGoiDangKyDichVuKhac> SimGoiDangKyDichVuKhacs { get; set; }

    public virtual DbSet<TinTuc> TinTucs { get; set; }

    public virtual DbSet<TrangThaiDonHang> TrangThaiDonHangs { get; set; }

    public virtual DbSet<TrangThaiSim> TrangThaiSims { get; set; }

    public virtual DbSet<TrangThaiThanhToan> TrangThaiThanhToans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-2108PGG\\SQLEXPRESS01;Initial Catalog=qldichvumobifone;TrustServerCertificate=True;Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinhLuanBaiViet>(entity =>
        {
            entity.HasKey(e => e.IdbinhLuan).HasName("PK__BinhLuan__5CDBC03C2AC7FC1B");

            entity.Property(e => e.NgayBinhLuan).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdTinTucNavigation).WithMany(p => p.BinhLuanBaiViets).HasConstraintName("FK__BinhLuanB__IdTin__5224328E");

            entity.HasOne(d => d.NguoiDung).WithMany(p => p.BinhLuanBaiViets).HasConstraintName("FK__BinhLuanB__Nguoi__531856C7");
        });

        modelBuilder.Entity<ChuDe>(entity =>
        {
            entity.HasKey(e => e.IdchuDe).HasName("PK__ChuDe__5C130CB823B151FE");
        });

        modelBuilder.Entity<CthoaDonDichVu>(entity =>
        {
            entity.HasKey(e => e.IdcthoaDonDv).HasName("PK__CTHoaDon__758AFF35B2F9C178");

            entity.Property(e => e.DonGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue((short)1);

            entity.HasOne(d => d.IdgoiDangKyNavigation).WithMany(p => p.CthoaDonDichVus).HasConstraintName("FK__CTHoaDonD__IDGoi__2BFE89A6");

            entity.HasOne(d => d.IdgoiDangKyDvkNavigation).WithMany(p => p.CthoaDonDichVus).HasConstraintName("FK__CTHoaDonD__IDGoi__2CF2ADDF");

            entity.HasOne(d => d.IdhoaDonDvNavigation).WithMany(p => p.CthoaDonDichVus).HasConstraintName("FK__CTHoaDonD__IDHoa__2B0A656D");
        });

        modelBuilder.Entity<CthoaDonDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IdcthoaDonDn).HasName("PK__CTHoaDon__758AFF3DC299A554");

            entity.HasOne(d => d.IdgoiDichVuNavigation).WithMany(p => p.CthoaDonDoanhNghieps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHoaDonD__IDGoi__47A6A41B");

            entity.HasOne(d => d.IdhoaDonDnNavigation).WithMany(p => p.CthoaDonDoanhNghieps).HasConstraintName("FK__CTHoaDonD__IDHoa__46B27FE2");
        });

        modelBuilder.Entity<CthoaDonSim>(entity =>
        {
            entity.HasKey(e => e.IdcthoaDonSim).HasName("PK__CTHoaDon__447141A03A55DD63");

            entity.Property(e => e.DonGia).HasDefaultValue(0);
            entity.Property(e => e.SoLuong).HasDefaultValue((short)1);

            entity.HasOne(d => d.IdgoiDangKyNavigation).WithMany(p => p.CthoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHoaDonS__IDGoi__3C34F16F");

            entity.HasOne(d => d.IdhoaDonSimNavigation).WithMany(p => p.CthoaDonSims).HasConstraintName("FK__CTHoaDonS__IDHoa__3A4CA8FD");

            entity.HasOne(d => d.IdsimNavigation).WithMany(p => p.CthoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTHoaDonS__IDSim__3B40CD36");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.IddichVu).HasName("PK__DichVu__C0C9592835F62669");
        });

        modelBuilder.Entity<DichVuDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IddichVuDn).HasName("PK__DichVuDo__64ECF5493765563C");

            entity.HasOne(d => d.IdnhomDichVuNavigation).WithMany(p => p.DichVuDoanhNghieps).HasConstraintName("FK__DichVuDoa__IDNho__656C112C");
        });

        modelBuilder.Entity<GiaoDichNapTien>(entity =>
        {
            entity.HasKey(e => e.IdgiaoDich).HasName("PK__GiaoDich__5E5A4D81EA2C7B2A");

            entity.Property(e => e.NgayNap).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.GiaoDichNapTiens).HasConstraintName("FK__GiaoDichN__IDNgu__1EA48E88");

            entity.HasOne(d => d.IdsimNavigation).WithMany(p => p.GiaoDichNapTiens)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GiaoDichN__IDSim__1DB06A4F");

            entity.HasOne(d => d.IdtrangThaiThanhToanNavigation).WithMany(p => p.GiaoDichNapTiens).HasConstraintName("FK__GiaoDichN__IDTra__208CD6FA");
        });

        modelBuilder.Entity<GoiDangKy>(entity =>
        {
            entity.HasKey(e => e.IdgoiDangKy).HasName("PK__GoiDangK__72344F024B68AC1D");

            entity.Property(e => e.GiaGoi).HasDefaultValue(0);

            entity.HasOne(d => d.IdloaiGoiNavigation).WithMany(p => p.GoiDangKies).HasConstraintName("FK__GoiDangKy__IDLoa__5DCAEF64");
        });

        modelBuilder.Entity<GoiDangKyDichVuKhac>(entity =>
        {
            entity.HasKey(e => e.IdgoiDangKy).HasName("PK__GoiDangK__72344F02C5642CE1");

            entity.Property(e => e.GiaGoi).HasDefaultValue(0);

            entity.HasOne(d => d.IdsanPhamNavigation).WithMany(p => p.GoiDangKyDichVuKhacs).HasConstraintName("FK__GoiDangKy__IDSan__02FC7413");
        });

        modelBuilder.Entity<GoiDichVu>(entity =>
        {
            entity.HasKey(e => e.IdgoiDichVu).HasName("PK__GoiDichV__A74519A3CFC713F5");

            entity.HasOne(d => d.IddichVuDnNavigation).WithMany(p => p.GoiDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GoiDichVu__IDDic__68487DD7");
        });

        modelBuilder.Entity<HoaDonDichVu>(entity =>
        {
            entity.HasKey(e => e.IdhoaDonDv).HasName("PK__HoaDonDi__C9D2DCF27B0CF7DE");

            entity.Property(e => e.NgayDatHang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0);

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.HoaDonDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDic__IDNgu__245D67DE");

            entity.HasOne(d => d.IdtrangThaiNavigation).WithMany(p => p.HoaDonDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDic__IDTra__2739D489");

            entity.HasOne(d => d.IdtrangThaiThanhToanNavigation).WithMany(p => p.HoaDonDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDic__IDTra__282DF8C2");
        });

        modelBuilder.Entity<HoaDonDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IdhoaDonDn).HasName("PK__HoaDonDo__C9D2DCFA4B982E80");

            entity.Property(e => e.NgayDatHang).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.HoaDonDoanhNghieps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDoa__IDNgu__41EDCAC5");

            entity.HasOne(d => d.IdtrangThaiNavigation).WithMany(p => p.HoaDonDoanhNghieps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonDoa__IDTra__43D61337");
        });

        modelBuilder.Entity<HoaDonSim>(entity =>
        {
            entity.HasKey(e => e.IdhoaDonSim).HasName("PK__HoaDonSi__364462F790B5FCBD");

            entity.Property(e => e.NgayDatHang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TongTien).HasDefaultValue(0);

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.HoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonSim__IDNgu__32AB8735");

            entity.HasOne(d => d.IdphuongThucVcNavigation).WithMany(p => p.HoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonSim__IDPhu__37703C52");

            entity.HasOne(d => d.IdtrangThaiNavigation).WithMany(p => p.HoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonSim__IDTra__3587F3E0");

            entity.HasOne(d => d.IdtrangThaiThanhToanNavigation).WithMany(p => p.HoaDonSims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonSim__IDTra__367C1819");
        });

        modelBuilder.Entity<LienHe>(entity =>
        {
            entity.HasKey(e => e.IdlienHe).HasName("PK__LienHe__002B7E194C5F7906");

            entity.Property(e => e.NgayGui).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(false);

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.LienHes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LienHe__IDNguoiD__0F624AF8");
        });

        modelBuilder.Entity<LoaiDichVuDiDong>(entity =>
        {
            entity.HasKey(e => e.IdloaiDichVu).HasName("PK__LoaiDich__7911AD8A3A58F249");

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.LoaiDichVuDiDongs).HasConstraintName("FK__LoaiDichV__IDDic__5535A963");
        });

        modelBuilder.Entity<LoaiDichVuKhac>(entity =>
        {
            entity.HasKey(e => e.IdloaiDichVuKhac).HasName("PK__LoaiDich__680849BBAE5AA9CA");

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.LoaiDichVuKhacs).HasConstraintName("FK__LoaiDichV__IDDic__7A672E12");
        });

        modelBuilder.Entity<LoaiGoiDangKy>(entity =>
        {
            entity.HasKey(e => e.IdloaiGoi).HasName("PK__LoaiGoiD__1B03B7B15CD5FDBE");

            entity.HasOne(d => d.IdloaiDichVuNavigation).WithMany(p => p.LoaiGoiDangKies).HasConstraintName("FK__LoaiGoiDa__IDLoa__59063A47");
        });

        modelBuilder.Entity<LoaiSo>(entity =>
        {
            entity.HasKey(e => e.IdloaiSo).HasName("PK__LoaiSo__B57A5AFAF273378E");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.IdnguoiDung).HasName("PK__NguoiDun__FCD7DB0904C51A8D");

            entity.Property(e => e.Quyen).HasDefaultValue(0);
            entity.Property(e => e.Trangthai).HasDefaultValue(1);
        });

        modelBuilder.Entity<NhomDichVuDoanhNghiep>(entity =>
        {
            entity.HasKey(e => e.IdnhomDichVu).HasName("PK__NhomDich__9E06E4F30B469E5E");

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.NhomDichVuDoanhNghieps).HasConstraintName("FK__NhomDichV__IDDic__619B8048");
        });

        modelBuilder.Entity<PhuongThucVanChuyen>(entity =>
        {
            entity.HasKey(e => e.IdphuongThucVc).HasName("PK__PhuongTh__A8430B57D2AE9A7B");

            entity.Property(e => e.GiaVanChuyen).HasDefaultValue(0);
        });

        modelBuilder.Entity<SanPhamDichVuKhac>(entity =>
        {
            entity.HasKey(e => e.IdsanPham).HasName("PK__SanPhamD__9D45E58A045C811E");

            entity.HasOne(d => d.IdloaiDichVuKhacNavigation).WithMany(p => p.SanPhamDichVuKhacs).HasConstraintName("FK__SanPhamDi__IDLoa__7E37BEF6");
        });

        modelBuilder.Entity<Sim>(entity =>
        {
            entity.HasKey(e => e.Idsim).HasName("PK__Sim__A5CFB83C08191B05");

            entity.Property(e => e.PhiHoaMang).HasDefaultValue(0);
            entity.Property(e => e.SoDu).HasDefaultValue(0m);

            entity.HasOne(d => d.IddichVuNavigation).WithMany(p => p.Sims).HasConstraintName("FK__Sim__IDDichVu__71D1E811");

            entity.HasOne(d => d.IdloaiSoNavigation).WithMany(p => p.Sims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sim__IDLoaiSo__72C60C4A");

            entity.HasOne(d => d.IdnguoiDungNavigation).WithMany(p => p.Sims).HasConstraintName("FK__Sim__IDNguoiDung__75A278F5");

            entity.HasOne(d => d.IdtrangThaiSimNavigation).WithMany(p => p.Sims)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sim__IDTrangThai__74AE54BC");
        });

        modelBuilder.Entity<SimGoiDangKy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sim_GoiD__3214EC270D46700C");

            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdgoiDangKyNavigation).WithMany(p => p.SimGoiDangKies).HasConstraintName("FK__Sim_GoiDa__IDGoi__06CD04F7");

            entity.HasOne(d => d.IdsimNavigation).WithMany(p => p.SimGoiDangKies)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Sim_GoiDa__IDSim__05D8E0BE");
        });

        modelBuilder.Entity<SimGoiDangKyDichVuKhac>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sim_GoiD__3214EC27FED94B17");

            entity.Property(e => e.NgayDangKy).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdgoiDangKyNavigation).WithMany(p => p.SimGoiDangKyDichVuKhacs).HasConstraintName("FK__Sim_GoiDa__IDGoi__0B91BA14");

            entity.HasOne(d => d.IdsimNavigation).WithMany(p => p.SimGoiDangKyDichVuKhacs)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Sim_GoiDa__IDSim__0A9D95DB");
        });

        modelBuilder.Entity<TinTuc>(entity =>
        {
            entity.HasKey(e => e.IdTinTuc).HasName("PK__TinTuc__B78296769E924EB2");

            entity.Property(e => e.LuotXem).HasDefaultValue(0);
            entity.Property(e => e.NgayDang).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdTheLoaiNavigation).WithMany(p => p.TinTucs).HasConstraintName("FK__TinTuc__IdTheLoa__4E53A1AA");
        });

        modelBuilder.Entity<TrangThaiDonHang>(entity =>
        {
            entity.HasKey(e => e.IdtrangThai).HasName("PK__TrangTha__556586003E6EC32E");
        });

        modelBuilder.Entity<TrangThaiSim>(entity =>
        {
            entity.HasKey(e => e.IdtrangThaiSim).HasName("PK__TrangTha__22D8CDF4950465BC");
        });

        modelBuilder.Entity<TrangThaiThanhToan>(entity =>
        {
            entity.HasKey(e => e.IdtrangThaiThanhToan).HasName("PK__TrangTha__6CB8788167B76064");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
