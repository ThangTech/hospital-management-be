using BenhNhanService.DAL.Helper;
using BenhNhanService.DAL.Interfaces;
using QuanLyBenhNhan.Models;
using System.Data;
using System.Data.SqlClient;

namespace BenhNhanService.DAL
{
    public class BenhNhanRepository : IBenhNhanRepository
    {
        private DatabaseHelper _dbHelper;

        public BenhNhanRepository(IConfiguration config)
        {
            _dbHelper = new DatabaseHelper(config);
        }

        public List<BenhNhan> GetAll()
        {
            try
            {
                // Gọi Stored Procedure: sp_BenhNhan_GetAll
                var dt = _dbHelper.ExecuteQuery("sp_BenhNhan_GetAll", CommandType.StoredProcedure);

                var result = new List<BenhNhan>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        // Xử lý chuyển đổi DateOnly an toàn
                        DateOnly? ns = null;
                        if (row["NgaySinh"] != DBNull.Value)
                        {
                            ns = DateOnly.FromDateTime(Convert.ToDateTime(row["NgaySinh"]));
                        }

                        result.Add(new BenhNhan
                        {
                            Id = Guid.Parse(row["Id"].ToString()),
                            HoTen = row["HoTen"].ToString(),
                            NgaySinh = ns,
                            GioiTinh = row["GioiTinh"] != DBNull.Value ? row["GioiTinh"].ToString() : null,
                            DiaChi = row["DiaChi"] != DBNull.Value ? row["DiaChi"].ToString() : null,
                            SoTheBaoHiem = row["SoTheBaoHiem"] != DBNull.Value ? row["SoTheBaoHiem"].ToString() : null
                        });
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Create(BenhNhan model)
        {
            try
            {
                if (model.Id == Guid.Empty) model.Id = Guid.NewGuid();

                // Chuẩn bị tham số để bắn vào Stored Procedure
                // Lưu ý: Tên tham số (@HoTen, @NgaySinh...) PHẢI KHỚP với trong SQL bạn vừa tạo
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@HoTen", model.HoTen ?? (object)DBNull.Value),
                    
                    // Xử lý DateOnly -> DateTime cho SQL
                    new SqlParameter("@NgaySinh", model.NgaySinh.HasValue ? model.NgaySinh.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value),

                    new SqlParameter("@GioiTinh", model.GioiTinh ?? (object)DBNull.Value),
                    new SqlParameter("@DiaChi", model.DiaChi ?? (object)DBNull.Value),
                    new SqlParameter("@SoTheBaoHiem", model.SoTheBaoHiem ?? (object)DBNull.Value)
                };

                // Gọi Stored Procedure: sp_BenhNhan_Create
                return _dbHelper.ExecuteNonQuery("sp_BenhNhan_Create", parameters, CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}