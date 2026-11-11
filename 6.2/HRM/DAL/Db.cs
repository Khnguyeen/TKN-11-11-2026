using Microsoft.Data.SqlClient;
namespace HRM.DAL;
public static class Db {
 public const string ConnectionString=@"Server=(localdb)\MSSQLLocalDB;Database=HRMDB;Integrated Security=true;TrustServerCertificate=true;";
 public static SqlConnection Open(){var c=new SqlConnection(ConnectionString);c.Open();return c;}
}
