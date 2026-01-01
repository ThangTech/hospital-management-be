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
            SqlParameter[] p = {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@TenKhoa", model.TenKhoa),
                new SqlParameter("@LoaiKhoa", model.LoaiKhoa),
                new SqlParameter("@SoGiuongTieuChuan", model.SoGiuongTieuChuan)
            };
            return _dbHelper.ExecuteNonQuery("sp_KhoaPhong_Create", p, CommandType.StoredProcedure);
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