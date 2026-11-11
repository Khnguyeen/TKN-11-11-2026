using Microsoft.Data.SqlClient;
using System.Configuration;
namespace RetailSales.DAL;
public static class Db {
 public static string ConnectionString => ConfigurationManager.ConnectionStrings["Db"]?.ConnectionString
   ?? @"Server=(localdb)\MSSQLLocalDB;Database=SalesDB;Integrated Security=true;TrustServerCertificate=true;";
 public static SqlConnection Open() { var c=new SqlConnection(ConnectionString); c.Open(); return c; }
}
