using System.Data;
using System.Data.SqlClient;

namespace BenhNhanService.DAL.Helper
{
    public class DatabaseHelper
    {
        // [SỬA 1] Chỉ giữ lại Property này (Xóa dòng public string _connectionString; đi)
        public string ConnectionString { get; private set; }

        public DatabaseHelper(IConfiguration configuration)
        {
            // [SỬA 2] Gán dữ liệu vào biến Property (Chữ Hoa)
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 1. Hàm lấy dữ liệu (SELECT)
        public DataTable ExecuteQuery(string query, CommandType cmdType = CommandType.Text, SqlParameter[] parameters = null)
        {
            // [SỬA 3] Dùng ConnectionString (Chữ Hoa)
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = cmdType;

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

        // 2. Hàm Thêm/Sửa/Xóa
        public bool ExecuteNonQuery(string query, SqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
            // [SỬA 4] Dùng ConnectionString (Chữ Hoa)
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = cmdType;

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