using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("TrangThaiSim")]
[Index("TenTrangThai", Name = "UQ__TrangTha__9489EF6638CF761F", IsUnique = true)]
public partial class TrangThaiSim
{
    [Key]
    [Column("IDTrangThaiSim")]
    public int IdtrangThaiSim { get; set; }

    [StringLength(50)]
    public string TenTrangThai { get; set; } = null!;

    [InverseProperty("IdtrangThaiSimNavigation")]
    public virtual ICollection<Sim> Sims { get; set; } = new List<Sim>();
}
