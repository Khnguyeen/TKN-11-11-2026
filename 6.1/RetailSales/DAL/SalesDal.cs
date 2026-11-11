using Microsoft.Data.SqlClient;
using RetailSales.DTO;
using System.Data;
namespace RetailSales.DAL;
public class SalesDal {
 public DataTable Query(string sql, params SqlParameter[] ps) { using var c=Db.Open(); using var cmd=new SqlCommand(sql,c); cmd.Parameters.AddRange(ps); using var da=new SqlDataAdapter(cmd); var t=new DataTable(); da.Fill(t); return t; }
 public int Execute(string sql, params SqlParameter[] ps) { using var c=Db.Open(); using var cmd=new SqlCommand(sql,c); cmd.Parameters.AddRange(ps); return cmd.ExecuteNonQuery(); }
 public DataTable Products(string keyword="") => Query("SELECT MaSP,TenSP,GiaBan,SoLuongTon FROM SanPham WHERE MaSP LIKE @k OR TenSP LIKE @k",new SqlParameter("@k","%"+keyword+"%"));
 public DataTable Customers() => Query("SELECT * FROM KhachHang");
 public DataTable Employees() => Query("SELECT * FROM NhanVien");
 public void SaveProduct(string id,string name,decimal price,int stock,bool insert) {
  if(insert) Execute("INSERT INTO SanPham(MaSP,TenSP,GiaBan,SoLuongTon) VALUES(@id,@n,@p,@s)",new("@id",id),new("@n",name),new("@p",price),new("@s",stock));
  else Execute("UPDATE SanPham SET TenSP=@n,GiaBan=@p,SoLuongTon=@s WHERE MaSP=@id",new("@id",id),new("@n",name),new("@p",price),new("@s",stock));
 }
 public void DeleteProduct(string id)=>Execute("DELETE FROM SanPham WHERE MaSP=@id",new("@id",id));
 public void SaveCustomer(string name,string phone,bool insert,int id=0) {
  if(insert) Execute("INSERT INTO KhachHang(TenKH,DienThoai) VALUES(@n,@p)",new("@n",name),new("@p",phone));
  else Execute("UPDATE KhachHang SET TenKH=@n,DienThoai=@p WHERE MaKH=@id",new("@n",name),new("@p",phone),new("@id",id));
 }
 public void DeleteCustomer(int id)=>Execute("DELETE FROM KhachHang WHERE MaKH=@id",new("@id",id));
 public void SaveEmployee(string name,string phone,string role,bool insert,int id=0) {
  if(insert) Execute("INSERT INTO NhanVien(TenNV,SoDienThoai,ChucVu) VALUES(@n,@p,@r)",new("@n",name),new("@p",phone),new("@r",role));
  else Execute("UPDATE NhanVien SET TenNV=@n,SoDienThoai=@p,ChucVu=@r WHERE MaNV=@id",new("@n",name),new("@p",phone),new("@r",role),new("@id",id));
 }
 public void DeleteEmployee(int id)=>Execute("DELETE FROM NhanVien WHERE MaNV=@id",new("@id",id));
 public int Checkout(int employeeId,int? customerId,DataTable cart) {
  using var c=Db.Open(); using var tx=c.BeginTransaction();
  try {
   decimal total=cart.AsEnumerable().Sum(r=>r.Field<decimal>("GiaBan")*r.Field<int>("SoLuong"));
   using var h=new SqlCommand("INSERT INTO HoaDon(MaNV,MaKH,TongTien) OUTPUT INSERTED.MaHD VALUES(@nv,@kh,@total)",c,tx);
   h.Parameters.AddWithValue("@nv",employeeId); h.Parameters.AddWithValue("@kh",(object?)customerId??DBNull.Value); h.Parameters.AddWithValue("@total",total);
   int id=(int)h.ExecuteScalar()!;
   foreach(DataRow r in cart.Rows) {
    string sp=r.Field<string>("MaSP")!; int qty=r.Field<int>("SoLuong");
    using var stock=new SqlCommand("UPDATE SanPham SET SoLuongTon=SoLuongTon-@q WHERE MaSP=@sp AND SoLuongTon>=@q",c,tx);
    stock.Parameters.AddWithValue("@q",qty); stock.Parameters.AddWithValue("@sp",sp);
    if(stock.ExecuteNonQuery()!=1) throw new InvalidOperationException("Không đủ tồn kho cho sản phẩm "+sp);
    using var detail=new SqlCommand("INSERT INTO ChiTietHoaDon(MaHD,MaSP,SoLuong,DonGia) VALUES(@hd,@sp,@q,@price)",c,tx);
    detail.Parameters.AddWithValue("@hd",id); detail.Parameters.AddWithValue("@sp",sp); detail.Parameters.AddWithValue("@q",qty); detail.Parameters.AddWithValue("@price",r.Field<decimal>("GiaBan"));
    detail.ExecuteNonQuery();
   }
   if(customerId.HasValue) { using var point=new SqlCommand("UPDATE KhachHang SET DiemTichLuy=DiemTichLuy+CONVERT(int,@total/100000) WHERE MaKH=@kh",c,tx); point.Parameters.AddWithValue("@total",total); point.Parameters.AddWithValue("@kh",customerId.Value); point.ExecuteNonQuery(); }
   tx.Commit(); return id;
  } catch { tx.Rollback(); throw; }
 }
 public decimal Revenue(string period) { using var c=Db.Open(); using var cmd=new SqlCommand("SELECT COALESCE(SUM(TongTien),0) FROM HoaDon WHERE (@p='day' AND CAST(NgayLap AS date)=CAST(GETDATE() AS date)) OR (@p='month' AND YEAR(NgayLap)=YEAR(GETDATE()) AND MONTH(NgayLap)=MONTH(GETDATE()))",c); cmd.Parameters.AddWithValue("@p",period); return Convert.ToDecimal(cmd.ExecuteScalar()); }
 public DataTable TopProducts()=>Query("SELECT TOP 5 sp.MaSP,sp.TenSP,SUM(ct.SoLuong) AS DaBan FROM ChiTietHoaDon ct JOIN SanPham sp ON sp.MaSP=ct.MaSP GROUP BY sp.MaSP,sp.TenSP ORDER BY DaBan DESC");
}
