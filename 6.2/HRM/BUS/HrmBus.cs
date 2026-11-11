using HRM.DAL;
using System.Data;
namespace HRM.BUS;
public class HrmBus {
 readonly HrmDal dal=new();
 public DataTable Employees(string k="")=>dal.Employees(k);
 public DataTable Departments()=>dal.Departments();
 public void SaveEmployee(string n,string c,int d,decimal s,double f,bool add,int id=0)=>dal.SaveEmployee(n,c,d,s,f,add,id);
 public void DeleteEmployee(int id)=>dal.DeleteEmployee(id);
 public DataTable LoadAttendance(string m)=>dal.LoadAttendance(m);
 public void SaveAttendance(DataTable t,string m)=>dal.SaveAttendance(t,m);
 public void ClosePayroll(string m)=>dal.ClosePayroll(m);
 public DataTable Payroll(string m)=>dal.Payroll(m);
}
