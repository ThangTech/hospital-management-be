using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using YtaService.DAL.Interfaces;
using YtaService.DTO;

namespace YtaService.DAL
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly string _connectionString;

        public HoaDonRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool TaoHoaDon(HoaDonCreateDTO dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_HoaDon_Create", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@BenhNhanId", dto.BenhNhanId);
                    cmd.Parameters.AddWithValue("@NhapVienId", dto.NhapVienId);
                    cmd.Parameters.AddWithValue("@TongTien", dto.TongTien);
                    cmd.Parameters.AddWithValue("@BaoHiemChiTra_Manual", (object)dto.BaoHiemChiTra ?? DBNull.Value);
                    
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

        public List<HoaDonViewDTO> LayDanhSach(Guid? benhNhanId, Guid? nhapVienId)
        {
            var list = new List<HoaDonViewDTO>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_HoaDon_GetList", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BenhNhanId", (object)benhNhanId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NhapVienId", (object)nhapVienId ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new HoaDonViewDTO
                            {
                                Id = (Guid)reader["Id"],
                                BenhNhanId = (Guid)reader["BenhNhanId"],
                                TenBenhNhan = reader["HoTen"].ToString(),
                                NhapVienId = (Guid)reader["NhapVienId"],
                                TongTien = reader["TongTien"] != DBNull.Value ? (decimal)reader["TongTien"] : 0,
                                BaoHiemChiTra = reader["BaoHiemChiTra"] != DBNull.Value ? (decimal)reader["BaoHiemChiTra"] : 0,
                                BenhNhanThanhToan = reader["BenhNhanThanhToan"] != DBNull.Value ? (decimal)reader["BenhNhanThanhToan"] : 0,
                                Ngay = reader["Ngay"] as DateTime?,
                                TrangThai = reader["TrangThai"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public HoaDonViewDTO GetById(Guid id)
        {
            HoaDonViewDTO data = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_HoaDon_GetById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new HoaDonViewDTO
                            {
                                Id = (Guid)reader["Id"],
                                BenhNhanId = (Guid)reader["BenhNhanId"],
                                TenBenhNhan = reader["HoTen"].ToString(),
                                NhapVienId = (Guid)reader["NhapVienId"],
                                TongTien = reader["TongTien"] != DBNull.Value ? (decimal)reader["TongTien"] : 0,
                                BaoHiemChiTra = reader["BaoHiemChiTra"] != DBNull.Value ? (decimal)reader["BaoHiemChiTra"] : 0,
                                BenhNhanThanhToan = reader["BenhNhanThanhToan"] != DBNull.Value ? (decimal)reader["BenhNhanThanhToan"] : 0,
                                Ngay = reader["Ngay"] as DateTime?,
                                TrangThai = reader["TrangThai"].ToString()
                            };
                        }
                    }
                }
            }
            return data;
        }

        public bool ThanhToan(HoaDonThanhToanDTO dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_HoaDon_ThanhToan", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", dto.Id);
                    cmd.Parameters.AddWithValue("@SoTien", dto.SoTien);

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

        public bool XoaHoaDon(Guid id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_HoaDon_Delete", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

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
        public HoaDonPreviewDTO LayGoiYVienPhi(Guid nhapVienId)
        {
            HoaDonPreviewDTO data = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_HoaDon_GetGoiY", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NhapVienId", nhapVienId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new HoaDonPreviewDTO
                            {
                                NhapVienId = (Guid)reader["NhapVienId"],
                                BenhNhanId = (Guid)reader["BenhNhanId"],
                                TenBenhNhan = reader["TenBenhNhan"].ToString(),
                                MucHuong = reader["MucHuong"] != DBNull.Value ? (decimal)reader["MucHuong"] : 0,
                                TenGiuong = reader["TenGiuong"] != DBNull.Value ? reader["TenGiuong"].ToString() : "N/A",
                                GiaGiuong = reader["GiaGiuong"] != DBNull.Value ? (decimal)reader["GiaGiuong"] : 0,
                                SoNgayNam = reader["SoNgayNam"] != DBNull.Value ? Convert.ToDouble(reader["SoNgayNam"]) : 0,
                                TongTienGoiY = reader["TongTienSuggested"] != DBNull.Value ? (decimal)reader["TongTienSuggested"] : 0
                            };
                        }
                    }
                }
            }
            return data;
        }
    }
}
