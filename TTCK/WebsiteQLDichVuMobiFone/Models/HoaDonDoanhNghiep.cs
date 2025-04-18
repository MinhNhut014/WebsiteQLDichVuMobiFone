﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("HoaDonDoanhNghiep")]
[Index("MaHoaDonDoanhNghiep", Name = "UQ__HoaDonDo__0762DD6B9611208E", IsUnique = true)]
public partial class HoaDonDoanhNghiep
{
    [Key]
    [Column("IDHoaDonDN")]
    public int IdhoaDonDn { get; set; }

    [StringLength(50)]
    [BindNever]
    public string MaHoaDonDoanhNghiep { get; set; } = string.Empty;

    [Column("IDNguoiDung")]
    public int IdnguoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDatHang { get; set; }

    [Column("IDTrangThai")]
    public int IdtrangThai { get; set; }

    [StringLength(255)]
    public string TenCongTy { get; set; } = null!;

    [StringLength(11)]
    public string SoDienThoaiCongTy { get; set; } = null!;

    [StringLength(100)]
    public string? EmailCongTy { get; set; }

    [StringLength(255)]
    public string DiaChiCongTy { get; set; } = null!;

    [InverseProperty("IdhoaDonDnNavigation")]
    public virtual ICollection<CthoaDonDoanhNghiep> CthoaDonDoanhNghieps { get; set; } = new List<CthoaDonDoanhNghiep>();

    [ForeignKey("IdnguoiDung")]
    [InverseProperty("HoaDonDoanhNghieps")]
    public virtual NguoiDung? IdnguoiDungNavigation { get; set; } = null!;

    [ForeignKey("IdtrangThai")]
    [InverseProperty("HoaDonDoanhNghieps")]
    public virtual TrangThaiDonHang? IdtrangThaiNavigation { get; set; } = null!;
}
