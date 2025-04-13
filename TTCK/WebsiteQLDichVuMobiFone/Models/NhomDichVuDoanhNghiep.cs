using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("NhomDichVuDoanhNghiep")]
[Index("TenNhom", Name = "UQ__NhomDich__2B432D0D0923C7C9", IsUnique = true)]
public partial class NhomDichVuDoanhNghiep
{
    [Key]
    [Column("IDNhomDichVu")]
    public int IdnhomDichVu { get; set; }

    [Column("IDDichVu")]
    public int IddichVu { get; set; }

    [StringLength(255)]
    public string TenNhom { get; set; } = null!;

    [InverseProperty("IdnhomDichVuNavigation")]
    public virtual ICollection<DichVuDoanhNghiep> DichVuDoanhNghieps { get; set; } = new List<DichVuDoanhNghiep>();

    [ForeignKey("IddichVu")]
    [InverseProperty("NhomDichVuDoanhNghieps")]
    public virtual DichVu? IddichVuNavigation { get; set; } = null!;
}
