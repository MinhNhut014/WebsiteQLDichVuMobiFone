using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("CTHoaDonSim")]
public partial class CthoaDonSim
{
    [Key]
    [Column("IDCTHoaDonSim")]
    public int IdcthoaDonSim { get; set; }

    [Column("IDHoaDonSim")]
    public int IdhoaDonSim { get; set; }

    [Column("IDSim")]
    public int Idsim { get; set; }

    [Column("IDGoiDangKy")]
    public int IdgoiDangKy { get; set; }

    public int? DonGia { get; set; }

    public short? SoLuong { get; set; }

    public int? ThanhTien { get; set; }

    [ForeignKey("IdgoiDangKy")]
    [InverseProperty("CthoaDonSims")]
    public virtual GoiDangKy IdgoiDangKyNavigation { get; set; } = null!;

    [ForeignKey("IdhoaDonSim")]
    [InverseProperty("CthoaDonSims")]
    public virtual HoaDonSim IdhoaDonSimNavigation { get; set; } = null!;

    [ForeignKey("Idsim")]
    [InverseProperty("CthoaDonSims")]
    public virtual Sim IdsimNavigation { get; set; } = null!;
}
