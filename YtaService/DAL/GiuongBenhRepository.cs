using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using YtaService.DAL.Helper;
using YtaService.DAL.Interfaces;
using YtaService.Models;
using YtaService.DTO;
using Microsoft.Extensions.Configuration;

namespace YtaService.DAL
{
    public class GiuongBenhRepository : IGiuongBenhRepository
    {
        private readonly GiuongBenhDatabaseHelper _dbHelper;
        private readonly string _connectionString;

        // --- SỬA TẠI ĐÂY: GỘP 2 CONSTRUCTOR THÀNH 1 ---
        // Hàm này nhận cả IConfiguration (cho hàm Update) và DatabaseHelper (cho hàm GetAll)
        public GiuongBenhRepository(IConfiguration configuration, GiuongBenhDatabaseHelper dbHelper)
        {
            // 1. Lấy connection string để dùng cho hàm UpdateGiuong
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            // 2. Lấy dbHelper để dùng cho hàm GetAll, Create...
            _dbHelper = dbHelper;
        }

        // --- HÀM 1: GET ALL (Lấy danh sách - Để sửa lỗi CS0535) ---
        public List<GiuongBenh> GetAll()
        {
            var list = new List<GiuongBenh>();

            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                // Gọi Procedure mới tạo
                using (var cmd = new SqlCommand("sp_GetAllGiuongOnly", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var giuong = new GiuongBenh();

                            // Map đúng 6 trường dữ liệu
                            giuong.Id = reader.GetGuid(0);
                            giuong.KhoaId = reader.IsDBNull(1) ? Guid.Empty : reader.GetGuid(1);
                            giuong.TenGiuong = reader.IsDBNull(2) ? "Chưa đặt tên" : reader.GetString(2);
                            giuong.LoaiGiuong = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            giuong.GiaTien = reader.IsDBNull(4) ? 0 : Convert.ToDecimal(reader.GetValue(4));
                            giuong.TrangThai = reader.IsDBNull(5) ? "Trống" : reader.GetString(5);

                            // Đảm bảo các object quan hệ là null để đỡ tốn ram
                            giuong.Khoa = null;
                            giuong.NhapViens = null;

                            list.Add(giuong);
                        }
                    }
                }
            }
            return list;
        }

        // Nhớ đổi kiểu trả về là GiuongBenhDetailDTO
        public GiuongBenhDetailDTO GetById(Guid id)
        {
            GiuongBenhDetailDTO result = null;

            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("sp_GetGiuongBenhById", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Id", id));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new GiuongBenhDetailDTO();

                            // 1. Map Giường
                            result.Id = reader.GetGuid(0);
                            result.TenGiuong = reader.GetString(1);
                            result.LoaiGiuong = reader.GetString(2);
                            result.GiaTien = reader.GetDecimal(3);
                            result.TrangThai = reader.GetString(4);

                            // 2. Map Khoa (Đọc thêm cột LoaiKhoa từ SQL)
                            // (Lưu ý: KhoaId là cột 5, TenKhoa là 6, LoaiKhoa là 7)
                            if (!reader.IsDBNull(5))
                            {
                                result.Khoa = new KhoaSimpleDTO
                                {
                                    TenKhoa = reader.GetString(6),
                                    LoaiKhoa = reader.GetString(7) // Đã có dữ liệu!
                                };
                            }

                            // 3. Map Nhập Viện & Bệnh Nhân
                            result.NhapViens = new List<NhapVienSimpleDTO>();

                            // Kiểm tra cột NhapVienId (cột số 9 - đếm kỹ thứ tự trong SQL)
                            // Thứ tự trong SQL: 0..4(Giường), 5..8(Khoa), 9(NvId), 10(NgayNhap), 11(BnId), 12(Ten), 13(GT), 14(BHYT), 15(NS)
                            if (!reader.IsDBNull(9))
                            {
                                var nv = new NhapVienSimpleDTO
                                {
                                    NgayNhap = reader.GetDateTime(10),
                                    BenhNhan = new BenhNhanSimpleDTO
                                    {
                                        HoTen = reader.GetString(12),
                                        GioiTinh = reader.GetString(13),       // Đã có dữ liệu!
                                        SoTheBaoHiem = reader.GetString(14),   // Đã có dữ liệu!
                                        NgaySinh = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15)
                                    }
                                };
                                result.NhapViens.Add(nv);
                            }
                        }
                    }
                }
            }
            return result;
        }
        public void Create(GiuongBenh giuong)
        {
            using (var conn = _dbHelper.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO GiuongBenh (Id, KhoaId, TenGiuong, LoaiGiuong, GiaTien, TrangThai) 
                                 VALUES (@Id, @KhoaId, @TenGiuong, @LoaiGiuong, @GiaTien, @TrangThai)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", giuong.Id);
                    cmd.Parameters.AddWithValue("@KhoaId", giuong.KhoaId); // Chú ý: Nếu KhoaId null thì phải truyền DBNull.Value
                    cmd.Parameters.AddWithValue("@TenGiuong", giuong.TenGiuong ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoaiGiuong", giuong.LoaiGiuong ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GiaTien", giuong.GiaTien);
                    cmd.Parameters.AddWithValue("@TrangThai", giuong.TrangThai ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool UpdateGiuong(GiuongUpdateDTO giuong)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    // Tên Stored Procedure
                    SqlCommand cmd = new SqlCommand("sp_Giuong_Update", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Truyền tham số (GUID và các kiểu dữ liệu khác)
                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = giuong.Id;
                    cmd.Parameters.Add("@KhoaId", SqlDbType.UniqueIdentifier).Value = giuong.KhoaId;

                    // Với NVarChar, nên chỉ định rõ kiểu để tránh lỗi font tiếng Việt
                    cmd.Parameters.Add("@LoaiGiuong", SqlDbType.NVarChar).Value = giuong.LoaiGiuong ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar).Value = giuong.TrangThai ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@TenGiuong", SqlDbType.NVarChar).Value = giuong.TenGiuong ?? (object)DBNull.Value;

                    cmd.Parameters.Add("@GiaTien", SqlDbType.Decimal).Value = giuong.GiaTien;

                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result > 0; // Trả về true nếu sửa thành công
                }
                catch (Exception ex)
                {
                    // Ghi log hoặc ném lỗi để Controller bắt được
                    throw new Exception("Lỗi Database: " + ex.Message);
                }
            }
        }

        public int DeleteGiuong(Guid id)
        {
            // Dùng _connectionString (đã có từ Constructor gộp)
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_Giuong_Delete", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

                    conn.Open();
                    // Dùng ExecuteScalar vì SP trả về 1 dòng kết quả (SELECT Result)
                    object result = cmd.ExecuteScalar();

                    return Convert.ToInt32(result);
                }
                catch (Exception ex)
                {
                    // Lỗi hệ thống khác
                    throw new Exception("Lỗi xóa giường: " + ex.Message);
                }
            }
        }

        // --- HÀM PHỤ TRỢ: Map dữ liệu (Để tránh viết lặp code) ---
        private GiuongBenh MapReaderToGiuongBenh(SqlDataReader reader)
        {
            var giuong = new GiuongBenh();

            // 1. Map Giường
            giuong.Id = reader.GetGuid(0);
            giuong.TenGiuong = reader.IsDBNull(1) ? "Chưa đặt tên" : reader.GetString(1);
            giuong.LoaiGiuong = reader.IsDBNull(2) ? "" : reader.GetString(2);
            giuong.GiaTien = reader.IsDBNull(3) ? 0 : Convert.ToDecimal(reader.GetValue(3));
            giuong.TrangThai = reader.IsDBNull(4) ? "Trống" : reader.GetString(4);
            giuong.KhoaId = reader.IsDBNull(5) ? Guid.Empty : reader.GetGuid(5);

            // 2. Map Khoa
            if (!reader.IsDBNull(5))
            {
                giuong.Khoa = new KhoaPhong
                {
                    Id = giuong.KhoaId.GetValueOrDefault(),
                    TenKhoa = reader.IsDBNull(6) ? "" : reader.GetString(6),
                    BacSis = new List<BacSi>(),
                    YTa = new List<YTa>()
                };
            }

            // 3. Map Nhập Viện
            giuong.NhapViens = new List<NhapVien>();
            if (!reader.IsDBNull(7))
            {
                giuong.NhapViens.Add(new NhapVien
                {
                    Id = reader.GetGuid(7),
                    NgayNhap = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8),
                    GiuongId = giuong.Id,
                    BenhNhan = new BenhNhan
                    {
                        Id = reader.GetGuid(9),
                        HoTen = reader.IsDBNull(10) ? "Không rõ tên" : reader.GetString(10)
                    }
                });
            }
            return giuong;
        }
    }
}