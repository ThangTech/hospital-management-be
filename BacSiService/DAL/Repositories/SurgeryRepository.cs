using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BacSiService.DAL.Repositories
{
    public class SurgeryRepository : ISurgeryRepository
    {
        private readonly string? _connectionString;

        public SurgeryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? configuration["ConnectionStrings:DefaultConnection"];
        }

        public List<SurgeryScheduleDto> GetAll()
        {
            var result = new List<SurgeryScheduleDto>();
            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetAllSurgeries", conn))
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

        public PagedResult<SurgeryScheduleDto> Search(Guid? bacSiId, int pageNumber, int pageSize, string? searchTerm)
        {
            var result = new PagedResult<SurgeryScheduleDto>
            {
                Data = new List<SurgeryScheduleDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_SearchSurgeries", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BacSiId", bacSiId.HasValue ? (object)bacSiId.Value : DBNull.Value);
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

        public SurgeryScheduleDto? Create(SurgeryScheduleDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_CreateSurgery", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NhapVienId", dto.NhapVienId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BacSiChinhId", dto.BacSiChinhId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LoaiPhauThuat", dto.LoaiPhauThuat ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ekip", dto.Ekip ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ngay", dto.Ngay ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PhongMo", dto.PhongMo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", dto.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChiPhi", dto.ChiPhi ?? (object)DBNull.Value);
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

        public SurgeryScheduleDto? Update(Guid id, SurgeryScheduleDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateSurgery", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@BacSiChinhId", dto.BacSiChinhId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LoaiPhauThuat", dto.LoaiPhauThuat ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ekip", dto.Ekip ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ngay", dto.Ngay ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PhongMo", dto.PhongMo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TrangThai", dto.TrangThai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChiPhi", dto.ChiPhi ?? (object)DBNull.Value);
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
            using (var cmd = new SqlCommand("sp_DeleteSurgery", conn))
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

        private static SurgeryScheduleDto MapToDto(SqlDataReader reader)
        {
            var dto = new SurgeryScheduleDto
            {
                Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                NhapVienId = reader["NhapVienId"] == DBNull.Value ? (Guid?)null : (Guid)reader["NhapVienId"],
                BacSiChinhId = reader["BacSiChinhId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BacSiChinhId"],
                LoaiPhauThuat = reader["LoaiPhauThuat"] as string,
                Ekip = reader["Ekip"] as string,
                Ngay = reader["Ngay"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Ngay"],
                PhongMo = reader["PhongMo"] as string,
                TrangThai = reader["TrangThai"] as string
            };
            
            // Safely try to get patient info (requires SP to JOIN with NhapVien->BenhNhan)
            try
            {
                if (HasColumn(reader, "TenBenhNhan"))
                    dto.TenBenhNhan = reader["TenBenhNhan"] as string;
                if (HasColumn(reader, "BenhNhanId"))
                    dto.BenhNhanId = reader["BenhNhanId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BenhNhanId"];
                if (HasColumn(reader, "NgaySinhBenhNhan"))
                    dto.NgaySinhBenhNhan = reader["NgaySinhBenhNhan"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["NgaySinhBenhNhan"];
                if (HasColumn(reader, "ChiPhi"))
                    dto.ChiPhi = reader["ChiPhi"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["ChiPhi"]);
                if (HasColumn(reader, "TenBacSi"))
                    dto.TenBacSi = reader["TenBacSi"] as string;
            }
            catch { /* Columns not found - SP needs update */ }
            
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
