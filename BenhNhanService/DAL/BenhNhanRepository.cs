using BenhNhanService.DAL.Helper;
using BenhNhanService.DAL.Interfaces;
using QuanLyBenhNhan.Models;
using System.Data;
using System.Data.SqlClient;
using static BenhNhanService.DTO.BenhNhanSearchDTO;

namespace BenhNhanService.DAL
{
    public class BenhNhanRepository : IBenhNhanRepository
    {
        private DatabaseHelper _dbHelper;
        public BenhNhanRepository(IConfiguration config)
        {
            _dbHelper = new DatabaseHelper(config);
        }

        // --- 1. ĐÂY LÀ HÀM BẠN ĐANG THIẾU (GÂY RA LỖI CS0535) ---
        public List<BenhNhan> GetAll()
        {
            try
            {
                var dt = _dbHelper.ExecuteQuery("sp_BenhNhan_GetAll", CommandType.StoredProcedure);
                var result = new List<BenhNhan>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(ParseDataRow(row)); // Gọi hàm chuyển đổi dữ liệu dùng chung
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // --- 2. Hàm Create (Đã chuẩn) ---
        public bool Create(BenhNhan model)
        {
            // Sanitize MucHuong (Ensure it is decimal 0-1)
            decimal? safeMucHuong = model.MucHuong;
            if (safeMucHuong.HasValue && safeMucHuong.Value > 1)
            {
                safeMucHuong = safeMucHuong.Value / 100m;
            }

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@HoTen", model.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@NgaySinh", model.NgaySinh.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@GioiTinh", model.GioiTinh ?? (object)DBNull.Value),
                new SqlParameter("@DiaChi", model.DiaChi ?? (object)DBNull.Value),
                new SqlParameter("@SoTheBaoHiem", model.SoTheBaoHiem ?? (object)DBNull.Value),
                new SqlParameter("@MucHuong", safeMucHuong ?? (object)DBNull.Value),
                new SqlParameter("@HanTheBHYT", model.HanTheBHYT ?? (object)DBNull.Value),
                new SqlParameter("@TrangThai", model.TrangThai ?? (object)DBNull.Value)
            };
            return _dbHelper.ExecuteNonQuery("sp_BenhNhan_Create", parameters, CommandType.StoredProcedure);
        }

        // --- 3. Hàm Update (Tôi đã viết đầy đủ tham số cho bạn) ---
        public bool Update(BenhNhan model)
        {
            // Sanitize MucHuong (Ensure it is decimal 0-1)
            decimal? safeMucHuong = model.MucHuong;
            if (safeMucHuong.HasValue && safeMucHuong.Value > 1)
            {
                safeMucHuong = safeMucHuong.Value / 100m;
            }

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@HoTen", model.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@NgaySinh", model.NgaySinh.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@GioiTinh", model.GioiTinh ?? (object)DBNull.Value),
                new SqlParameter("@DiaChi", model.DiaChi ?? (object)DBNull.Value),
                new SqlParameter("@SoTheBaoHiem", model.SoTheBaoHiem ?? (object)DBNull.Value),
                new SqlParameter("@MucHuong", safeMucHuong ?? (object)DBNull.Value),
                new SqlParameter("@HanTheBHYT", model.HanTheBHYT ?? (object)DBNull.Value),
                new SqlParameter("@TrangThai", model.TrangThai ?? (object)DBNull.Value)
            };
            return _dbHelper.ExecuteNonQuery("sp_BenhNhan_Update", parameters, CommandType.StoredProcedure);
        }

        // --- 4. Hàm GetByID (Tôi đã viết logic chuyển đổi dữ liệu thật) ---
        public BenhNhan GetDatabyID(string id)
        {
            var dt = _dbHelper.ExecuteQuery("sp_BenhNhan_GetById", CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@Id", id) });
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ParseDataRow(dt.Rows[0]); // Lấy dòng đầu tiên
            }
            return null; // Không tìm thấy
        }

