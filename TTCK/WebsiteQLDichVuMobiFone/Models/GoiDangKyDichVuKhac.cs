using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("GoiDangKyDichVuKhac")]
[Index("TenGoi", Name = "UQ__GoiDangK__32A249F671EAC8A7", IsUnique = true)]
public partial class GoiDangKyDichVuKhac
{
    [Key]
    [Column("IDGoiDangKy")]
    public int IdgoiDangKy { get; set; }

    [StringLength(255)]
    public string TenGoi { get; set; } = null!;

    public int? GiaGoi { get; set; }

    [StringLength(50)]
    public string? ThoiHan { get; set; }

    [Column("IDSanPham")]
    public int IdsanPham { get; set; }

    [InverseProperty("IdgoiDangKyDvkNavigation")]
    public virtual ICollection<CthoaDonDichVu> CthoaDonDichVus { get; set; } = new List<CthoaDonDichVu>();

    [ForeignKey("IdsanPham")]
    [InverseProperty("GoiDangKyDichVuKhacs")]
    public virtual SanPhamDichVuKhac? IdsanPhamNavigation { get; set; } = null!;

    [InverseProperty("IdgoiDangKyNavigation")]
    public virtual ICollection<SimGoiDangKyDichVuKhac> SimGoiDangKyDichVuKhacs { get; set; } = new List<SimGoiDangKyDichVuKhac>();
}
