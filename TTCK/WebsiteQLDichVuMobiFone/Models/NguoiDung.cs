﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace WebsiteQLDichVuMobiFone.Models;

[Table("NguoiDung")]
[Index("SoDienThoai", Name = "UQ__NguoiDun__0389B7BD05D90289", IsUnique = true)]
[Index("TenDangNhap", Name = "UQ__NguoiDun__55F68FC0FCB58BFA", IsUnique = true)]
[Index("Cccd", Name = "UQ__NguoiDun__A955A0AA364AE806", IsUnique = true)]
[Index("Email", Name = "UQ__NguoiDun__A9D10534904170A5", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    [Column("IDNguoiDung")]
    public int IdnguoiDung { get; set; }

    [StringLength(255)]
    public string? AnhDaiDien { get; set; }

    [StringLength(100)]
    public string HoTen { get; set; } = null!;

    [Column("CCCD")]
    [StringLength(15)]
    public string? Cccd { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(11)]
    public string? SoDienThoai { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    [StringLength(255)]
    public string TenDangNhap { get; set; } = null!;

    [StringLength(1000)]
    public string MatKhau { get; set; } = null!;

    [Column("quyen")]
    public int? Quyen { get; set; }

    [Column("trangthai")]
    public int? Trangthai { get; set; }

    [InverseProperty("NguoiDung")]
    public virtual ICollection<BinhLuanBaiViet> BinhLuanBaiViets { get; set; } = new List<BinhLuanBaiViet>();

    [InverseProperty("IdnguoiDungNavigation")]
    public virtual ICollection<HoaDonDichVu> HoaDonDichVus { get; set; } = new List<HoaDonDichVu>();

    [InverseProperty("IdnguoiDungNavigation")]
    public virtual ICollection<HoaDonDoanhNghiep> HoaDonDoanhNghieps { get; set; } = new List<HoaDonDoanhNghiep>();

    [InverseProperty("IdnguoiDungNavigation")]
    public virtual ICollection<HoaDonSim> HoaDonSims { get; set; } = new List<HoaDonSim>();
}
