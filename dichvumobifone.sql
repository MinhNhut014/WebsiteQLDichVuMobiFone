USE Master
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'qldichvumobifone')
    DROP DATABASE qldichvumobifone;
GO
CREATE DATABASE qldichvumobifone;
GO

USE qldichvumobifone;

-- Bảng Người Dùng --
CREATE TABLE NguoiDung (
    IDNguoiDung INT IDENTITY(1,1) PRIMARY KEY,
    AnhDaiDien NVARCHAR(255),
    HoTen NVARCHAR(100) NOT NULL,
    CCCD NVARCHAR(15) UNIQUE,
    Email NVARCHAR(50) UNIQUE,
    SoDienThoai NVARCHAR(11) UNIQUE,
    DiaChi NVARCHAR(255),
    TenDangNhap NVARCHAR(255) UNIQUE NOT NULL,
    MatKhau NVARCHAR(1000) NOT NULL,
	quyen int default 0,
	trangthai int default 1
);

-- Bảng Dịch Vụ (Cha của tất cả các dịch vụ) --
CREATE TABLE DichVu (
    IDDichVu INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(255) NOT NULL UNIQUE,
	MoTa NVARCHAR(255),
    AnhDichVu NVARCHAR(255)
);

-- Dịch vụ Di động --
CREATE TABLE LoaiDichVuDiDong (
    IDLoaiDichVu INT IDENTITY(1,1) PRIMARY KEY,
    IDDichVu INT NOT NULL FOREIGN KEY REFERENCES DichVu(IDDichVu) ON DELETE CASCADE,
    TenLoaiDichVu NVARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE LoaiGoiDangKy (
    IDLoaiGoi INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiGoi NVARCHAR(255) NOT NULL UNIQUE,
    IDLoaiDichVu INT NOT NULL FOREIGN KEY REFERENCES LoaiDichVuDiDong(IDLoaiDichVu) ON DELETE CASCADE
);

CREATE TABLE GoiDangKy (
    IDGoiDangKy INT IDENTITY(1,1) PRIMARY KEY,
    TenGoi NVARCHAR(255) NOT NULL UNIQUE,
    GiaGoi INT DEFAULT 0,
    ThoiHan NVARCHAR(50),
	TinhNang NVARCHAR(255),
    IDLoaiGoi INT NOT NULL FOREIGN KEY REFERENCES LoaiGoiDangKy(IDLoaiGoi) ON DELETE CASCADE,
    ThongTinGoi NVARCHAR(1000),
    ThongTinChiTiet NVARCHAR(MAX)
);

-- Dịch vụ Doanh nghiệp --
CREATE TABLE NhomDichVuDoanhNghiep (
    IDNhomDichVu INT IDENTITY(1,1) PRIMARY KEY,
    IDDichVu INT NOT NULL FOREIGN KEY REFERENCES DichVu(IDDichVu) ON DELETE CASCADE,
    TenNhom NVARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE DichVuDoanhNghiep (
    IDDichVuDN INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(255) NOT NULL UNIQUE,
    IDNhomDichVu INT NOT NULL FOREIGN KEY REFERENCES NhomDichVuDoanhNghiep(IDNhomDichVu) ON DELETE CASCADE
);

CREATE TABLE GoiDichVu (
    IDGoiDichVu INT IDENTITY(1,1) PRIMARY KEY,    
    TenGoiDV NVARCHAR(255) NOT NULL,
	HinhAnh NVARCHAR(255),
    MoTa NVARCHAR(1000),
    ThongTinChiTiet NVARCHAR(MAX),
    IDDichVuDN INT NOT NULL FOREIGN KEY REFERENCES DichVuDoanhNghiep(IDDichVuDN)
);

-- Dịch vụ SIM --
CREATE TABLE LoaiSo (
    IDLoaiSo INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiSo NVARCHAR(50) NOT NULL UNIQUE
);
--Trạng Thái Của Sim
CREATE TABLE TrangThaiSim(
	IDTrangThaiSim INT IDENTITY(1,1) PRIMARY KEY,
	TenTrangThai NVARCHAR(50) NOT NULL UNIQUE
);
CREATE TABLE Sim (
    IDSim INT IDENTITY(1,1) PRIMARY KEY,
    IDDichVu INT NOT NULL FOREIGN KEY REFERENCES DichVu(IDDichVu) ON DELETE CASCADE,
    SoThueBao NVARCHAR(11) UNIQUE NOT NULL,
    IDLoaiSo INT NOT NULL FOREIGN KEY REFERENCES LoaiSo(IDLoaiSo),
    KhuVucHoaMang NVARCHAR(255),
    PhiHoaMang INT DEFAULT 0,
	IDTrangThaiSim INT NOT NULL FOREIGN KEY REFERENCES TrangThaiSim(IDTrangThaiSim),
	IDNguoiDung INT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
	SoDu DECIMAL(18,2) DEFAULT 0
);

-- Dịch vụ Khác (Giáo dục, Tài chính, Giải trí, Internet An toàn, Du lịch) --
CREATE TABLE LoaiDichVuKhac (
    IDLoaiDichVuKhac INT IDENTITY(1,1) PRIMARY KEY,
    IDDichVu INT NOT NULL FOREIGN KEY REFERENCES DichVu(IDDichVu) ON DELETE CASCADE,
    TenLoaiDichVu NVARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE SanPhamDichVuKhac (
    IDSanPham INT IDENTITY(1,1) PRIMARY KEY,
    TenSanPham NVARCHAR(255) NOT NULL UNIQUE,
	HinhAnh NVARCHAR(255),
    MoTa NVARCHAR(1000),
    ThongTinChiTiet NVARCHAR(MAX),
    IDLoaiDichVuKhac INT NOT NULL FOREIGN KEY REFERENCES LoaiDichVuKhac(IDLoaiDichVuKhac) ON DELETE CASCADE
);

CREATE TABLE GoiDangKyDichVuKhac (
    IDGoiDangKy INT IDENTITY(1,1) PRIMARY KEY,
    TenGoi NVARCHAR(255) NOT NULL UNIQUE,
    GiaGoi INT DEFAULT 0,
    ThoiHan NVARCHAR(50),
    IDSanPham INT NOT NULL FOREIGN KEY REFERENCES SanPhamDichVuKhac(IDSanPham) ON DELETE CASCADE
);
-- Bảng trung gian giữa SIM và GoiDangKy (dịch vụ Di động)
CREATE TABLE Sim_GoiDangKy (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    IDSim INT NULL FOREIGN KEY REFERENCES Sim(IDSim) ON DELETE CASCADE,
    IDGoiDangKy INT NULL FOREIGN KEY REFERENCES GoiDangKy(IDGoiDangKy),
    NgayDangKy DATETIME DEFAULT GETDATE()
);

-- Bảng trung gian giữa SIM và GoiDangKyDichVuKhac (dịch vụ khác)
CREATE TABLE Sim_GoiDangKyDichVuKhac (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    IDSim INT NULL FOREIGN KEY REFERENCES Sim(IDSim) ON DELETE CASCADE,
    IDGoiDangKy INT NULL FOREIGN KEY REFERENCES GoiDangKyDichVuKhac(IDGoiDangKy),
    NgayDangKy DATETIME DEFAULT GETDATE()
);
CREATE TABLE LienHe (
    IDLienHe INT IDENTITY(1,1) PRIMARY KEY,
	IDNguoiDung INT NOT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
    HoTen NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(15) NOT NULL,
    NoiDung NVARCHAR(MAX),
    NgayGui DATETIME DEFAULT GETDATE(),
    TrangThai BIT DEFAULT 0 -- 0: chưa xử lý, 1: đã xử lý
);

-- Bảng Trạng Thái Đơn Hàng
CREATE TABLE TrangThaiDonHang (
    IDTrangThai INT IDENTITY(1,1) PRIMARY KEY,
    TenTrangThai NVARCHAR(50) NOT NULL UNIQUE -- (Chờ xử lý, Đang giao, Hoàn thành, Hủy...)
);
--Bảng Phương Thức Vận Chuyển
CREATE TABLE PhuongThucVanChuyen (
    IDPhuongThucVC INT IDENTITY(1,1) PRIMARY KEY,
    TenVanChuyen NVARCHAR(255) NOT NULL UNIQUE,
    MoTa NVARCHAR(1000),
    GiaVanChuyen INT DEFAULT 0 CHECK (GiaVanChuyen >= 0)
);
CREATE TABLE TrangThaiThanhToan (
    IDTrangThaiThanhToan INT IDENTITY(1,1) PRIMARY KEY,
    TenTrangThai NVARCHAR(100) NOT NULL -- VD: "Chưa thanh toán", "Đã thanh toán", "Đang xử lý"
);
CREATE TABLE GiaoDichNapTien(
	IDGiaoDich INT IDENTITY(1,1) PRIMARY KEY,
	MaGiaoDichNapTien NVARCHAR(50),
	IDSim INT NULL FOREIGN KEY REFERENCES Sim(IDSim) ON DELETE CASCADE,
	IDNguoiDung INT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
	SoTienNap DECIMAL(18,2),
	NgayNap DATETIME DEFAULT GETDATE(),
	GhiChu NVARCHAR(255),
	IDTrangThaiThanhToan INT NULL FOREIGN KEY REFERENCES TrangThaiThanhToan(IDTrangThaiThanhToan),
	PhuongThucNap NVARCHAR(100)
);
-- Bảng Hóa Đơn Dịch Vụ (Dịch vụ Di động + Dịch vụ Khác)
CREATE TABLE HoaDonDichVu (
    IDHoaDonDV INT IDENTITY(1,1) PRIMARY KEY,
	MaHoaDonDichVu NVARCHAR(50) UNIQUE NOT NULL,
    IDNguoiDung INT NOT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
    NgayDatHang DATETIME DEFAULT GETDATE(),
    TongTien INT DEFAULT 0,
    IDTrangThai INT NOT NULL FOREIGN KEY REFERENCES TrangThaiDonHang(IDTrangThai),
	TenKhachHang NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(11) NOT NULL,
    Email NVARCHAR(100),
	IDTrangThaiThanhToan INT NOT NULL FOREIGN KEY REFERENCES TrangThaiThanhToan(IDTrangThaiThanhToan),
	PhuongThucThanhToan NVARCHAR(100)
);

-- Chi Tiết Hóa Đơn Dịch Vụ
CREATE TABLE CTHoaDonDichVu (
    IDCTHoaDonDV INT IDENTITY(1,1) PRIMARY KEY,
    IDHoaDonDV INT NOT NULL FOREIGN KEY REFERENCES HoaDonDichVu(IDHoaDonDV) ON DELETE CASCADE,
    IDGoiDangKy INT NULL FOREIGN KEY REFERENCES GoiDangKy(IDGoiDangKy), -- Liên kết gói dịch vụ
    IDGoiDangKyDVK INT NULL FOREIGN KEY REFERENCES GoiDangKyDichVuKhac(IDGoiDangKy),
    DonGia INT DEFAULT 0,
    SoLuong SMALLINT DEFAULT 1,
    ThanhTien INT
);

-- Bảng Hóa Đơn SIM
CREATE TABLE HoaDonSim (
    IDHoaDonSim INT IDENTITY(1,1) PRIMARY KEY,
	MaHoaDonSim NVARCHAR(50) UNIQUE NOT NULL,
    IDNguoiDung INT NOT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
    NgayDatHang DATETIME DEFAULT GETDATE(),
    TongTien INT DEFAULT 0,
    IDTrangThai INT NOT NULL FOREIGN KEY REFERENCES TrangThaiDonHang(IDTrangThai),
    TenKhachHang NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(11) NOT NULL,
    Email NVARCHAR(100),
    DiaDiemNhan NVARCHAR(255) NOT NULL,
    PhuongThucThanhToan NVARCHAR(50) NOT NULL,
	IDTrangThaiThanhToan INT NOT NULL FOREIGN KEY REFERENCES TrangThaiThanhToan(IDTrangThaiThanhToan),
    IDPhuongThucVC INT NOT NULL FOREIGN KEY REFERENCES PhuongThucVanChuyen(IDPhuongThucVC) -- Thay thế cột cũ
);


-- Chi Tiết Hóa Đơn SIM
CREATE TABLE CTHoaDonSim (
    IDCTHoaDonSim INT IDENTITY(1,1) PRIMARY KEY,
    IDHoaDonSim INT NOT NULL FOREIGN KEY REFERENCES HoaDonSim(IDHoaDonSim) ON DELETE CASCADE,
    IDSim INT NOT NULL FOREIGN KEY REFERENCES Sim(IDSim),
    IDGoiDangKy INT NOT NULL FOREIGN KEY REFERENCES GoiDangKy(IDGoiDangKy), -- Gói đi kèm
    DonGia INT DEFAULT 0,
    SoLuong SMALLINT DEFAULT 1,
    ThanhTien INT 
);

-- Bảng Hóa Đơn Dịch Vụ Doanh Nghiệp
CREATE TABLE HoaDonDoanhNghiep (
    IDHoaDonDN INT IDENTITY(1,1) PRIMARY KEY,
	MaHoaDonDoanhNghiep NVARCHAR(50) UNIQUE NOT NULL,
    IDNguoiDung INT NOT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
    NgayDatHang DATETIME DEFAULT GETDATE(),
	IDTrangThai INT NOT NULL FOREIGN KEY REFERENCES TrangThaiDonHang(IDTrangThai),
    TenCongTy NVARCHAR(255) NOT NULL,
    SoDienThoaiCongTy NVARCHAR(11) NOT NULL,
    EmailCongTy NVARCHAR(100),
    DiaChiCongTy NVARCHAR(255) NOT NULL
    
);

-- Chi Tiết Hóa Đơn Dịch Vụ Doanh Nghiệp
CREATE TABLE CTHoaDonDoanhNghiep (
    IDCTHoaDonDN INT IDENTITY(1,1) PRIMARY KEY,
    IDHoaDonDN INT NOT NULL FOREIGN KEY REFERENCES HoaDonDoanhNghiep(IDHoaDonDN) ON DELETE CASCADE,
    IDGoiDichVu INT NOT NULL FOREIGN KEY REFERENCES GoiDichVu(IDGoiDichVu)
);

--Bổ sung thêm bảng chức năng tin tức và bình luận bài viết 

-- Bảng chủ đề bài viết
CREATE TABLE ChuDe (
    IDChuDe INT IDENTITY(1,1) PRIMARY KEY,
    TenChuDe NVARCHAR(100) NOT NULL
);
-- Bảng quản lý bài viết
CREATE TABLE TinTuc(
    IdTinTuc INT PRIMARY KEY IDENTITY(1,1),
    TieuDe NVARCHAR(255) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL, 
    AnhDaiDien NVARCHAR(255),        
    NgayDang DATETIME DEFAULT GETDATE(),
	LuotXem INT Default 0,
    IdTheLoai INT,
    FOREIGN KEY (IdTheLoai) REFERENCES ChuDe(IdChuDe)
);
-- Bảng bình luận bài viết
CREATE TABLE BinhLuanBaiViet (
    IDBinhLuan INT IDENTITY(1,1) PRIMARY KEY,
    IdTinTuc INT NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    NoiDung NVARCHAR(1000) NOT NULL,
    NgayBinhLuan DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdTinTuc) REFERENCES TinTuc(IdTinTuc) ON DELETE CASCADE,
    NguoiDungID INT FOREIGN KEY (NguoiDungID) REFERENCES NguoiDung(IDNguoiDung)
);

---Kiểm tra dữ liệu trong các bảng---
select *from DichVu;
select *from GoiDichVu;
select * from PhuongThucVanChuyen;
select *from TrangThaiSim;
select *from HoaDonSim;
select *from NguoiDung;
DELETE FROM NguoiDung;

---Dữ liệu bảng giao hàng tiết kiệm
INSERT INTO PhuongThucVanChuyen (TenVanChuyen, MoTa, GiaVanChuyen)
VALUES 
('Giao hàng tiết kiệm', N'Giao hàng trong vòng 5-7 ngày với chi phí thấp', 20000),
('Giao hàng J&T', N'Dịch vụ giao hàng nhanh của J&T Express', 40000);
-- Thêm các trạng thái đơn hàng vào bảng TrangThaiDonHang
INSERT INTO TrangThaiDonHang (TenTrangThai) 
VALUES 
('Chờ xử lý'),  -- Trạng thái khi đơn hàng vừa được tạo
('Đang giao'),   -- Trạng thái khi đơn hàng đang được vận chuyển
('Hoàn thành'),  -- Trạng thái khi đơn hàng đã hoàn tất
('Hủy');         -- Trạng thái khi đơn hàng bị hủy
---Dữ liệu dịch vụ của mobifone
INSERT INTO DichVu (TenDichVu, MoTa, AnhDichVu)  
VALUES  
(N'Dịch vụ di động', N'Hỗ trợ đăng ký các gói cước thoại, SMS và gói data tốc độ cao, giúp khách hàng sử dụng dịch vụ di động tiện lợi với chi phí tối ưu.', 'dichvu_di_dong.png'),  
(N'SIM', N'Cung cấp SIM trả trước và SIM trả sau với nhiều ưu đãi hấp dẫn. Khách hàng có thể chọn số đẹp, đăng ký nhanh chóng và kích hoạt dễ dàng ngay trên hệ thống.', 'dichvu_sim.png'),  
(N'Dịch vụ doanh nghiệp', N'Cung cấp các giải pháp viễn thông và công nghệ thông tin chuyên biệt cho doanh nghiệp, bao gồm tổng đài, dịch vụ số và hạ tầng mạng.', 'dichvu_doanh_nghiep.png'),  
(N'Dịch vụ khác', N'Hệ sinh thái đa dạng gồm dịch vụ giáo dục, giải trí, truyền hình và du lịch, mang đến nhiều tiện ích giúp cuộc sống thêm phong phú.', 'dichvu_khac.png');

---- Thông tin dịch vụ sim của mobifone
---Dữ liệu trong các bảng dịch vụ mobifone
INSERT INTO TrangThaiSim (TenTrangThai)  
--1. Dữ liệu bảng trạng thái sim
VALUES  
    (N'Chưa kích hoạt'),  
	(N'Đang giao dịch'), 
    (N'Đang hoạt động'),  
    (N'Bị khóa một chiều'),  
    (N'Bị khóa hai chiều'),  
    (N'Đã hủy');  
INSERT INTO TrangThaiThanhToan (TenTrangThai)
VALUES
(N'Chưa thanh toán'),
(N'Đã thanh toán'),
(N'Đang xử lý'),
(N'Thanh toán thất bại'),
(N'Hoàn tiền');