using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("LoaiGoiDangKy")]
[Index("TenLoaiGoi", Name = "UQ__LoaiGoiD__9BA443BC070BA267", IsUnique = true)]
public partial class LoaiGoiDangKy
{
    [Key]
    [Column("IDLoaiGoi")]
    public int IdloaiGoi { get; set; }

    [Display(Name = "Tên Loại Gói")]
    [StringLength(255)]
    public string TenLoaiGoi { get; set; } = null!;

    [Display(Name = "Loại Dịch Vụ")]
    [Column("IDLoaiDichVu")]
    public int IdloaiDichVu { get; set; }

    [InverseProperty("IdloaiGoiNavigation")]
    public virtual ICollection<GoiDangKy> GoiDangKies { get; set; } = new List<GoiDangKy>();

    [ForeignKey("IdloaiDichVu")]
    [InverseProperty("LoaiGoiDangKies")]
    public virtual LoaiDichVuDiDong? IdloaiDichVuNavigation { get; set; } = null!;
}
