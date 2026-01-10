using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BacSiService.DAL.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly string? _connectionString;

        public MedicalRecordRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration["ConnectionStrings:DefaultConnection"];
        }

        public List<MedicalRecordDto> GetAll()
        {
            var result = new List<MedicalRecordDto>();
            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetAllMedicalRecords", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(MapToDto(reader));
                    }
                }
            }
            return result;
        }

        public PagedResult<MedicalRecordDto> GetByAdmission(Guid? patientId, int pageNumber, int pageSize, string? searchTerm)
        {
            var result = new PagedResult<MedicalRecordDto>
            {
                Data = new List<MedicalRecordDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetMedicalRecords", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NhapVienId", patientId.HasValue ? (object)patientId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                var totalParam = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(totalParam);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Data.Add(MapToDto(reader));
                    }
                }

                result.TotalRecords = (int)(totalParam.Value ?? 0);
                result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / pageSize);
            }
            return result;
        }

        public MedicalRecordDto? Create(MedicalRecordDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_CreateMedicalRecord", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NhapVienId", dto.NhapVienId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TienSuBenh", dto.TienSuBenh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChanDoanBanDau", dto.ChanDoanBanDau ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PhuongAnDieuTri", dto.PhuongAnDieuTri ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KetQuaDieuTri", dto.KetQuaDieuTri ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChanDoanRaVien", dto.ChanDoanRaVien ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BacSiPhuTrachId", dto.BacSiPhuTrachId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NguoiDungId", nguoiDungId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AuditUser", auditUser ?? (object)DBNull.Value);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapToDto(reader);
                    }
                }
            }
            return null;
        }

        public MedicalRecordDto? Update(Guid id, MedicalRecordDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateMedicalRecord", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@TienSuBenh", dto.TienSuBenh ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChanDoanBanDau", dto.ChanDoanBanDau ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChanDoanRaVien", dto.ChanDoanRaVien ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PhuongAnDieuTri", dto.PhuongAnDieuTri ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KetQuaDieuTri", dto.KetQuaDieuTri ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BacSiPhuTrachId", dto.BacSiPhuTrachId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NguoiDungId", nguoiDungId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AuditUser", auditUser ?? (object)DBNull.Value);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapToDto(reader);
                    }
                }
            }
            return null;
        }

        public bool Delete(Guid id, Guid? nguoiDungId = null, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return false;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_DeleteMedicalRecord", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@NguoiDungId", nguoiDungId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AuditUser", auditUser ?? (object)DBNull.Value);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0 || rows == -1;
            }
        }

        private static MedicalRecordDto MapToDto(SqlDataReader reader)
        {
            var dto = new MedicalRecordDto
            {
                Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                NhapVienId = reader["NhapVienId"] == DBNull.Value ? (Guid?)null : (Guid)reader["NhapVienId"],
                TienSuBenh = reader["TienSuBenh"] as string,
                ChanDoanBanDau = reader["ChanDoanBanDau"] as string,
                PhuongAnDieuTri = reader["PhuongAnDieuTri"] as string,
                KetQuaDieuTri = reader["KetQuaDieuTri"] as string,
                ChanDoanRaVien = reader["ChanDoanRaVien"] as string,
                NgayLap = reader["NgayLap"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["NgayLap"],
                BacSiPhuTrachId = reader["BacSiPhuTrachId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BacSiPhuTrachId"]
            };
            
            // Safely map additional fields from JOIN (may not exist in all SPs)
            try
            {
                if (HasColumn(reader, "TenBenhNhan"))
                    dto.TenBenhNhan = reader["TenBenhNhan"] as string;
                if (HasColumn(reader, "NgaySinhBenhNhan"))
                    dto.NgaySinhBenhNhan = reader["NgaySinhBenhNhan"] == DBNull.Value ? null : (DateTime?)reader["NgaySinhBenhNhan"];
                if (HasColumn(reader, "BenhNhanId"))
                    dto.BenhNhanId = reader["BenhNhanId"] == DBNull.Value ? null : (Guid?)reader["BenhNhanId"];
                if (HasColumn(reader, "GioiTinh"))
                    dto.GioiTinh = reader["GioiTinh"] as string;
                if (HasColumn(reader, "DiaChi"))
                    dto.DiaChi = reader["DiaChi"] as string;
                if (HasColumn(reader, "SoTheBaoHiem"))
                    dto.SoTheBaoHiem = reader["SoTheBaoHiem"] as string;
                if (HasColumn(reader, "TenBacSi"))
                    dto.TenBacSi = reader["TenBacSi"] as string;
                if (HasColumn(reader, "NgayNhap"))
                    dto.NgayNhap = reader["NgayNhap"] == DBNull.Value ? null : (DateTime?)reader["NgayNhap"];
                if (HasColumn(reader, "NgayXuat"))
                    dto.NgayXuat = reader["NgayXuat"] == DBNull.Value ? null : (DateTime?)reader["NgayXuat"];
                if (HasColumn(reader, "LyDoNhap"))
                    dto.LyDoNhap = reader["LyDoNhap"] as string;
                if (HasColumn(reader, "TrangThaiNhapVien"))
                    dto.TrangThaiNhapVien = reader["TrangThaiNhapVien"] as string;
                if (HasColumn(reader, "TenKhoa"))
                    dto.TenKhoa = reader["TenKhoa"] as string;
                if (HasColumn(reader, "TenGiuong"))
                    dto.TenGiuong = reader["TenGiuong"] as string;
            }
            catch { /* Additional columns not found */ }
            
            return dto;
        }
        
        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}