using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("GoiDangKy")]
[Index("TenGoi", Name = "UQ__GoiDangK__32A249F65BDE75EA", IsUnique = true)]
public partial class GoiDangKy
{
    [Key]
    [Column("IDGoiDangKy")]
    public int IdgoiDangKy { get; set; }

    [StringLength(255)]
    public string TenGoi { get; set; } = null!;

    public int GiaGoi { get; set; }

    [StringLength(50)]
    public string? ThoiHan { get; set; }

    [StringLength(255)]
    public string? TinhNang { get; set; }

    [Column("IDLoaiGoi")]
    public int IdloaiGoi { get; set; }

    [StringLength(1000)]
    public string? ThongTinGoi { get; set; }

    public string? ThongTinChiTiet { get; set; }

    [InverseProperty("IdgoiDangKyNavigation")]
    public virtual ICollection<CthoaDonDichVu> CthoaDonDichVus { get; set; } = new List<CthoaDonDichVu>();

    [InverseProperty("IdgoiDangKyNavigation")]
    public virtual ICollection<CthoaDonSim> CthoaDonSims { get; set; } = new List<CthoaDonSim>();

    [ForeignKey("IdloaiGoi")]
    [InverseProperty("GoiDangKies")]
    public virtual LoaiGoiDangKy? IdloaiGoiNavigation { get; set; } = null!;

    [InverseProperty("GoiDangKyDiKemNavigation")]
    public virtual ICollection<Sim> Sims { get; set; } = new List<Sim>();
}
