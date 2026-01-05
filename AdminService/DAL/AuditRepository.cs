using AdminService.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdminService.DAL;

public class AuditRepository : IAuditRepository
{
    private readonly string? _connectionString;

    public AuditRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public PagedResult<NhatKyHeThongDTO> GetNhatKyHeThong(AuditSearchDTO search)
    {
        var result = new PagedResult<NhatKyHeThongDTO>
        {
            Data = new List<NhatKyHeThongDTO>(),
            PageNumber = search.PageNumber,
            PageSize = search.PageSize
        };

        if (string.IsNullOrEmpty(_connectionString)) return result;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_GetNhatKyHeThong", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        // Input parameters
        cmd.Parameters.AddWithValue("@NguoiDungId", (object?)search.NguoiDungId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@HanhDong", (object?)search.HanhDong ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TuNgay", (object?)search.TuNgay ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DenNgay", (object?)search.DenNgay ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PageNumber", search.PageNumber);
        cmd.Parameters.AddWithValue("@PageSize", search.PageSize);

        // Output parameter
        var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(totalRecordsParam);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Data.Add(new NhatKyHeThongDTO
            {
                Id = (Guid)reader["Id"],
                NguoiDungId = reader["NguoiDungId"] as Guid?,
                TenNguoiDung = reader["TenNguoiDung"] as string,
                HanhDong = reader["HanhDong"] as string,
                ThoiGian = reader["ThoiGian"] as DateTime?,
                MoTa = reader["MoTa"] as string
            });
        }

        reader.Close();
        result.TotalRecords = (int)totalRecordsParam.Value;
        result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / search.PageSize);

        return result;
    }

    public PagedResult<AuditHoSoBenhAnDTO> GetAuditHoSoBenhAn(AuditSearchDTO search)
    {
        var result = new PagedResult<AuditHoSoBenhAnDTO>
        {
            Data = new List<AuditHoSoBenhAnDTO>(),
            PageNumber = search.PageNumber,
            PageSize = search.PageSize
        };

        if (string.IsNullOrEmpty(_connectionString)) return result;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_GetAuditHoSoBenhAn", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        // Input parameters
        cmd.Parameters.AddWithValue("@NguoiDungId", (object?)search.NguoiDungId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@HanhDong", (object?)search.HanhDong ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TuNgay", (object?)search.TuNgay ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DenNgay", (object?)search.DenNgay ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PageNumber", search.PageNumber);
        cmd.Parameters.AddWithValue("@PageSize", search.PageSize);

        // Output parameter
        var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(totalRecordsParam);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Data.Add(new AuditHoSoBenhAnDTO
            {
                Id = (Guid)reader["Id"],
                HoSoBenhAnId = reader["HoSoBenhAnId"] as Guid?,
                HanhDong = reader["HanhDong"] as string,
                ChanDoanCu = reader["ChanDoanCu"] as string,
                KetQuaCu = reader["KetQuaCu"] as string,
                NguoiDungId = reader["NguoiDungId"] as Guid?,
                TenNguoiSua = reader["TenNguoiSua"] as string,
                ThoiGianSua = reader["ThoiGianSua"] as DateTime?
            });
        }

        reader.Close();
        result.TotalRecords = (int)totalRecordsParam.Value;
        result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / search.PageSize);

        return result;
    }
}
