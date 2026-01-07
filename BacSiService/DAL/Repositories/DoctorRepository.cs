using BacSiService.DAL.Interfaces;
using BacSiService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using BacSiService.DTOs;

namespace BacSiService.DAL.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly HospitalManageContext? _context;
        private readonly string? _connectionString;

        public DoctorRepository(HospitalManageContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? configuration["ConnectionStrings:DefaultConnection"];
        }

        public BacSi? CreateDoctor(DoctorDto doctorDto)
        {
            if (string.IsNullOrEmpty(_connectionString))
                return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_CreateDoctor", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@HoTen", doctorDto.HoTen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChuyenKhoa", doctorDto.ChuyenKhoa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ThongTinLienHe", doctorDto.ThongTinLienHe ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KhoaId", doctorDto.KhoaId.HasValue ? (object)doctorDto.KhoaId.Value : DBNull.Value);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BacSi
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            HoTen = reader["HoTen"] as string,
                            ChuyenKhoa = reader["ChuyenKhoa"] as string,
                            ThongTinLienHe = reader["ThongTinLienHe"] as string,
                            KhoaId = reader["KhoaId"] == DBNull.Value ? null : (Guid?)reader["KhoaId"]
                        };
                    }
                }
            }
            return null;
        }

        public IEnumerable<BacSi> GetAll()
        {
            if (string.IsNullOrEmpty(_connectionString))
                return new List<BacSi>();

            var list = new List<BacSi>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetAllDoctors", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var doctor = new BacSi
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            HoTen = reader["HoTen"] as string,
                            ChuyenKhoa = reader["ChuyenKhoa"] as string,
                            ThongTinLienHe = reader["ThongTinLienHe"] as string,
                            KhoaId = reader["KhoaId"] == DBNull.Value ? null : (Guid?)reader["KhoaId"]
                        };
                        list.Add(doctor);
                    }
                }
            }
            return list;
        }

        public BacSi? GetById(Guid id)
        {
            if (string.IsNullOrEmpty(_connectionString))
                return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetDoctorById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BacSi
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            HoTen = reader["HoTen"] as string,
                            ChuyenKhoa = reader["ChuyenKhoa"] as string,
                            ThongTinLienHe = reader["ThongTinLienHe"] as string,
                            KhoaId = reader["KhoaId"] == DBNull.Value ? null : (Guid?)reader["KhoaId"]
                        };
                    }
                }
            }
            return null;
        }

        public BacSi? UpdateDoctor(Guid id, DoctorUpdateDTO doctorUpdateDTO)
        {
            if (string.IsNullOrEmpty(_connectionString))
                return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_UpdateDoctor", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@HoTen", doctorUpdateDTO.HoTen ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ChuyenKhoa", doctorUpdateDTO.ChuyenKhoa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ThongTinLienHe", doctorUpdateDTO.ThongTinLienHe ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@KhoaId", doctorUpdateDTO.KhoaId.HasValue ? (object)doctorUpdateDTO.KhoaId.Value : DBNull.Value);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BacSi
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            HoTen = reader["HoTen"] as string,
                            ChuyenKhoa = reader["ChuyenKhoa"] as string,
                            ThongTinLienHe = reader["ThongTinLienHe"] as string,
                            KhoaId = reader["KhoaId"] == DBNull.Value ? null : (Guid?)reader["KhoaId"]
                        };
                    }
                }
            }
            return null;
        }

        public bool DeleteDoctor(Guid id)
        {
            if (string.IsNullOrEmpty(_connectionString))
                return false;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_DeleteDoctor", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();
                if (rows == -1)
                {
                    return true;
                }
                return rows > 0;
            }
        }

        public PagedResult<BacSi> SearchDoctors(SearchRequestDTO request)
        {
            if (string.IsNullOrEmpty(_connectionString))
                return new PagedResult<BacSi> { Data = new List<BacSi>() };

            var result = new PagedResult<BacSi>
            {
                Data = new List<BacSi>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_SearchDoctors", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SearchTerm",
                    string.IsNullOrEmpty(request.SearchTerm) ? (object)DBNull.Value : request.SearchTerm);
                cmd.Parameters.AddWithValue("@HoTen",
                    string.IsNullOrEmpty(request.HoTen) ? (object)DBNull.Value : request.HoTen);
                cmd.Parameters.AddWithValue("@ChuyenKhoa",
                    string.IsNullOrEmpty(request.ChuyenKhoa) ? (object)DBNull.Value : request.ChuyenKhoa);
                cmd.Parameters.AddWithValue("@ThongTinLienHe",
                    string.IsNullOrEmpty(request.ThongTinLienHe) ? (object)DBNull.Value : request.ThongTinLienHe);
                cmd.Parameters.AddWithValue("@KhoaId",
                    request.KhoaId != Guid.Empty ? (object)request.KhoaId : DBNull.Value);
                cmd.Parameters.AddWithValue("@SortBy",
                    string.IsNullOrEmpty(request.SortBy) ? "HoTen" : request.SortBy);
                cmd.Parameters.AddWithValue("@SortOrder",
                    string.IsNullOrEmpty(request.SortOrder) ? "ASC" : request.SortOrder);
                cmd.Parameters.AddWithValue("@PageNumber", request.PageNumber);
                cmd.Parameters.AddWithValue("@PageSize", request.PageSize);

                var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(totalRecordsParam);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var doctor = new BacSi
                        {
                            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
                            HoTen = reader["HoTen"] as string,
                            ChuyenKhoa = reader["ChuyenKhoa"] as string,
                            ThongTinLienHe = reader["ThongTinLienHe"] as string,
                            KhoaId = reader["KhoaId"] == DBNull.Value ? null : (Guid?)reader["KhoaId"]
                        };
                        result.Data.Add(doctor);
                    }
                }

                result.TotalRecords = (int)totalRecordsParam.Value;
                result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / request.PageSize);
            }

            return result;
        }
    }
}