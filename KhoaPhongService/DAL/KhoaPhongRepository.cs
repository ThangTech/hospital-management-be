using System.Data;
using System.Data.SqlClient;
using KhoaPhongService.DAL.Helper;
using KhoaPhongService.DAL.Interfaces;
using KhoaPhongService.Models;

namespace KhoaPhongService.DAL
{
    public class KhoaPhongRepository : IKhoaPhongRepository
    {
        private readonly DatabaseHelper _dbHelper;
        public KhoaPhongRepository(IConfiguration config)
        {
            _dbHelper = new DatabaseHelper(config);
        }

        public List<KhoaPhong> GetAll()
        {
            var dt = _dbHelper.ExecuteQuery("sp_KhoaPhong_GetAll", CommandType.StoredProcedure);
            var result = new List<KhoaPhong>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows) result.Add(ParseDataRow(row));
            }
            return result;
        }

        public KhoaPhong GetById(string id)
        {
            var dt = _dbHelper.ExecuteQuery("sp_KhoaPhong_GetById", CommandType.StoredProcedure,
                new SqlParameter[] { new SqlParameter("@Id", id) });

            if (dt != null && dt.Rows.Count > 0) return ParseDataRow(dt.Rows[0]);
            return null;
        }

        public bool Create(KhoaPhong model)
        {
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@TenKhoa", model.TenKhoa),
                    new SqlParameter("@LoaiKhoa", model.LoaiKhoa),
                    new SqlParameter("@SoGiuongTieuChuan", model.SoGiuongTieuChuan)
                };
                _dbHelper.ExecuteNonQuery("sp_KhoaPhong_Create", p, CommandType.StoredProcedure);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(KhoaPhong model)
        {
            try
            {
                SqlParameter[] p = {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@TenKhoa", model.TenKhoa),
                    new SqlParameter("@LoaiKhoa", model.LoaiKhoa),
                    new SqlParameter("@SoGiuongTieuChuan", model.SoGiuongTieuChuan)
                };
                _dbHelper.ExecuteNonQuery("sp_KhoaPhong_Update", p, CommandType.StoredProcedure);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                _dbHelper.ExecuteNonQuery("sp_KhoaPhong_Delete",
                    new SqlParameter[] { new SqlParameter("@Id", id) },
                    CommandType.StoredProcedure);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<KhoaPhong> Search(string keyword)
        {
            // Nếu keyword null thì gán bằng chuỗi rỗng để tránh lỗi SQL
            string searchTerm = keyword ?? "";

            var dt = _dbHelper.ExecuteQuery("sp_KhoaPhong_Search", CommandType.StoredProcedure,
                new SqlParameter[] { new SqlParameter("@Keyword", searchTerm) });

            var result = new List<KhoaPhong>();
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows) result.Add(ParseDataRow(row));
            }
            return result;
        }


        public int CheckDependencies(string id, out string message)
        {
            message = "";
            using (SqlConnection conn = new SqlConnection(_dbHelper.ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_KhoaPhong_CheckDependencies", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@KhoaId", Guid.Parse(id));

                    // Tham số đầu ra đếm số lượng
                    var pCount = new SqlParameter("@Count", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    // Tham số đầu ra lấy thông báo lỗi
                    var pMsg = new SqlParameter("@Message", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(pCount);
                    cmd.Parameters.Add(pMsg);

                    cmd.ExecuteNonQuery();

                    if (pMsg.Value != DBNull.Value) message = pMsg.Value.ToString();
                    return pCount.Value != DBNull.Value ? Convert.ToInt32(pCount.Value) : 0;
                }
            }
        }
        private KhoaPhong ParseDataRow(DataRow row)
        {
            return new KhoaPhong
            {
                Id = Guid.Parse(row["Id"].ToString()),
                TenKhoa = row["TenKhoa"].ToString(),
                LoaiKhoa = row["LoaiKhoa"].ToString(),
                SoGiuongTieuChuan = Convert.ToInt32(row["SoGiuongTieuChuan"])
            };
        }
    }
}