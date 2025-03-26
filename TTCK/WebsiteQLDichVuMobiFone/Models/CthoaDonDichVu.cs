using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("CTHoaDonDichVu")]
public partial class CthoaDonDichVu
{
    [Key]
    [Column("IDCTHoaDonDV")]
    public int IdcthoaDonDv { get; set; }

    [Column("IDHoaDonDV")]
    public int IdhoaDonDv { get; set; }

    [Column("IDGoiDangKy")]
    public int? IdgoiDangKy { get; set; }

    [Column("IDGoiDangKyDVK")]
    public int? IdgoiDangKyDvk { get; set; }

    public int? DonGia { get; set; }

    public short? SoLuong { get; set; }

    public int? ThanhTien { get; set; }

    [ForeignKey("IdgoiDangKyDvk")]
    [InverseProperty("CthoaDonDichVus")]
    public virtual GoiDangKyDichVuKhac? IdgoiDangKyDvkNavigation { get; set; }

    [ForeignKey("IdgoiDangKy")]
    [InverseProperty("CthoaDonDichVus")]
    public virtual GoiDangKy? IdgoiDangKyNavigation { get; set; }

    [ForeignKey("IdhoaDonDv")]
    [InverseProperty("CthoaDonDichVus")]
    public virtual HoaDonDichVu? IdhoaDonDvNavigation { get; set; } = null!;
}
