using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("LienHe")]
public partial class LienHe
{
    [Key]
    [Column("IDLienHe")]
    public int IdlienHe { get; set; }

    [Column("IDNguoiDung")]
    public int IdnguoiDung { get; set; }

    [StringLength(255)]
    public string HoTen { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(15)]
    public string SoDienThoai { get; set; } = null!;

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGui { get; set; }

    public bool? TrangThai { get; set; }

    [ForeignKey("IdnguoiDung")]
    [InverseProperty("LienHes")]
    public virtual NguoiDung? IdnguoiDungNavigation { get; set; } = null!;
}
