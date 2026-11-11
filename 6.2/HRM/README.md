# Bài 6.2 – HRM & Payroll System

## 1. Thông tin sinh viên
- Họ và tên: [Điền họ tên]
- Mã sinh viên: [Điền MSV]
- Lớp / Khóa: [Điền lớp]

## 2. Công nghệ
C# WinForms, .NET 10, SQL Server, ADO.NET, DTO–DAL–BUS–GUI, DataSet/DataTable/SqlDataAdapter.

## 3. Cài đặt và chạy
1. Thực thi `Database/ScriptDB.sql` trong SQL Server Management Studio.
2. Kiểm tra connection string trong `HRM/DAL/Db.cs`.
3. Mở `HRM.sln` bằng Visual Studio 2022 có .NET 10 SDK.
4. Restore NuGet và chạy dự án.

## 4. Hình ảnh giao diện
> Các ảnh dưới đây là mockup minh họa giao diện, không phải ảnh chụp ứng dụng đang chạy.

![Màn hình chính](images/01_main_screen.png)
![Màn hình CRUD](images/02_crud_screen.png)
![Màn hình nghiệp vụ](images/03_transaction_screen.png)
![Màn hình thống kê](images/04_report_screen.png)
![Minh họa xử lý lỗi](images/05_error_handling.png)

## 5. Nghiệp vụ
- Tìm kiếm nhân viên dùng tham số SQL.
- Nạp chấm công theo tháng bằng SqlDataAdapter vào DataTable.
- Lưu dữ liệu chấm công theo tháng; kiểm tra bảng lương để khóa tháng đã chốt.
- Chốt lương trong SqlTransaction theo công thức đề bài.
- Bảng lương dùng MaLuong làm PK và UNIQUE(MaNV, ThangNam) để xử lý mâu thuẫn khóa chính trong đề.
## 6. Ghi chú
Giao diện hiện tại là bản khung nghiệp vụ; cần bổ sung thao tác thêm/sửa/xóa nhân viên trên GUI và kiểm thử thực tế trước khi nộp.
