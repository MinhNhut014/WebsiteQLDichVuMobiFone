using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("HoaDonDichVu")]
public partial class HoaDonDichVu
{
    [Key]
    [Column("IDHoaDonDV")]
    public int IdhoaDonDv { get; set; }

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
    public string? DiaChi { get; set; } = null!;

    [InverseProperty("IdhoaDonDvNavigation")]
    public virtual ICollection<CthoaDonDichVu> CthoaDonDichVus { get; set; } = new List<CthoaDonDichVu>();

    [ForeignKey("IdnguoiDung")]
    [InverseProperty("HoaDonDichVus")]
    public virtual NguoiDung IdnguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdtrangThai")]
    [InverseProperty("HoaDonDichVus")]
    public virtual TrangThaiDonHang IdtrangThaiNavigation { get; set; } = null!;
}
