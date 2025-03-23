using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("GoiDichVu")]
public partial class GoiDichVu
{
    [Key]
    [Column("IDGoiDichVu")]
    public int IdgoiDichVu { get; set; }

    [Column("TenGoiDV")]
    [StringLength(255)]
    public string TenGoiDv { get; set; } = null!;

    [StringLength(255)]
    public string? HinhAnh { get; set; }

    [StringLength(1000)]
    public string? MoTa { get; set; }

    public string? ThongTinChiTiet { get; set; }

    [Column("IDDichVuDN")]
    public int IddichVuDn { get; set; }

    [InverseProperty("IdgoiDichVuNavigation")]
    public virtual ICollection<CthoaDonDoanhNghiep> CthoaDonDoanhNghieps { get; set; } = new List<CthoaDonDoanhNghiep>();

    [ForeignKey("IddichVuDn")]
    [InverseProperty("GoiDichVus")]
    public virtual DichVuDoanhNghiep? IddichVuDnNavigation { get; set; } = null!;
}