        // --- 5. Hàm Delete (Đã chuẩn) ---
        public bool Delete(string id)
        {
            return _dbHelper.ExecuteNonQuery("sp_BenhNhan_Delete", 
                new SqlParameter[] { new SqlParameter("@Id", id) }, CommandType.StoredProcedure);
        }

        // --- Hàm phụ trợ: Chuyển đổi từ DataRow sang Object BenhNhan (Tránh lặp code) ---
        private BenhNhan ParseDataRow(DataRow row)
        {
            return new BenhNhan
            {
                Id = Guid.Parse(row["Id"].ToString()),
                HoTen = row["HoTen"].ToString(),
                NgaySinh = DateOnly.FromDateTime(Convert.ToDateTime(row["NgaySinh"])),
                GioiTinh = row["GioiTinh"] != DBNull.Value ? row["GioiTinh"].ToString() : null,
                DiaChi = row["DiaChi"] != DBNull.Value ? row["DiaChi"].ToString() : null,
                SoTheBaoHiem = row["SoTheBaoHiem"] != DBNull.Value ? row["SoTheBaoHiem"].ToString() : null,
                MucHuong = row["MucHuong"] != DBNull.Value ? Convert.ToDecimal(row["MucHuong"]) : null,
                HanTheBHYT = row["HanTheBHYT"] != DBNull.Value ? Convert.ToDateTime(row["HanTheBHYT"]) : null,
                TrangThai = row.Table.Columns.Contains("TrangThai") && row["TrangThai"] != DBNull.Value ? row["TrangThai"].ToString() : null
            };
        }

        private BenhNhan ParseDataRow(SqlDataReader reader)
        {
            return new BenhNhan
            {
                // SqlDataReader cũng dùng indexer ["TenCot"] giống hệt DataRow
                Id = Guid.Parse(reader["Id"].ToString()),
                HoTen = reader["HoTen"].ToString(),
                NgaySinh = DateOnly.FromDateTime(Convert.ToDateTime(reader["NgaySinh"])),
                GioiTinh = reader["GioiTinh"] != DBNull.Value ? reader["GioiTinh"].ToString() : null,
                DiaChi = reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : null,
                SoTheBaoHiem = reader["SoTheBaoHiem"] != DBNull.Value ? reader["SoTheBaoHiem"].ToString() : null,
                MucHuong = reader["MucHuong"] != DBNull.Value ? Convert.ToDecimal(reader["MucHuong"]) : null,
                HanTheBHYT = reader["HanTheBHYT"] != DBNull.Value ? Convert.ToDateTime(reader["HanTheBHYT"]) : null,
                TrangThai = Enumerable.Range(0, reader.FieldCount).Any(i => reader.GetName(i).Equals("TrangThai", StringComparison.OrdinalIgnoreCase)) && reader["TrangThai"] != DBNull.Value ? reader["TrangThai"].ToString() : null
            };
        }

        // Đừng quên sửa Interface IBenhNhanRepository trước nhé!
        public List<BenhNhan> Search(BenhNhanSearchModel model, out long total)
        {
            total = 0;
            try
            {
                var result = new List<BenhNhan>();
                using (SqlConnection conn = new SqlConnection(_dbHelper.ConnectionString)) // Lấy ConnectionString từ Helper
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_BenhNhan_Search", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Truyền tham số từ DTO vào SQL
                        cmd.Parameters.AddWithValue("@PageIndex", model.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", model.PageSize);

                        // Xử lý null an toàn cho 3 tiêu chí
                        cmd.Parameters.AddWithValue("@HoTen", (object)model.HoTen ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DiaChi", (object)model.DiaChi ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SoTheBaoHiem", (object)model.SoTheBaoHiem ?? DBNull.Value);

                        // Tham số đầu ra (Output) lấy tổng số dòng
                        var pTotal = new SqlParameter("@TotalRecord", SqlDbType.Int);
                        pTotal.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pTotal);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(ParseDataRow(reader)); // Hàm convert dùng chung của bạn
                            }
                        }

                        if (pTotal.Value != DBNull.Value) total = Convert.ToInt64(pTotal.Value);
                    }
                }
                return result;
            }
            catch (Exception) { throw; }
        }
    }
}