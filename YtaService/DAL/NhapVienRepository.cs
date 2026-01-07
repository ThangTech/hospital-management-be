using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.DAL
{
    public class NhapVienRepository : INhapVienRepository
    {
        private readonly string _connectionString;

        public NhapVienRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool TaoPhieuNhapVien(NhapVienCreateDTO dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // GỌI STORED PROCEDURE: sp_NhapVien_TaoMoi
                using (SqlCommand cmd = new SqlCommand("sp_NhapVien_TaoMoi", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@BenhNhanId", dto.BenhNhanId);
                    cmd.Parameters.AddWithValue("@GiuongId", dto.GiuongId);
                    cmd.Parameters.AddWithValue("@KhoaId", dto.KhoaId);
                    cmd.Parameters.AddWithValue("@LyDoNhap", dto.LyDoNhap ?? (object)DBNull.Value);

                    // Hứng giá trị Return (0 hoặc 1)
                    var returnVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                    returnVal.Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)returnVal.Value == 1;
                    }
                    catch { return false; }
                }
            }
        }

        public List<NhapVienViewDTO> LayDanhSachDangDieuTri()
        {
            var list = new List<NhapVienViewDTO>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // GỌI STORED PROCEDURE: sp_NhapVien_GetAll
                using (SqlCommand cmd = new SqlCommand("sp_NhapVien_GetAll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new NhapVienViewDTO
                            {
                                Id = (Guid)reader["Id"],
                                BenhNhanId = (Guid)reader["BenhNhanId"],
                                TenBenhNhan = reader["TenBenhNhan"].ToString(), // Lấy từ SP
                                GiuongId = (Guid)reader["GiuongId"],
                                TenGiuong = reader["TenGiuong"].ToString(),     // Lấy từ SP
                                KhoaId = (Guid)reader["KhoaId"],
                                TenKhoa = reader["TenKhoa"].ToString(),         // Lấy từ SP
                                LyDoNhap = reader["LyDoNhap"].ToString(),
                                NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                                NgayXuat = reader["NgayXuat"] as DateTime?,
                                TrangThai = reader["TrangThai"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 3. CẬP NHẬT NHẬP VIỆN
        public bool CapNhatNhapVien(NhapVienUpdateDTO dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_NhapVien_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    // Xử lý null an toàn cho DateTime
                    cmd.Parameters.AddWithValue("@NgayXuat", dto.NgayXuat.HasValue ? (object)dto.NgayXuat.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@LyDoNhap", dto.LyDoNhap ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", dto.TrangThai ?? (object)DBNull.Value);

                    // Cách 1: Nếu SP có RETURN 1
                    var returnVal = cmd.Parameters.Add("@RetVal", SqlDbType.Int);
                    returnVal.Direction = ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    // Logic kiểm tra linh hoạt hơn:
                    // Nếu SQL trả về 1 (thành công) HOẶC không trả về gì nhưng không lỗi (giả định là update thành công)
                    if (returnVal.Value != DBNull.Value)
                    {
                        return (int)returnVal.Value == 1;
                    }

                    return true; // (Tùy chọn) Nếu SP không có RETURN, coi như chạy xong là thành công
                }
            }
        }

        // 4. XÓA NHẬP VIỆN (Kiểm tra điều kiện: đã xuất viện, không có hóa đơn, không nằm giường)
        public int XoaNhapVien(Guid id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_NhapVien_Delete", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

                        // --- KHẮC PHỤC LỖI EXECUTESCALAR ---
                        // Khai báo biến để hứng giá trị RETURN từ SQL
                        var returnVal = cmd.Parameters.Add("@RetVal", SqlDbType.Int);
                        returnVal.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery(); // Thực thi lệnh

                        // Kiểm tra kết quả trả về
                        if (returnVal.Value != DBNull.Value)
                            return (int)returnVal.Value;
                        else
                            return 0; // Trả về 0 nếu SP không return gì cả
                    }
                }
                catch (Exception)
                {
                    return -99; // Lỗi hệ thống
                }
            }
        }

        // 5. CHUYỂN GIƯỜNG (Có thể khác khoa)
        public bool ChuyenGiuong(ChuyenGiuongDTO dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_NhapVien_ChuyenGiuong", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@NhapVienId", dto.NhapVienId);
                    cmd.Parameters.AddWithValue("@GiuongMoiId", dto.GiuongMoiId);
                    cmd.Parameters.AddWithValue("@LyDoChuyenGiuong", dto.LyDoChuyenGiuong ?? (object)DBNull.Value);

                    var returnVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                    returnVal.Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        cmd.ExecuteNonQuery();
                        // 1 = thành công, 0 = giường mới không available hoặc lỗi
                        return (int)returnVal.Value == 1;
                    }
                    catch { return false; }
                }
            }
        }
        public NhapVienViewDTO GetById(Guid id)
        {
            NhapVienViewDTO data = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_NhapVien_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new NhapVienViewDTO
                            {
                                Id = (Guid)reader["Id"],
                                BenhNhanId = (Guid)reader["BenhNhanId"],
                                TenBenhNhan = reader["TenBenhNhan"].ToString(),
                                GiuongId = (Guid)reader["GiuongId"],
                                TenGiuong = reader["TenGiuong"].ToString(),
                                KhoaId = (Guid)reader["KhoaId"],
                                TenKhoa = reader["TenKhoa"].ToString(),
                                LyDoNhap = reader["LyDoNhap"].ToString(),
                                NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                                NgayXuat = reader["NgayXuat"] as DateTime?,
                                TrangThai = reader["TrangThai"].ToString()
                            };
                        }
                    }
                }
            }
            return data;
        }

        // 6. TÌM KIẾM NHẬP VIỆN
        public List<NhapVienViewDTO> TimKiem(NhapVienSearchDTO dto)
        {
            var list = new List<NhapVienViewDTO>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_NhapVien_TimKiem", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TenBenhNhan", dto.TenBenhNhan ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@KhoaId", dto.KhoaId.HasValue ? (object)dto.KhoaId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", dto.TrangThai ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TuNgay", dto.TuNgay.HasValue ? (object)dto.TuNgay.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@DenNgay", dto.DenNgay.HasValue ? (object)dto.DenNgay.Value : DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new NhapVienViewDTO
                            {
                                Id = (Guid)reader["Id"],
                                BenhNhanId = (Guid)reader["BenhNhanId"],
                                TenBenhNhan = reader["TenBenhNhan"].ToString(),
                                GiuongId = (Guid)reader["GiuongId"],
                                TenGiuong = reader["TenGiuong"].ToString(),
                                KhoaId = (Guid)reader["KhoaId"],
                                TenKhoa = reader["TenKhoa"].ToString(),
                                LyDoNhap = reader["LyDoNhap"].ToString(),
                                NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                                NgayXuat = reader["NgayXuat"] as DateTime?,
                                TrangThai = reader["TrangThai"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
