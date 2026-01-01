
using System.Data;
using System.Data.SqlClient; 
using YtaService.DAL.Helper;
using YtaService.DAL.Interfaces; 
using YtaService.DTO;
using YtaService.Models;

namespace YtaService.DAL
{
    public class YtaRepository : IYtaRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public YtaRepository(IConfiguration configuration)
        {
            _dbHelper = new DatabaseHelper(configuration);
        }

        public List<YTa> Search(YTaSearchDTO model, out long total)
        {
            total = 0;
            var result = new List<YTa>();

            using (SqlConnection conn = new SqlConnection(_dbHelper.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_Yta_Search", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PageIndex", model.PageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", model.PageSize);
                    cmd.Parameters.AddWithValue("@HoTen", (object)model.HoTen ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SoDienThoai", (object)model.SoDienThoai ?? DBNull.Value);

                    var pTotal = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(pTotal);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(ParseReader(reader));
                        }
                    }

                    if (pTotal.Value != DBNull.Value) total = Convert.ToInt64(pTotal.Value);
                }
            }
            return result;
        }

        public bool Create(YTa model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@HoTen", model.HoTen ?? (object)DBNull.Value),
                // Xử lý DateOnly? sang DateTime
                new SqlParameter("@NgaySinh", model.NgaySinh.HasValue ? model.NgaySinh.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value),
                new SqlParameter("@GioiTinh", model.GioiTinh ?? (object)DBNull.Value),
                new SqlParameter("@SoDienThoai", model.SoDienThoai ?? (object)DBNull.Value),
                new SqlParameter("@KhoaId", model.KhoaId ?? (object)DBNull.Value),
                new SqlParameter("@ChungChiHanhNghe", model.ChungChiHanhNghe ?? (object)DBNull.Value)
            };
            return _dbHelper.ExecuteNonQuery("sp_Yta_Create", parameters, CommandType.StoredProcedure);
        }

        public bool Update(YTa model)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@HoTen", model.HoTen ?? (object)DBNull.Value),
                new SqlParameter("@NgaySinh", model.NgaySinh.HasValue ? model.NgaySinh.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value),
                new SqlParameter("@GioiTinh", model.GioiTinh ?? (object)DBNull.Value),
                new SqlParameter("@SoDienThoai", model.SoDienThoai ?? (object)DBNull.Value),
                new SqlParameter("@KhoaId", model.KhoaId ?? (object)DBNull.Value),
                new SqlParameter("@ChungChiHanhNghe", model.ChungChiHanhNghe ?? (object)DBNull.Value)
            };
            return _dbHelper.ExecuteNonQuery("sp_Yta_Update", parameters, CommandType.StoredProcedure);
        }

        public bool Delete(string id)
        {
            return _dbHelper.ExecuteNonQuery("sp_Yta_Delete",
                new SqlParameter[] { new SqlParameter("@Id", id) }, CommandType.StoredProcedure);
        }
        public List<YTa> GetAll()
        {
            var dt = _dbHelper.ExecuteQuery("sp_Yta_GetAll", CommandType.StoredProcedure);
            var result = new List<YTa>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(ParseDataRow(row)); // Tận dụng hàm ParseDataRow đã viết ở dưới
                }
            }
            return result;
        }
        public YTa GetById(string id)
        {
            var dt = _dbHelper.ExecuteQuery("sp_Yta_GetById", CommandType.StoredProcedure,
                new SqlParameter[] { new SqlParameter("@Id", id) });

            if (dt != null && dt.Rows.Count > 0)
            {
                return ParseDataRow(dt.Rows[0]);
            }
            return null;
        }

        // --- Helper Mapping Methods ---
        private YTa ParseReader(SqlDataReader reader)
        {
            return new YTa
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                HoTen = reader["HoTen"].ToString(),
                NgaySinh = reader["NgaySinh"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(reader["NgaySinh"])) : null,
                GioiTinh = reader["GioiTinh"] != DBNull.Value ? reader["GioiTinh"].ToString() : null,
                SoDienThoai = reader["SoDienThoai"] != DBNull.Value ? reader["SoDienThoai"].ToString() : null,
                KhoaId = reader["KhoaId"] != DBNull.Value ? Guid.Parse(reader["KhoaId"].ToString()) : null,
                ChungChiHanhNghe = reader["ChungChiHanhNghe"] != DBNull.Value ? reader["ChungChiHanhNghe"].ToString() : null
            };
        }

        private YTa ParseDataRow(DataRow row)
        {
            return new YTa
            {
                Id = Guid.Parse(row["Id"].ToString()),
                HoTen = row["HoTen"].ToString(),
                NgaySinh = row["NgaySinh"] != DBNull.Value ? DateOnly.FromDateTime(Convert.ToDateTime(row["NgaySinh"])) : null,
                GioiTinh = row["GioiTinh"] != DBNull.Value ? row["GioiTinh"].ToString() : null,
                SoDienThoai = row["SoDienThoai"] != DBNull.Value ? row["SoDienThoai"].ToString() : null,
                KhoaId = row["KhoaId"] != DBNull.Value ? Guid.Parse(row["KhoaId"].ToString()) : null,
                ChungChiHanhNghe = row["ChungChiHanhNghe"] != DBNull.Value ? row["ChungChiHanhNghe"].ToString() : null
            };
        }
    }
}