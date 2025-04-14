using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("TrangThaiThanhToan")]
public partial class TrangThaiThanhToan
{
    [Key]
    [Column("IDTrangThaiThanhToan")]
    public int IdtrangThaiThanhToan { get; set; }

    [StringLength(100)]
    public string TenTrangThai { get; set; } = null!;

    [InverseProperty("IdtrangThaiThanhToanNavigation")]
    public virtual ICollection<GiaoDichNapTien> GiaoDichNapTiens { get; set; } = new List<GiaoDichNapTien>();

    [InverseProperty("IdtrangThaiThanhToanNavigation")]
    public virtual ICollection<HoaDonDichVu> HoaDonDichVus { get; set; } = new List<HoaDonDichVu>();

    [InverseProperty("IdtrangThaiThanhToanNavigation")]
    public virtual ICollection<HoaDonSim> HoaDonSims { get; set; } = new List<HoaDonSim>();
}
