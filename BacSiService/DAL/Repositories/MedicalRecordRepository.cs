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
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["ConnectionStrings:DefaultConnection"];
        }

        public PagedResult<MedicalRecordDto> GetByPatient(Guid? patientId, int pageNumber, int pageSize, string? searchTerm)
        {
            var result = new PagedResult<MedicalRecordDto> { Data = new List<MedicalRecordDto>(), PageNumber = pageNumber, PageSize = pageSize };
            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetMedicalRecordsByPatient", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PatientId", patientId.HasValue ? (object)patientId.Value : DBNull.Value);
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
                        var dto = new MedicalRecordDto
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            NhapVienId = reader["NhapVienId"] == DBNull.Value ? (Guid?)null : (Guid)reader["NhapVienId"],
                            BenhNhanId = reader["BenhNhanId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BenhNhanId"],
                            BacSiId = reader["BacSiId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BacSiId"],
                            DieuDuongId = reader["DieuDuongId"] == DBNull.Value ? (Guid?)null : (Guid)reader["DieuDuongId"],
                            KhoaId = reader["KhoaId"] == DBNull.Value ? (Guid?)null : (Guid)reader["KhoaId"],
                            MoTa = reader["MoTa"] as string,
                            Ngay = reader["Ngay"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Ngay"]
                        };
                        result.Data.Add(dto);
                    }
                }

                result.TotalRecords = (int)(totalParam.Value ?? 0);
                result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / pageSize);
            }

            return result;
        }

        public MedicalRecordDto? Create(MedicalRecordDto dto, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_CreateMedicalRecord", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NhapVienId", dto.NhapVienId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BenhNhanId", dto.BenhNhanId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BacSiId", dto.BacSiId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DieuDuongId", dto.DieuDuongId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KhoaId", dto.KhoaId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MoTa", dto.MoTa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ngay", dto.Ngay ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AuditUser", auditUser ?? (object)DBNull.Value);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MedicalRecordDto
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            NhapVienId = reader["NhapVienId"] == DBNull.Value ? (Guid?)null : (Guid)reader["NhapVienId"],
                            BenhNhanId = reader["BenhNhanId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BenhNhanId"],
                            BacSiId = reader["BacSiId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BacSiId"],
                            DieuDuongId = reader["DieuDuongId"] == DBNull.Value ? (Guid?)null : (Guid)reader["DieuDuongId"],
                            KhoaId = reader["KhoaId"] == DBNull.Value ? (Guid?)null : (Guid)reader["KhoaId"],
                            MoTa = reader["MoTa"] as string,
                            Ngay = reader["Ngay"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Ngay"]
                        };
                    }
                }
            }
            return null;
        }

        public MedicalRecordDto? Update(Guid id, MedicalRecordDto dto, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateMedicalRecord", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@MoTa", dto.MoTa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Ngay", dto.Ngay ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AuditUser", auditUser ?? (object)DBNull.Value);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MedicalRecordDto
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            NhapVienId = reader["NhapVienId"] == DBNull.Value ? (Guid?)null : (Guid)reader["NhapVienId"],
                            BenhNhanId = reader["BenhNhanId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BenhNhanId"],
                            BacSiId = reader["BacSiId"] == DBNull.Value ? (Guid?)null : (Guid)reader["BacSiId"],
                            DieuDuongId = reader["DieuDuongId"] == DBNull.Value ? (Guid?)null : (Guid)reader["DieuDuongId"],
                            KhoaId = reader["KhoaId"] == DBNull.Value ? (Guid?)null : (Guid)reader["KhoaId"],
                            MoTa = reader["MoTa"] as string,
                            Ngay = reader["Ngay"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["Ngay"]
                        };
                    }
                }
            }
            return null;
        }

        public bool Delete(Guid id, string? auditUser = null)
        {
            if (string.IsNullOrEmpty(_connectionString)) return false;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_DeleteMedicalRecord", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@AuditUser", auditUser ?? (object)DBNull.Value);
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows == -1)
                {
                    return true;
                }
                return rows > 0;
            }
        }
    }
}
