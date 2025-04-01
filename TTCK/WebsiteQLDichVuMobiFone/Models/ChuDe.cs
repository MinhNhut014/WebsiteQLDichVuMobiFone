using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("ChuDe")]
public partial class ChuDe
{
    [Key]
    [Column("IDChuDe")]
    public int IdchuDe { get; set; }

    [StringLength(100)]
    public string? TenChuDe { get; set; } = null!;

    [InverseProperty("IdTheLoaiNavigation")]
    public virtual ICollection<TinTuc> TinTucs { get; set; } = new List<TinTuc>();
}
