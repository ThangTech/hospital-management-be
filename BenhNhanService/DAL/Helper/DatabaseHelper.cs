using System.Data;
using System.Data.SqlClient;

namespace BenhNhanService.DAL.Helper
{
    public class DatabaseHelper
    {
        private string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 1. Hàm lấy dữ liệu (SELECT) - Có hỗ trợ Stored Procedure
        public DataTable ExecuteQuery(string query, CommandType cmdType = CommandType.Text, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = cmdType; // <-- QUAN TRỌNG: Xác định là Text hay StoredProcedure

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // 2. Hàm Thêm/Sửa/Xóa - Có hỗ trợ Stored Procedure
        public bool ExecuteNonQuery(string query, SqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = cmdType; // <-- QUAN TRỌNG

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }
    }
}