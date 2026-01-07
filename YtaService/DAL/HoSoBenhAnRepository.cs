using System.Data;
using System.Data.SqlClient;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.DAL
{
    public class HoSoBenhAnRepository : IHoSoBenhAnRepository
    {
        private readonly string _connectionString;

        public HoSoBenhAnRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool TaoHoSo(HoSoBenhAnCreateDTO hoso)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // SQL Insert với Guid
                string query = @"INSERT INTO HoSoBenhAn 
                                (Id, NhapVienId, BacSiPhuTrachId, TienSuBenh, ChanDoanBanDau, PhuongAnDieuTri, NgayLap) 
                                VALUES 
                                (@Id, @NhapVienId, @BacSiPhuTrachId, @TienSuBenh, @ChanDoanBanDau, @PhuongAnDieuTri, @NgayLap)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Tự tạo Guid mới cho Id
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());

                    // Xử lý giá trị NULL (Nếu hoso.NhapVienId null thì truyền DBNull.Value)
                    cmd.Parameters.AddWithValue("@NhapVienId", hoso.NhapVienId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BacSiPhuTrachId", hoso.BacSiPhuTrachId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TienSuBenh", hoso.TienSuBenh ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChanDoanBanDau", hoso.ChanDoanBanDau ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhuongAnDieuTri", hoso.PhuongAnDieuTri ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayLap", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<HoSoBenhAnViewDTO> LayTheoNhapVien(Guid nhapVienId)
        {
            var list = new List<HoSoBenhAnViewDTO>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Lấy thông tin dựa trên Id Nhập Viện
                string query = "SELECT * FROM HoSoBenhAn WHERE NhapVienId = @NhapVienId ORDER BY NgayLap DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NhapVienId", nhapVienId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new HoSoBenhAnViewDTO
                            {
                                Id = (Guid)reader["Id"],
                                NhapVienId = reader["NhapVienId"] as Guid?,
                                BacSiPhuTrachId = reader["BacSiPhuTrachId"] as Guid?,
                                TienSuBenh = reader["TienSuBenh"]?.ToString(),
                                ChanDoanBanDau = reader["ChanDoanBanDau"]?.ToString(),
                                PhuongAnDieuTri = reader["PhuongAnDieuTri"]?.ToString(),
                                ChanDoanRaVien = reader["ChanDoanRaVien"]?.ToString(),
                                KetQuaDieuTri = reader["KetQuaDieuTri"]?.ToString(),
                                NgayLap = reader["NgayLap"] as DateTime?
                            });
                        }
                    }
                }
            }
            return list;
        }
        public List<HoSoBenhAnViewDTO> LayTatCa()
        {
            var list = new List<HoSoBenhAnViewDTO>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Lấy toàn bộ dữ liệu, sắp xếp cái mới nhất lên đầu
                string query = "SELECT * FROM HoSoBenhAn ORDER BY NgayLap DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new HoSoBenhAnViewDTO
                            {
                                Id = (Guid)reader["Id"],
                                // Sử dụng 'as' để ép kiểu an toàn cho các trường có thể NULL
                                NhapVienId = reader["NhapVienId"] as Guid?,
                                BacSiPhuTrachId = reader["BacSiPhuTrachId"] as Guid?,
                                TienSuBenh = reader["TienSuBenh"]?.ToString(),
                                ChanDoanBanDau = reader["ChanDoanBanDau"]?.ToString(),
                                PhuongAnDieuTri = reader["PhuongAnDieuTri"]?.ToString(),
                                ChanDoanRaVien = reader["ChanDoanRaVien"]?.ToString(),
                                KetQuaDieuTri = reader["KetQuaDieuTri"]?.ToString(),
                                NgayLap = reader["NgayLap"] as DateTime?
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}