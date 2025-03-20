using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("TinTuc")]
public partial class TinTuc
{
    [Key]
    public int IdTinTuc { get; set; }

    [StringLength(255)]
    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    [StringLength(255)]
    public string? AnhDaiDien { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDang { get; set; }

    public int? LuotXem { get; set; }

    public int? IdTheLoai { get; set; }

    [InverseProperty("IdTinTucNavigation")]
    public virtual ICollection<BinhLuanBaiViet> BinhLuanBaiViets { get; set; } = new List<BinhLuanBaiViet>();

    [ForeignKey("IdTheLoai")]
    [InverseProperty("TinTucs")]
    public virtual ChuDe? IdTheLoaiNavigation { get; set; }
}
