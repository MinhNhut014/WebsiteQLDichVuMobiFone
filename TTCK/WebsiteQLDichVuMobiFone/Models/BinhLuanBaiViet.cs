using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("BinhLuanBaiViet")]
public partial class BinhLuanBaiViet
{
    [Key]
    [Column("IDBinhLuan")]
    public int IdbinhLuan { get; set; }

    public int? IdTinTuc { get; set; }

    [StringLength(100)]
    public string? HoTen { get; set; } = null!;

    [StringLength(1000)]
    public string? NoiDung { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayBinhLuan { get; set; }

    [Column("NguoiDungID")]
    public int? NguoiDungId { get; set; }

    [ForeignKey("IdTinTuc")]
    [InverseProperty("BinhLuanBaiViets")]
    public virtual TinTuc? IdTinTucNavigation { get; set; } = null!;

    [ForeignKey("NguoiDungId")]
    [InverseProperty("BinhLuanBaiViets")]
    public virtual NguoiDung? NguoiDung { get; set; }
}
