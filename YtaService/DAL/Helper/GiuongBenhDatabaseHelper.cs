using Microsoft.Extensions.Configuration;
// Helper này dùng thư viện MỚI cho Giường Bệnh
using Microsoft.Data.SqlClient;
using System.Data;

namespace YtaService.DAL.Helper
{
    public class GiuongBenhDatabaseHelper
    {
        public string ConnectionString { get; private set; }

        public GiuongBenhDatabaseHelper(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // Hàm hỗ trợ ExecuteNonQuery cho Giường Bệnh
        public void ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}