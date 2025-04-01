using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("LoaiSo")]
[Index("TenLoaiSo", Name = "UQ__LoaiSo__F434DB2637E8DE8E", IsUnique = true)]
public partial class LoaiSo
{
    [Key]
    [Column("IDLoaiSo")]
    public int IdloaiSo { get; set; }

    [StringLength(50)]
    public string TenLoaiSo { get; set; } = null!;

    [InverseProperty("IdloaiSoNavigation")]
    public virtual ICollection<Sim> Sims { get; set; } = new List<Sim>();
}
