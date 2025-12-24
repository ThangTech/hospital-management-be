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

        // Single constructor: accept both DbContext and IConfiguration (both provided by DI)
        public DoctorRepository(HospitalManageContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultsConnection");
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
                            ThongTinLienHe = reader["ThongTinLienHe"] as string
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
                            ThongTinLienHe = reader["ThongTinLienHe"] as string
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
                            ThongTinLienHe = reader["ThongTinLienHe"] as string
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
                            ThongTinLienHe = reader["ThongTinLienHe"] as string
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
                return rows > 0;
            }
        }


    }
}
