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
        
        // Build WHERE clause
        var whereClause = "WHERE 1=1";
        if (search.NguoiDungId.HasValue)
            whereClause += " AND nk.NguoiDungId = @NguoiDungId";
        if (!string.IsNullOrEmpty(search.HanhDong))
            whereClause += " AND nk.HanhDong LIKE @HanhDong";
        if (search.TuNgay.HasValue)
            whereClause += " AND nk.ThoiGian >= @TuNgay";
        if (search.DenNgay.HasValue)
            whereClause += " AND nk.ThoiGian <= @DenNgay";

        // Count total
        var countSql = $@"
            SELECT COUNT(*) FROM NhatKyHeThong nk 
            LEFT JOIN NguoiDung nd ON nk.NguoiDungId = nd.Id 
            {whereClause}";

        using (var countCmd = new SqlCommand(countSql, conn))
        {
            AddSearchParams(countCmd, search);
            conn.Open();
            result.TotalRecords = (int)countCmd.ExecuteScalar();
            result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / search.PageSize);
        }

        // Get data with paging
        var offset = (search.PageNumber - 1) * search.PageSize;
        var dataSql = $@"
            SELECT nk.Id, nk.NguoiDungId, nd.TenDangNhap AS TenNguoiDung, 
                   nk.HanhDong, nk.ThoiGian, nk.MoTa
            FROM NhatKyHeThong nk
            LEFT JOIN NguoiDung nd ON nk.NguoiDungId = nd.Id
            {whereClause}
            ORDER BY nk.ThoiGian DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using (var dataCmd = new SqlCommand(dataSql, conn))
        {
            AddSearchParams(dataCmd, search);
            dataCmd.Parameters.AddWithValue("@Offset", offset);
            dataCmd.Parameters.AddWithValue("@PageSize", search.PageSize);

            using var reader = dataCmd.ExecuteReader();
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
        }

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

        var whereClause = "WHERE 1=1";
        if (search.NguoiDungId.HasValue)
            whereClause += " AND a.NguoiDungId = @NguoiDungId";
        if (!string.IsNullOrEmpty(search.HanhDong))
            whereClause += " AND a.HanhDong LIKE @HanhDong";
        if (search.TuNgay.HasValue)
            whereClause += " AND a.ThoiGianSua >= @TuNgay";
        if (search.DenNgay.HasValue)
            whereClause += " AND a.ThoiGianSua <= @DenNgay";

        // Count total
        var countSql = $@"
            SELECT COUNT(*) FROM Audit_HoSoBenhAn a 
            LEFT JOIN NguoiDung nd ON a.NguoiDungId = nd.Id 
            {whereClause}";

        using (var countCmd = new SqlCommand(countSql, conn))
        {
            AddSearchParams(countCmd, search);
            conn.Open();
            result.TotalRecords = (int)countCmd.ExecuteScalar();
            result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / search.PageSize);
        }

        var offset = (search.PageNumber - 1) * search.PageSize;
        var dataSql = $@"
            SELECT a.Id, a.HoSoBenhAnId, a.HanhDong, a.ChanDoanCu, a.KetQuaCu, 
                   a.NguoiDungId, COALESCE(nd.TenDangNhap, a.NguoiSua) AS TenNguoiSua, a.ThoiGianSua
            FROM Audit_HoSoBenhAn a
            LEFT JOIN NguoiDung nd ON a.NguoiDungId = nd.Id
            {whereClause}
            ORDER BY a.ThoiGianSua DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using (var dataCmd = new SqlCommand(dataSql, conn))
        {
            AddSearchParams(dataCmd, search);
            dataCmd.Parameters.AddWithValue("@Offset", offset);
            dataCmd.Parameters.AddWithValue("@PageSize", search.PageSize);

            using var reader = dataCmd.ExecuteReader();
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
        }

        return result;
    }

    private static void AddSearchParams(SqlCommand cmd, AuditSearchDTO search)
    {
        if (search.NguoiDungId.HasValue)
            cmd.Parameters.AddWithValue("@NguoiDungId", search.NguoiDungId.Value);
        if (!string.IsNullOrEmpty(search.HanhDong))
            cmd.Parameters.AddWithValue("@HanhDong", $"%{search.HanhDong}%");
        if (search.TuNgay.HasValue)
            cmd.Parameters.AddWithValue("@TuNgay", search.TuNgay.Value);
        if (search.DenNgay.HasValue)
            cmd.Parameters.AddWithValue("@DenNgay", search.DenNgay.Value);
    }
}
