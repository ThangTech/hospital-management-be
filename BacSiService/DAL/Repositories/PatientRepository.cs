using BacSiService.DAL.Interfaces;
using BacSiService.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BacSiService.DAL.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string? _connectionString;
        public PatientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["ConnectionStrings:DefaultConnection"];
        }

        public PagedResult<PatientLookupDto> Lookup(string? term, int pageNumber, int pageSize)
        {
            var result = new PagedResult<PatientLookupDto> { Data = new List<PatientLookupDto>(), PageNumber = pageNumber, PageSize = pageSize };
            if (string.IsNullOrEmpty(_connectionString)) return result;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_LookupPatients", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Term", string.IsNullOrEmpty(term) ? (object)DBNull.Value : term);
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                var totalParam = new SqlParameter("@TotalRecords", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(totalParam);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dto = new PatientLookupDto
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            HoTen = reader["HoTen"] as string,
                            SoTheBaoHiem = reader["SoTheBaoHiem"] as string,
                            DiaChi = reader["DiaChi"] as string,
                            GioiTinh = reader["GioiTinh"] as string
                        };
                        result.Data.Add(dto);
                    }
                }

                result.TotalRecords = (int)(totalParam.Value ?? 0);
                result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / pageSize);
            }

            return result;
        }
    }
}
