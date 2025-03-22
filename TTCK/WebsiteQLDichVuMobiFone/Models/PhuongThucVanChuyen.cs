using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("PhuongThucVanChuyen")]
[Index("TenVanChuyen", Name = "UQ__PhuongTh__C9194EFB82661348", IsUnique = true)]
public partial class PhuongThucVanChuyen
{
    [Key]
    [Column("IDPhuongThucVC")]
    public int IdphuongThucVc { get; set; }

    [StringLength(255)]
    public string TenVanChuyen { get; set; } = null!;

    [StringLength(1000)]
    public string? MoTa { get; set; }

    public int? GiaVanChuyen { get; set; }

    [InverseProperty("IdphuongThucVcNavigation")]
    public virtual ICollection<HoaDonSim> HoaDonSims { get; set; } = new List<HoaDonSim>();
}
