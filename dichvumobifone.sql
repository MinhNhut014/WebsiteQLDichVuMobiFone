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
	GoiDangKyDiKem INT NULL FOREIGN KEY REFERENCES GoiDangKy(IDGoiDangKy)
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
-- Bảng Hóa Đơn Dịch Vụ (Dịch vụ Di động + Dịch vụ Khác)
CREATE TABLE HoaDonDichVu (
    IDHoaDonDV INT IDENTITY(1,1) PRIMARY KEY,
    IDNguoiDung INT NOT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
    NgayDatHang DATETIME DEFAULT GETDATE(),
    TongTien INT DEFAULT 0,
    IDTrangThai INT NOT NULL FOREIGN KEY REFERENCES TrangThaiDonHang(IDTrangThai),
	TenKhachHang NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(11) NOT NULL,
    Email NVARCHAR(100)
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
    IDNguoiDung INT NOT NULL FOREIGN KEY REFERENCES NguoiDung(IDNguoiDung),
    NgayDatHang DATETIME DEFAULT GETDATE(),
    TongTien INT DEFAULT 0,
    IDTrangThai INT NOT NULL FOREIGN KEY REFERENCES TrangThaiDonHang(IDTrangThai),
    TenKhachHang NVARCHAR(255) NOT NULL,
    SoDienThoai NVARCHAR(11) NOT NULL,
    Email NVARCHAR(100),
    DiaDiemNhan NVARCHAR(255) NOT NULL,
    PhuongThucThanhToan NVARCHAR(50) NOT NULL,
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
--2. Dữ liệu bảng loại sim
INSERT INTO LoaiSo (TenLoaiSo)  
VALUES  
    (N'Trả Trước'),  
    (N'Trả Sau');
--3. Dữ liệu sim
INSERT INTO Sim (IDDichVu, SoThueBao, IDLoaiSo, KhuVucHoaMang, PhiHoaMang, IDTrangThaiSim, GoiDangKyDiKem)
VALUES 
(2, '0783212416', 1, 'Toàn quốc', 50000, 1, NULL),
(2, '0783329026', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783329050', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783213630', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783213710', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783213728', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783214309', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783214370', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783214408', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783218610', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783225810', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783329546', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783329820', 1, 'Toàn quốc', 50000, 1, NULL),  
(2, '0783214808', 1, 'Toàn quốc', 50000, 1, NULL)


----Thông tin dịch vụ di đông của mobifone
---1. Loại dịch vụ di động
INSERT INTO LoaiDichVuDiDong (IDDichVu, TenLoaiDichVu)  
VALUES  
(1, 'Gói cước'),  
(1, 'Gói data');
---2. loại gói đăng ký
-- Thêm dữ liệu vào bảng LoaiGoiDangKy (Loại gói cước)
INSERT INTO LoaiGoiDangKy (TenLoaiGoi, IDLoaiDichVu)  
VALUES  
('Thoại quốc tế', 1),  
('Chuyển vùng quốc tế', 1),  
('MOBISAFE', 1),  
('Tích điểm', 1),  
('Siêu data', 1),  
('Quốc tế linh hoạt', 1);  

-- Thêm dữ liệu vào bảng LoaiGoiDangKy (Loại gói Data)
INSERT INTO LoaiGoiDangKy (TenLoaiGoi, IDLoaiDichVu)  
VALUES  
('Miễn phí MXH', 2),  
('Dài kỳ', 2),  
('Data bổ sung', 2),  
('Tháng', 2),  
('Data thường', 2),  
('Ngày', 2);  
---3. Gói đăng ký dịch vụ di động
INSERT INTO GoiDangKy (TenGoi, GiaGoi, ThoiHan, TinhNang, IDLoaiGoi, ThongTinGoi, ThongTinChiTiet)
VALUES 
	---Gói Data
    -- Gói Data - Miễn phí MXH
    (N'MXH80', 80000, N'30 Ngày', N'Data: 30GB', 7, N'80.000 đ / 30 Ngày - 30GB', N'Chi tiết gói MXH80...'),
    (N'12MXH80', 960000, N'360 Ngày', N'Data: 360GB', 7, N'960.000 đ / 360 Ngày - 360GB', N'Chi tiết gói 12MXH80...'),
    (N'6MXH80', 480000, N'180 Ngày', N'Data: 180GB', 7, N'480.000 đ / 180 Ngày - 180GB', N'Chi tiết gói 6MXH80...'),

    (N'MXH90', 90000, N'30 Ngày', N'Data: 30GB', 7, N'90.000 đ / 30 Ngày - 30GB', N'Chi tiết gói MXH90...'),
    (N'12MXH90', 1080000, N'360 Ngày', N'Data: 360GB', 7, N'1.080.000 đ / 360 Ngày - 360GB', N'Chi tiết gói 12MXH90...'),
    (N'6MXH90', 540000, N'180 Ngày', N'Data: 180GB', 7, N'540.000 đ / 180 Ngày - 180GB', N'Chi tiết gói 6MXH90...'),

    (N'MXH100', 100000, N'30 Ngày', N'Data: 30GB', 7, N'100.000 đ / 30 Ngày - 30GB', N'Chi tiết gói MXH100...'),
    (N'6MXH100', 600000, N'180 Ngày', N'Data: 180GB', 7, N'600.000 đ / 180 Ngày - 180GB', N'Chi tiết gói 6MXH100...'),

    -- Gói Data - Dài Kỳ (IDLoaiGoi = 8)
    (N'12MFV250', 3000000, N'360 Ngày', N'Thoại nội mạng: 2500 phút. Thoại ngoại mạng: 250 phút. Data: 250 GB', 8, N'3.000.000 đ / 360 Ngày - 2500 phút thoại', N'Chi tiết gói 12MFV250...'),
    (N'6MFV250', 1500000, N'180 Ngày', N'Thoại nội mạng: 2000 phút. Thoại ngoại mạng: 250 phút. Data: 250 GB', 8, N'1.500.000 đ / 180 Ngày - 2000 phút thoại', N'Chi tiết gói 6MFV250...'),
    (N'24MFV250', 6000000, N'720 Ngày', N'Thoại nội mạng: 3000 phút. Thoại ngoại mạng: 250 phút. Data: 250 GB', 8, N'6.000.000 đ / 720 Ngày - 3000 phút thoại', N'Chi tiết gói 24MFV250...'),
    (N'5GV24', 4800000, N'720 Ngày', N'Thoại liên mạng: 250 phút...', 8, N'4.800.000 đ / 720 Ngày - 250 phút thoại', N'Chi tiết gói 5GV24...'),
    (N'5GC24', 4800000, N'720 Ngày', N'Thoại liên mạng: 250 phút...', 8, N'4.800.000 đ / 720 Ngày - 250 phút thoại', N'Chi tiết gói 5GC24...'),
    (N'5GV12', 2400000, N'360 Ngày', N'Thoại liên mạng: 250 phút...', 8, N'2.400.000 đ / 360 Ngày - 250 phút thoại', N'Chi tiết gói 5GV12...'),
    (N'5GC12', 2400000, N'360 Ngày', N'Thoại liên mạng: 250 phút...', 8, N'2.400.000 đ / 360 Ngày - 250 phút thoại', N'Chi tiết gói 5GC12...'),
    (N'5GLQ12', 2400000, N'360 Ngày', N'Thoại liên mạng: 250 phút...', 8, N'2.400.000 đ / 360 Ngày - 250 phút thoại', N'Chi tiết gói 5GLQ12...'),

    -- Gói Data Bổ Sung (IDLoaiGoi = 9)
    (N'2H', 2000, N'2 Giờ', N'Data: 0.49GB', 9, N'2.000 đ / 2 Giờ - 0.49GB', N'Chi tiết gói 2H...'),
    (N'3H', 9000, N'3 Giờ', N'Data: 3GB', 9, N'9.000 đ / 3 Giờ - 3GB', N'Chi tiết gói 3H...'),
    (N'1H', 5000, N'1 Giờ', N'Data: 1GB', 9, N'5.000 đ / 1 Giờ - 1GB', N'Chi tiết gói 1H...'),

	---Gói Cước
	--Gói cước Thoại Quốc Tế
    (N'QTTK15', 15000, N'1 Tháng', N'', 1, N'15.000 đ / 1 Tháng', N'Chi tiết gói QTTK15...'),

    (N'TQT9', 9000, N'1 Ngày', N'Thoại quốc tế: 6 phút', 1, N'9.000 đ / 1 Ngày - 6 phút', N'Chi tiết gói TQT9...'),
    (N'TQT19', 19000, N'1 Ngày', N'Thoại quốc tế: 10 phút', 1, N'19.000 đ / 1 Ngày - 10 phút', N'Chi tiết gói TQT19...'),
    (N'TQT49', 49000, N'7 Ngày', N'Thoại quốc tế: 40 phút', 1, N'49.000 đ / 7 Ngày - 40 phút', N'Chi tiết gói TQT49...'),
    
    (N'TQT99', 99000, N'15 Ngày', N'Thoại quốc tế: 100 phút', 1, N'99.000 đ / 15 Ngày - 100 phút', N'Chi tiết gói TQT99...'),
    (N'TQT199', 199000, N'30 Ngày', N'Thoại quốc tế: 250 phút', 1, N'199.000 đ / 30 Ngày - 250 phút', N'Chi tiết gói TQT199...'),
    (N'TQT299', 299000, N'30 Ngày', N'Thoại quốc tế: 380 phút', 1, N'299.000 đ / 30 Ngày - 380 phút', N'Chi tiết gói TQT299...'),

	--Gói cước chuyển vùng quốc tế
	(N'GC', 500000, N'10 Ngày', N'Data: 5.00 GB', 2, N'500.000 đ / 10 Ngày - 5GB', N'Chi tiết gói GC...'),
    (N'GHK', 250000, N'15 Ngày', N'Data: 20.00 GB', 2, N'250.000 đ / 15 Ngày - 20GB', N'Chi tiết gói GHK...'),
    (N'GJ', 500000, N'10 Ngày', N'Data: 5.00 GB', 2, N'500.000 đ / 10 Ngày - 5GB', N'Chi tiết gói GJ...'),
    
    (N'RB2', 200000, N'30 Ngày', N'Data: 2.00 GB', 2, N'200.000 đ / 30 Ngày - 2GB', N'Chi tiết gói RB2...'),
    (N'GUS', 350000, N'15 Ngày', N'Data: 25.00 GB', 2, N'350.000 đ / 15 Ngày - 25GB', N'Chi tiết gói GUS...'),
    (N'GMA', 250000, N'15 Ngày', N'Data: 19.77 GB', 2, N'250.000 đ / 15 Ngày - 19.77GB', N'Chi tiết gói GMA...'),

    (N'RB3', 450000, N'30 Ngày', N'Data: 2.00 GB', 2, N'450.000 đ / 30 Ngày - 2GB', N'Chi tiết gói RB3...'),
    (N'CVQT', 0, N'0', N'', 2, N'0 đ / 0', N'Chi tiết gói CVQT...'),

	--Gói cước mobisafe
	(N'AT5', 25000, N'1 Tháng', N'', 3, N'25.000 đ / 1 Tháng', N'Chi tiết gói AT5...'),
    (N'AT10', 30000, N'1 Tháng', N'', 3, N'30.000 đ / 1 Tháng', N'Chi tiết gói AT10...'),
    (N'AT25', 40000, N'1 Tháng', N'', 3, N'40.000 đ / 1 Tháng', N'Chi tiết gói AT25...'),

    (N'6AT5', 150000, N'6 Tháng', N'', 3, N'150.000 đ / 6 Tháng', N'Chi tiết gói 6AT5...'),
    (N'6AT10', 180000, N'6 Tháng', N'', 3, N'180.000 đ / 6 Tháng', N'Chi tiết gói 6AT10...'),
    (N'6AT25', 240000, N'6 Tháng', N'', 3, N'240.000 đ / 6 Tháng', N'Chi tiết gói 6AT25...'),

    (N'12AT5', 300000, N'12 Tháng', N'', 3, N'300.000 đ / 12 Tháng', N'Chi tiết gói 12AT5...'),
    (N'12AT10', 360000, N'12 Tháng', N'', 3, N'360.000 đ / 12 Tháng', N'Chi tiết gói 12AT10...');

---===Thông tin dịch vụ doanh nghiệp của mobifone===---
---1. Nhóm dịch vụ doanh nghiệp của mobifone
INSERT INTO NhomDichVuDoanhNghiep (IDDichVu, TenNhom)
VALUES 
    (3, N'Viễn thông'),
    (3, N'Công nghệ thông tin');

---2.Loại dịch vụ doanh nghiệp của mobifone
INSERT INTO DichVuDoanhNghiep (TenDichVu, IDNhomDichVu)
VALUES 
	--Loại dịch vụ của nhóm dịch vụ viễn thông 
    (N'Giải pháp cho doanh nghiệp', 1),
    (N'Gói cước', 1),
	(N'Ưu đãi M-Business', 1),

	--Loại dịch vụ của nhóm dịch vụ Công nghệ thông tin
	(N'Doanh Nghiệp Số', 2),
    (N'Chính Phủ Số', 2),
	(N'Quảng Cáo Số', 2);

---3. Gói dịch vụ doanh nghiệp
INSERT INTO GoiDichVu (TenGoiDV, HinhAnh, MoTa, ThongTinChiTiet, IDDichVuDN)
VALUES 
    ('Voice Brandname', 'voice_brandname.png', 
     'Voice Brandname là giải pháp cho phép gán tên thương hiệu của doanh nghiệp trên cuộc gọi từ doanh nghiệp đến khách hàng là thuê bao MobiFone.', 
     'Voice Brandname giúp doanh nghiệp hiển thị tên thương hiệu khi gọi đến khách hàng.',1),

    ('mBiz360', 'mbiz360.png', 
     'Gói dịch vụ giúp khách hàng Doanh nghiệp tiết kiệm chi phí thuận tiện trong việc Đăng ký sử dụng dịch vụ thông qua hình thức đăng ký SMS hoặc App Mobile', 
     'mBiz360 giúp tiết kiệm chi phí và thuận tiện trong việc đăng ký dịch vụ thông qua SMS hoặc App Mobile.',1),

    ('MobiData Sponsor', 'MobiData_Sponsor.png', 
     'Tổng đài ảo giúp doanh nghiệp linh hoạt trong liên lạc', 
     'Cloud PBX là tổng đài điện thoại sử dụng công nghệ điện toán đám mây, giúp doanh nghiệp dễ dàng quản lý cuộc gọi nội bộ và ngoại tuyến.',1);
