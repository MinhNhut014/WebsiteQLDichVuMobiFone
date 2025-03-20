using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("DichVuDoanhNghiep")]
[Index("TenDichVu", Name = "UQ__DichVuDo__A77D06897B324589", IsUnique = true)]
public partial class DichVuDoanhNghiep
{
    [Key]
    [Column("IDDichVuDN")]
    public int IddichVuDn { get; set; }

    [StringLength(255)]
    public string TenDichVu { get; set; } = null!;

    [Column("IDNhomDichVu")]
    public int IdnhomDichVu { get; set; }

    [InverseProperty("IddichVuDnNavigation")]
    public virtual ICollection<GoiDichVu> GoiDichVus { get; set; } = new List<GoiDichVu>();

    [ForeignKey("IdnhomDichVu")]
    [InverseProperty("DichVuDoanhNghieps")]
    public virtual NhomDichVuDoanhNghiep IdnhomDichVuNavigation { get; set; } = null!;
}
