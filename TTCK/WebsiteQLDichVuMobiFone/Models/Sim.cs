using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("Sim")]
[Index("SoThueBao", Name = "UQ__Sim__F0752C9774705E59", IsUnique = true)]
public partial class Sim
{
    [Key]
    [Column("IDSim")]
    public int Idsim { get; set; }

    [Column("IDDichVu")]
    public int IddichVu { get; set; }

    [StringLength(11)]
    public string SoThueBao { get; set; } = null!;

    [Column("IDLoaiSo")]
    public int IdloaiSo { get; set; }

    [StringLength(255)]
    public string? KhuVucHoaMang { get; set; }

    public int? PhiHoaMang { get; set; }

    [Column("IDTrangThaiSim")]
    public int IdtrangThaiSim { get; set; }

    [Column("IDNguoiDung")]
    public int? IdnguoiDung { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? SoDu { get; set; }

    [InverseProperty("IdsimNavigation")]
    public virtual ICollection<CthoaDonSim> CthoaDonSims { get; set; } = new List<CthoaDonSim>();

    [InverseProperty("IdsimNavigation")]
    public virtual ICollection<GiaoDichNapTien> GiaoDichNapTiens { get; set; } = new List<GiaoDichNapTien>();

    [ForeignKey("IddichVu")]
    [InverseProperty("Sims")]
    public virtual DichVu? IddichVuNavigation { get; set; } = null!;

    [ForeignKey("IdloaiSo")]
    [InverseProperty("Sims")]
    public virtual LoaiSo? IdloaiSoNavigation { get; set; } = null!;

    [ForeignKey("IdnguoiDung")]
    [InverseProperty("Sims")]
    public virtual NguoiDung? IdnguoiDungNavigation { get; set; }

    [ForeignKey("IdtrangThaiSim")]
    [InverseProperty("Sims")]
    public virtual TrangThaiSim? IdtrangThaiSimNavigation { get; set; } = null!;

    [InverseProperty("IdsimNavigation")]
    public virtual ICollection<SimGoiDangKy> SimGoiDangKies { get; set; } = new List<SimGoiDangKy>();

    [InverseProperty("IdsimNavigation")]
    public virtual ICollection<SimGoiDangKyDichVuKhac> SimGoiDangKyDichVuKhacs { get; set; } = new List<SimGoiDangKyDichVuKhac>();
}
