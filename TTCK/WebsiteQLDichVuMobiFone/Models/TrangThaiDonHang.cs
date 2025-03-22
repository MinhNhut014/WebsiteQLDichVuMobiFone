using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("TrangThaiDonHang")]
[Index("TenTrangThai", Name = "UQ__TrangTha__9489EF66D08FD229", IsUnique = true)]
public partial class TrangThaiDonHang
{
    [Key]
    [Column("IDTrangThai")]
    public int IdtrangThai { get; set; }

    [StringLength(50)]
    public string TenTrangThai { get; set; } = null!;

    [InverseProperty("IdtrangThaiNavigation")]
    public virtual ICollection<HoaDonDichVu> HoaDonDichVus { get; set; } = new List<HoaDonDichVu>();

    [InverseProperty("IdtrangThaiNavigation")]
    public virtual ICollection<HoaDonDoanhNghiep> HoaDonDoanhNghieps { get; set; } = new List<HoaDonDoanhNghiep>();

    [InverseProperty("IdtrangThaiNavigation")]
    public virtual ICollection<HoaDonSim> HoaDonSims { get; set; } = new List<HoaDonSim>();
}
