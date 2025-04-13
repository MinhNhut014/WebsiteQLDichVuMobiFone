using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("Sim_GoiDangKy")]
public partial class SimGoiDangKy
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
    [InverseProperty("SimGoiDangKies")]
    public virtual GoiDangKy? IdgoiDangKyNavigation { get; set; }

    [ForeignKey("Idsim")]
    [InverseProperty("SimGoiDangKies")]
    public virtual Sim? IdsimNavigation { get; set; }
}
