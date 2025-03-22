using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("DichVu")]
[Index("TenDichVu", Name = "UQ__DichVu__A77D06892183F658", IsUnique = true)]
public partial class DichVu
{
    [Key]
    [Column("IDDichVu")]
    public int IddichVu { get; set; }

    [StringLength(255)]
    public string TenDichVu { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [StringLength(255)]
    public string? AnhDichVu { get; set; }

    [InverseProperty("IddichVuNavigation")]
    public virtual ICollection<LoaiDichVuDiDong> LoaiDichVuDiDongs { get; set; } = new List<LoaiDichVuDiDong>();

    [InverseProperty("IddichVuNavigation")]
    public virtual ICollection<LoaiDichVuKhac> LoaiDichVuKhacs { get; set; } = new List<LoaiDichVuKhac>();

    [InverseProperty("IddichVuNavigation")]
    public virtual ICollection<NhomDichVuDoanhNghiep> NhomDichVuDoanhNghieps { get; set; } = new List<NhomDichVuDoanhNghiep>();

    [InverseProperty("IddichVuNavigation")]
    public virtual ICollection<Sim> Sims { get; set; } = new List<Sim>();
}
