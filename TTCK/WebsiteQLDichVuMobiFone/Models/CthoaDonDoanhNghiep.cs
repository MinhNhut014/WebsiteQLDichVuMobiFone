using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("CTHoaDonDoanhNghiep")]
public partial class CthoaDonDoanhNghiep
{
    [Key]
    [Column("IDCTHoaDonDN")]
    public int IdcthoaDonDn { get; set; }

    [Column("IDHoaDonDN")]
    public int IdhoaDonDn { get; set; }

    [Column("IDGoiDichVu")]
    public int IdgoiDichVu { get; set; }

    public int? DonGia { get; set; }

    public short? SoLuong { get; set; }

    public int? ThanhTien { get; set; }

    [ForeignKey("IdgoiDichVu")]
    [InverseProperty("CthoaDonDoanhNghieps")]
    public virtual GoiDichVu? IdgoiDichVuNavigation { get; set; } = null!;

    [ForeignKey("IdhoaDonDn")]
    [InverseProperty("CthoaDonDoanhNghieps")]
    public virtual HoaDonDoanhNghiep? IdhoaDonDnNavigation { get; set; } = null!;
}
