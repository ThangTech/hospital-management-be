using System.Data;
using System.Data.SqlClient;

namespace YtaService.DAL.Helper
{
    public class DatabaseHelper
    {
        public string ConnectionString { get; private set; }

        public DatabaseHelper(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // --- 1. Hàm lấy dữ liệu (SELECT) ---
        public DataTable ExecuteQuery(string query, CommandType cmdType = CommandType.Text, SqlParameter[] parameters = null)
        {
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

        // --- 2. Hàm Thêm/Sửa/Xóa (BẠN ĐANG THIẾU HÀM NÀY) ---
        public bool ExecuteNonQuery(string query, SqlParameter[] parameters = null, CommandType cmdType = CommandType.Text)
        {
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

                    // Thực thi lệnh và trả về số dòng bị ảnh hưởng
                    int result = cmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }
    }
}