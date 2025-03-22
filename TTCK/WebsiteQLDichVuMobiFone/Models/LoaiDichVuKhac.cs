using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("LoaiDichVuKhac")]
[Index("TenLoaiDichVu", Name = "UQ__LoaiDich__EA233FD240C73F6D", IsUnique = true)]
public partial class LoaiDichVuKhac
{
    [Key]
    [Column("IDLoaiDichVuKhac")]
    public int IdloaiDichVuKhac { get; set; }

    [Column("IDDichVu")]
    public int IddichVu { get; set; }

    [StringLength(255)]
    public string TenLoaiDichVu { get; set; } = null!;

    [ForeignKey("IddichVu")]
    [InverseProperty("LoaiDichVuKhacs")]
    public virtual DichVu IddichVuNavigation { get; set; } = null!;

    [InverseProperty("IdloaiDichVuKhacNavigation")]
    public virtual ICollection<SanPhamDichVuKhac> SanPhamDichVuKhacs { get; set; } = new List<SanPhamDichVuKhac>();
}
