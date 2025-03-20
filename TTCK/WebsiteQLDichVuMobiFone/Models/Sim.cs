using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("Sim")]
[Index("SoThueBao", Name = "UQ__Sim__F0752C9708795F97", IsUnique = true)]
public partial class Sim
{
    [Key]
    [Column("IDSim")]
    public int Idsim { get; set; }

    [Column("IDDichVu")]
    public int IddichVu { get; set; }

    [StringLength(11)]
    public string SoThueBao { get; set; } = null!;

    [Column("IDLoaiSo")]
    public int IdloaiSo { get; set; }

    [StringLength(255)]
    public string? KhuVucHoaMang { get; set; }

    public int? PhiHoaMang { get; set; }

    [Column("IDTrangThaiSim")]
    public int IdtrangThaiSim { get; set; }

    [InverseProperty("IdsimNavigation")]
    public virtual ICollection<CthoaDonSim> CthoaDonSims { get; set; } = new List<CthoaDonSim>();

    [ForeignKey("IddichVu")]
    [InverseProperty("Sims")]
    public virtual DichVu? IddichVuNavigation { get; set; } = null!;

    [ForeignKey("IdloaiSo")]
    [InverseProperty("Sims")]
    public virtual LoaiSo? IdloaiSoNavigation { get; set; } = null!;

    [ForeignKey("IdtrangThaiSim")]
    [InverseProperty("Sims")]
    public virtual TrangThaiSim? IdtrangThaiSimNavigation { get; set; } = null!;
}
