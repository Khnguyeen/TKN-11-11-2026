using RetailSales.DAL;
using System.Data;
namespace RetailSales.BUS;
public class SalesBus {
 private readonly SalesDal dal=new();
 public DataTable Products(string q="")=>dal.Products(q);
 public DataTable Customers()=>dal.Customers();
 public DataTable Employees()=>dal.Employees();
 public void SaveProduct(string id,string n,decimal p,int s,bool add)=>dal.SaveProduct(id,n,p,s,add);
 public void DeleteProduct(string id)=>dal.DeleteProduct(id);
 public void SaveCustomer(string n,string p,bool add,int id=0)=>dal.SaveCustomer(n,p,add,id);
 public void DeleteCustomer(int id)=>dal.DeleteCustomer(id);
 public void SaveEmployee(string n,string p,string r,bool add,int id=0)=>dal.SaveEmployee(n,p,r,add,id);
 public void DeleteEmployee(int id)=>dal.DeleteEmployee(id);
 public int Checkout(int nv,int? kh,DataTable cart)=>dal.Checkout(nv,kh,cart);
 public decimal Revenue(string p)=>dal.Revenue(p);
 public DataTable TopProducts()=>dal.TopProducts();
}
