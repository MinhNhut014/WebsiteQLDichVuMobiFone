using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("LoaiGoiDangKy")]
[Index("TenLoaiGoi", Name = "UQ__LoaiGoiD__9BA443BCA909796F", IsUnique = true)]
public partial class LoaiGoiDangKy
{
    [Key]
    [Column("IDLoaiGoi")]
    public int IdloaiGoi { get; set; }

    [StringLength(255)]
    public string TenLoaiGoi { get; set; } = null!;

    [Column("IDLoaiDichVu")]
    public int IdloaiDichVu { get; set; }

    [InverseProperty("IdloaiGoiNavigation")]
    public virtual ICollection<GoiDangKy> GoiDangKies { get; set; } = new List<GoiDangKy>();

    [ForeignKey("IdloaiDichVu")]
    [InverseProperty("LoaiGoiDangKies")]
    public virtual LoaiDichVuDiDong? IdloaiDichVuNavigation { get; set; } = null!;
}
