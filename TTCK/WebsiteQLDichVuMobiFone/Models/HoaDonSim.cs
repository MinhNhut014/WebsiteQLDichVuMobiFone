using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("HoaDonSim")]
[Index("MaHoaDonSim", Name = "UQ__HoaDonSi__3C1A07B21183E640", IsUnique = true)]
public partial class HoaDonSim
{
    [Key]
    [Column("IDHoaDonSim")]
    public int IdhoaDonSim { get; set; }

    [StringLength(50)]
    [BindNever]
    public string MaHoaDonSim { get; set; } = string.Empty;

    [Column("IDNguoiDung")]
    public int IdnguoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDatHang { get; set; }

    public int? TongTien { get; set; }

    [Column("IDTrangThai")]
    public int IdtrangThai { get; set; }

    [StringLength(255)]
    public string TenKhachHang { get; set; } = null!;

    [StringLength(11)]
    public string SoDienThoai { get; set; } = null!;

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string DiaDiemNhan { get; set; } = null!;

    [StringLength(50)]
    public string PhuongThucThanhToan { get; set; } = null!;

    [Column("IDTrangThaiThanhToan")]
    public int IdtrangThaiThanhToan { get; set; }

    [Column("IDPhuongThucVC")]
    public int IdphuongThucVc { get; set; }

    [InverseProperty("IdhoaDonSimNavigation")]
    public virtual ICollection<CthoaDonSim> CthoaDonSims { get; set; } = new List<CthoaDonSim>();

    [ForeignKey("IdnguoiDung")]
    [InverseProperty("HoaDonSims")]
    public virtual NguoiDung? IdnguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdphuongThucVc")]
    [InverseProperty("HoaDonSims")]
    public virtual PhuongThucVanChuyen? IdphuongThucVcNavigation { get; set; } = null!;

    [ForeignKey("IdtrangThai")]
    [InverseProperty("HoaDonSims")]
    public virtual TrangThaiDonHang? IdtrangThaiNavigation { get; set; } = null!;

    [ForeignKey("IdtrangThaiThanhToan")]
    [InverseProperty("HoaDonSims")]
    public virtual TrangThaiThanhToan? IdtrangThaiThanhToanNavigation { get; set; } = null!;
}
