using BacSiService.DAL.Interfaces;
using BacSiService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

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

        public IEnumerable<BacSi> GetAll()
        {
           
            if (!string.IsNullOrEmpty(_connectionString))
            {
                var list = new List<BacSi>();
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("sp_GetAllDoctors", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        // cache ordinals if columns exist
                        int idOrd = -1, hoTenOrd = -1, chuyenKhoaOrd = -1, thongTinOrd = -1;
                        if (reader.FieldCount > 0)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var name = reader.GetName(i);
                                if (string.Equals(name, "Id", StringComparison.OrdinalIgnoreCase)) idOrd = i;
                                else if (string.Equals(name, "HoTen", StringComparison.OrdinalIgnoreCase)) hoTenOrd = i;
                                else if (string.Equals(name, "ChuyenKhoa", StringComparison.OrdinalIgnoreCase)) chuyenKhoaOrd = i;
                                else if (string.Equals(name, "ThongTinLienHe", StringComparison.OrdinalIgnoreCase)) thongTinOrd = i;
                            }
                        }

                        while (reader.Read())
                        {
                            var d = new BacSi
                            {
                                Id = (idOrd >= 0 && !reader.IsDBNull(idOrd)) ? reader.GetGuid(idOrd) : Guid.Empty,
                                HoTen = (hoTenOrd >= 0 && !reader.IsDBNull(hoTenOrd)) ? reader.GetString(hoTenOrd) : null,
                                ChuyenKhoa = (chuyenKhoaOrd >= 0 && !reader.IsDBNull(chuyenKhoaOrd)) ? reader.GetString(chuyenKhoaOrd) : null,
                                ThongTinLienHe = (thongTinOrd >= 0 && !reader.IsDBNull(thongTinOrd)) ? reader.GetString(thongTinOrd) : null
                            };
                            list.Add(d);
                        }
                    }
                }
                return list;
            }
            return new List<BacSi>();
        }
    }
}
