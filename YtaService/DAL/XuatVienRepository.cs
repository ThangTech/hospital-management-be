using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.DAL
{
    public class XuatVienRepository : IXuatVienRepository
    {
        private readonly string _connectionString;

        public XuatVienRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int XuatVien(XuatVienDTO dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_NhapVien_XuatVien", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    cmd.Parameters.AddWithValue("@NgayXuat", dto.NgayXuat ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@ChanDoanXuatVien", dto.ChanDoanXuatVien ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LoiDanBacSi", dto.LoiDanBacSi ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GhiChu", dto.GhiChu ?? (object)DBNull.Value);

                    // Return values:
                    // 1: Thành công
                    // -1: Không tìm thấy phiếu
                    // -2: Còn hóa đơn chưa thanh toán
                    // -3: Đã xuất viện rồi
                    var returnVal = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                    returnVal.Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return (int)returnVal.Value;
                    }
                    catch { return -99; } // Lỗi hệ thống
                }
            }
        }

        // 2. LẤY DANH SÁCH SẴN SÀNG XUẤT VIỆN
        public List<SanSangXuatVienDTO> LayDanhSachSanSangXuatVien()
        {
            var list = new List<SanSangXuatVienDTO>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_XuatVien_LayDanhSachSanSang", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SanSangXuatVienDTO
                            {
                                NhapVienId = (Guid)reader["NhapVienId"],
                                TenBenhNhan = reader["TenBenhNhan"].ToString(),
                                TenGiuong = reader["TenGiuong"].ToString(),
                                TenKhoa = reader["TenKhoa"].ToString(),
                                NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                                SoNgayNam = Convert.ToInt32(reader["SoNgayNam"]),
                                TongTien = reader["TongTien"] != DBNull.Value ? Convert.ToDecimal(reader["TongTien"]) : 0
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 3. LẤY LỊCH SỬ XUẤT VIỆN
        public List<LichSuXuatVienDTO> LayLichSuXuatVien()
        {
            var list = new List<LichSuXuatVienDTO>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_XuatVien_LayLichSu", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LichSuXuatVienDTO
                            {
                                NhapVienId = (Guid)reader["NhapVienId"],
                                TenBenhNhan = reader["TenBenhNhan"].ToString(),
                                TenKhoa = reader["TenKhoa"].ToString(),
                                NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                                NgayXuat = Convert.ToDateTime(reader["NgayXuat"]),
                                SoNgayNam = Convert.ToInt32(reader["SoNgayNam"]),
                                ChanDoanXuatVien = reader["ChanDoanXuatVien"]?.ToString(),
                                LoiDanBacSi = reader["LoiDanBacSi"]?.ToString(),
                                GhiChu = reader["GhiChu"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // 4. XEM TRƯỚC XUẤT VIỆN
        public XuatVienPreviewDTO XemTruocXuatVien(Guid nhapVienId)
        {
            XuatVienPreviewDTO result = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_XuatVien_XemTruoc", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NhapVienId", nhapVienId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Đọc thông tin chính
                        if (reader.Read())
                        {
                            result = new XuatVienPreviewDTO
                            {
                                NhapVienId = (Guid)reader["NhapVienId"],
                                TenBenhNhan = reader["TenBenhNhan"].ToString(),
                                NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                                SoNgayNam = Convert.ToInt32(reader["SoNgayNam"]),
                                TongTienHoaDon = reader["TongTienHoaDon"] != DBNull.Value ? Convert.ToDecimal(reader["TongTienHoaDon"]) : 0,
                                DaThanhToan = reader["DaThanhToan"] != DBNull.Value ? Convert.ToDecimal(reader["DaThanhToan"]) : 0,
                                ConNo = reader["ConNo"] != DBNull.Value ? Convert.ToDecimal(reader["ConNo"]) : 0,
                                SanSangXuatVien = Convert.ToInt32(reader["SanSangXuatVien"]) == 1,
                                DanhSachHoaDon = new List<HoaDonDTO>()
                            };
                        }

                        // Đọc danh sách hóa đơn
                        if (result != null && reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                result.DanhSachHoaDon.Add(new HoaDonDTO
                                {
                                    Id = (Guid)reader["Id"],
                                    TongTien = reader["TongTien"] != DBNull.Value ? Convert.ToDecimal(reader["TongTien"]) : 0,
                                    TrangThai = reader["TrangThai"].ToString(),
                                    Ngay = reader["Ngay"] as DateTime?
                                });
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
