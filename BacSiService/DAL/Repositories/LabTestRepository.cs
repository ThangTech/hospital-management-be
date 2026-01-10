using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BacSiService.DAL.Repositories
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly string? _connectionString;

        public LabTestRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? configuration["ConnectionStrings:DefaultConnection"];
        }

        public List<LabTestDto> GetAll()
        {
            var result = new List<LabTestDto>();
            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetAllLabTests", conn))
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

        public PagedResult<LabTestDto> Search(Guid? nhapVienId, int pageNumber, int pageSize, string? searchTerm)
        {
            var result = new PagedResult<LabTestDto>
            {
                Data = new List<LabTestDto>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_SearchLabTests", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NhapVienId", nhapVienId.HasValue ? (object)nhapVienId.Value : DBNull.Value);
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

        public LabTestDto? Create(LabTestDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_CreateLabTest", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NhapVienId", dto.NhapVienId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BacSiId", dto.BacSiId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LoaiXetNghiem", dto.LoaiXetNghiem ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KetQua", dto.KetQua ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ngay", dto.Ngay ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DonGia", dto.DonGia ?? (object)DBNull.Value);
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

        public LabTestDto? Update(Guid id, LabTestDto dto, Guid? nguoiDungId = null, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateLabTest", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@LoaiXetNghiem", dto.LoaiXetNghiem ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KetQua", dto.KetQua ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ngay", dto.Ngay ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DonGia", dto.DonGia ?? (object)DBNull.Value);
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
            using (var cmd = new SqlCommand("sp_DeleteLabTest", conn))
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

        private static LabTestDto MapToDto(SqlDataReader reader)
        {
            var dto = new LabTestDto
            {
                Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                NhapVienId = reader["NhapVienId"] == DBNull.Value ? (Guid?)null : (Guid)reader["NhapVienId"],
                BacSiId = reader["BacSiId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BacSiId"],
                LoaiXetNghiem = reader["LoaiXetNghiem"] as string,
                KetQua = reader["KetQua"] as string,
                Ngay = reader["Ngay"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Ngay"],
                DonGia = HasColumn(reader, "DonGia") && reader["DonGia"] != DBNull.Value ? (decimal)reader["DonGia"] : (decimal?)null,
                TenBenhNhan = HasColumn(reader, "TenBenhNhan") ? reader["TenBenhNhan"] as string : null,
                // Map other potential fields if the SP returns them
            };
            return dto;
        }

        private static bool HasColumn(SqlDataReader r, string columnName)
        {
            try
            {
                return r.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}
