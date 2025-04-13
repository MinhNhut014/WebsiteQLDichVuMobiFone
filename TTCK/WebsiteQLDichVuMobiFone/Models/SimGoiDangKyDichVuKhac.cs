using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("Sim_GoiDangKyDichVuKhac")]
public partial class SimGoiDangKyDichVuKhac
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("IDSim")]
    public int? Idsim { get; set; }

    [Column("IDGoiDangKy")]
    public int? IdgoiDangKy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDangKy { get; set; }

    [ForeignKey("IdgoiDangKy")]
    [InverseProperty("SimGoiDangKyDichVuKhacs")]
    public virtual GoiDangKyDichVuKhac? IdgoiDangKyNavigation { get; set; }

    [ForeignKey("Idsim")]
    [InverseProperty("SimGoiDangKyDichVuKhacs")]
    public virtual Sim? IdsimNavigation { get; set; }
}
