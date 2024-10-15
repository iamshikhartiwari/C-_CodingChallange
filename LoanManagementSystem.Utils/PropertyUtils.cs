using System.Data.SqlClient;
using System.Configuration.ConfigurationManager;

namespace LoanManagementSystem.Utils;

public class PropertyUtils
{
    public static class PropertyUtil
    {
        public static SqlConnection getDBConnection()
        {
            SqlConnection connection = new SqlConnection();
            // string connectionstring = "Server=localhost,1433;Database=LoanManagementDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;";
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["MyCasestudyConnections"].ConnectionString;

            connection.ConnectionString = connectionstring;
            return connection;
        }
    }
}