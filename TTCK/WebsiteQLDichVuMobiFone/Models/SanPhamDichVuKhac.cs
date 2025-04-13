using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("SanPhamDichVuKhac")]
[Index("TenSanPham", Name = "UQ__SanPhamD__FCA804696FF566BB", IsUnique = true)]
public partial class SanPhamDichVuKhac
{
    [Key]
    [Column("IDSanPham")]
    public int IdsanPham { get; set; }

    [StringLength(255)]
    public string TenSanPham { get; set; } = null!;

    [StringLength(255)]
    public string? HinhAnh { get; set; }

    [StringLength(1000)]
    public string? MoTa { get; set; }

    public string? ThongTinChiTiet { get; set; }

    [Column("IDLoaiDichVuKhac")]
    public int IdloaiDichVuKhac { get; set; }

    [InverseProperty("IdsanPhamNavigation")]
    public virtual ICollection<GoiDangKyDichVuKhac> GoiDangKyDichVuKhacs { get; set; } = new List<GoiDangKyDichVuKhac>();

    [ForeignKey("IdloaiDichVuKhac")]
    [InverseProperty("SanPhamDichVuKhacs")]
    public virtual LoaiDichVuKhac? IdloaiDichVuKhacNavigation { get; set; } = null!;
}
