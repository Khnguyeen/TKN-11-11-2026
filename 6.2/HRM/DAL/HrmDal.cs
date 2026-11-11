using Microsoft.Data.SqlClient;
using System.Data;
namespace HRM.DAL;
public class HrmDal {
 public DataTable Query(string sql,params SqlParameter[] ps){using var c=Db.Open();using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddRange(ps);using var da=new SqlDataAdapter(cmd);var t=new DataTable();da.Fill(t);return t;}
 public int Execute(string sql,params SqlParameter[] ps){using var c=Db.Open();using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddRange(ps);return cmd.ExecuteNonQuery();}
 public DataTable Employees(string key="")=>Query("SELECT nv.MaNV,nv.HoTen,nv.NgaySinh,nv.CCCD,nv.MaPB,pb.TenPB,nv.LuongCoBan,nv.HeSoLuong FROM NhanVien nv JOIN PhongBan pb ON nv.MaPB=pb.MaPB WHERE nv.HoTen LIKE @k OR nv.CCCD LIKE @k OR pb.TenPB LIKE @k",new("@k","%"+key+"%"));
 public DataTable Departments()=>Query("SELECT * FROM PhongBan");
 public void SaveEmployee(string name,string cccd,int dept,decimal salary,double factor,bool insert,int id=0){
  if(insert)Execute("INSERT INTO NhanVien(HoTen,CCCD,MaPB,LuongCoBan,HeSoLuong) VALUES(@n,@c,@d,@s,@f)",new("@n",name),new("@c",cccd),new("@d",dept),new("@s",salary),new("@f",factor));
  else Execute("UPDATE NhanVien SET HoTen=@n,CCCD=@c,MaPB=@d,LuongCoBan=@s,HeSoLuong=@f WHERE MaNV=@id",new("@n",name),new("@c",cccd),new("@d",dept),new("@s",salary),new("@f",factor),new("@id",id));
 }
 public void DeleteEmployee(int id)=>Execute("DELETE FROM NhanVien WHERE MaNV=@id",new("@id",id));
 public DataTable LoadAttendance(string month){
  using var c=Db.Open();using var da=new SqlDataAdapter(@"SELECT nv.MaNV,nv.HoTen,ISNULL(bc.MaCong,0) AS MaCong,@month AS ThangNam,ISNULL(bc.SoNgayCong,0) AS SoNgayCong,ISNULL(bc.SoGioOT,0) AS SoGioOT FROM NhanVien nv LEFT JOIN BangChamCong bc ON nv.MaNV=bc.MaNV AND bc.ThangNam=@month",c);
  da.SelectCommand!.Parameters.AddWithValue("@month",month);
  var t=new DataTable();da.Fill(t);t.Columns["MaCong"]!.ReadOnly=true;t.Columns["MaNV"]!.ReadOnly=true;t.Columns["HoTen"]!.ReadOnly=true;t.Columns["ThangNam"]!.ReadOnly=true;return t;
 }
 public void SaveAttendance(DataTable table,string month){
  using var c=Db.Open();using var tx=c.BeginTransaction();
  try{
   foreach(DataRow r in table.Rows){
    int id=Convert.ToInt32(r["MaNV"]);double days=Convert.ToDouble(r["SoNgayCong"]);double ot=Convert.ToDouble(r["SoGioOT"]);
    using var check=new SqlCommand("IF EXISTS(SELECT 1 FROM BangLuong WHERE MaNV=@id AND ThangNam=@m) SELECT 1 ELSE SELECT 0",c,tx);
    check.Parameters.AddWithValue("@id",id);check.Parameters.AddWithValue("@m",month);
    if((int)check.ExecuteScalar()! == 1)throw new InvalidOperationException("Tháng đã chốt lương, không thể sửa chấm công.");
    using var cmd=new SqlCommand(@"IF EXISTS(SELECT 1 FROM BangChamCong WHERE MaNV=@id AND ThangNam=@m) UPDATE BangChamCong SET SoNgayCong=@d,SoGioOT=@ot WHERE MaNV=@id AND ThangNam=@m ELSE INSERT INTO BangChamCong(MaNV,ThangNam,SoNgayCong,SoGioOT) VALUES(@id,@m,@d,@ot)",c,tx);
    cmd.Parameters.AddWithValue("@id",id);cmd.Parameters.AddWithValue("@m",month);cmd.Parameters.AddWithValue("@d",days);cmd.Parameters.AddWithValue("@ot",ot);cmd.ExecuteNonQuery();
   } tx.Commit();
  }catch{tx.Rollback();throw;}
 }
 public void ClosePayroll(string month){
  using var c=Db.Open();using var tx=c.BeginTransaction();
  try{
   using var cmd=new SqlCommand(@"INSERT INTO BangLuong(MaNV,ThangNam,ThucLinh)
   SELECT nv.MaNV,@m,((nv.LuongCoBan*nv.HeSoLuong/22)*bc.SoNgayCong)+(bc.SoGioOT*50000)
   FROM NhanVien nv JOIN BangChamCong bc ON nv.MaNV=bc.MaNV AND bc.ThangNam=@m
   WHERE NOT EXISTS(SELECT 1 FROM BangLuong bl WHERE bl.MaNV=nv.MaNV AND bl.ThangNam=@m)",c,tx);
   cmd.Parameters.AddWithValue("@m",month);cmd.ExecuteNonQuery();tx.Commit();
  }catch{tx.Rollback();throw;}
 }
 public DataTable Payroll(string month)=>Query("SELECT nv.MaNV,nv.HoTen,bl.ThangNam,bl.ThucLinh FROM BangLuong bl JOIN NhanVien nv ON nv.MaNV=bl.MaNV WHERE bl.ThangNam=@m",new("@m",month));
}
