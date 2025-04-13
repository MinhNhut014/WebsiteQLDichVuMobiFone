using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("LoaiDichVuDiDong")]
[Index("TenLoaiDichVu", Name = "UQ__LoaiDich__EA233FD2538E709B", IsUnique = true)]
public partial class LoaiDichVuDiDong
{
    [Key]
    [Column("IDLoaiDichVu")]
    public int IdloaiDichVu { get; set; }

    [Column("IDDichVu")]
    public int IddichVu { get; set; }

    [StringLength(255)]
    public string TenLoaiDichVu { get; set; } = null!;

    [ForeignKey("IddichVu")]
    [InverseProperty("LoaiDichVuDiDongs")]
    public virtual DichVu? IddichVuNavigation { get; set; } = null!;

    [InverseProperty("IdloaiDichVuNavigation")]
    public virtual ICollection<LoaiGoiDangKy> LoaiGoiDangKies { get; set; } = new List<LoaiGoiDangKy>();
}
