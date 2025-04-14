using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("GiaoDichNapTien")]
public partial class GiaoDichNapTien
{
    [Key]
    [Column("IDGiaoDich")]
    public int IdgiaoDich { get; set; }

    [StringLength(50)]
    public string? MaGiaoDichNapTien { get; set; }

    [Column("IDSim")]
    public int? Idsim { get; set; }

    [Column("IDNguoiDung")]
    public int? IdnguoiDung { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SoTienNap { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayNap { get; set; }

    [StringLength(255)]
    public string? GhiChu { get; set; }

    [Column("IDTrangThaiThanhToan")]
    public int? IdtrangThaiThanhToan { get; set; }

    [StringLength(100)]
    public string? PhuongThucNap { get; set; }

    [ForeignKey("IdnguoiDung")]
    [InverseProperty("GiaoDichNapTiens")]
    public virtual NguoiDung? IdnguoiDungNavigation { get; set; }

    [ForeignKey("Idsim")]
    [InverseProperty("GiaoDichNapTiens")]
    public virtual Sim? IdsimNavigation { get; set; }

    [ForeignKey("IdtrangThaiThanhToan")]
    [InverseProperty("GiaoDichNapTiens")]
    public virtual TrangThaiThanhToan? IdtrangThaiThanhToanNavigation { get; set; }
}
