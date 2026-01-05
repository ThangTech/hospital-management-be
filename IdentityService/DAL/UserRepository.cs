using System.Data;
using IdentityService.Models;
using Microsoft.Data.SqlClient;

namespace IdentityService.DAL;

/// <summary>
/// Repository quản lý NguoiDung sử dụng Stored Procedures
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("ConnectionString không được cấu hình");
    }

    /// <summary>
    /// Lấy user theo ID
    /// </summary>
    public async Task<NguoiDung?> GetByIdAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_GetNguoiDungByIdFull", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToNguoiDung(reader);
        }

        return null;
    }

    /// <summary>
    /// Lấy user theo tên đăng nhập
    /// </summary>
    public async Task<NguoiDung?> GetByTenDangNhapAsync(string tenDangNhap)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_GetNguoiDungByTenDangNhapFull", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToNguoiDung(reader);
        }

        return null;
    }

    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại chưa
    /// </summary>
    public async Task<bool> ExistsAsync(string tenDangNhap)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_CheckNguoiDungExists", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

        var existsParam = new SqlParameter("@Exists", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(existsParam);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();

        return (bool)existsParam.Value;
    }

    /// <summary>
    /// Thêm user mới
    /// </summary>
    public async Task<NguoiDung> CreateAsync(NguoiDung nguoiDung)
    {
        nguoiDung.Id = Guid.NewGuid();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_CreateNguoiDungFull", connection);
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@Id", nguoiDung.Id);
        command.Parameters.AddWithValue("@TenDangNhap", nguoiDung.TenDangNhap ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@MatKhauHash", nguoiDung.MatKhauHash ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@VaiTro", nguoiDung.VaiTro ?? (object)DBNull.Value);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToNguoiDung(reader);
        }

        return nguoiDung;
    }

    /// <summary>
    /// Cập nhật user
    /// </summary>
    public async Task<NguoiDung> UpdateAsync(NguoiDung nguoiDung)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_UpdateNguoiDungFull", connection);
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@Id", nguoiDung.Id);
        command.Parameters.AddWithValue("@TenDangNhap", nguoiDung.TenDangNhap ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@MatKhauHash", nguoiDung.MatKhauHash ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@VaiTro", nguoiDung.VaiTro ?? (object)DBNull.Value);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToNguoiDung(reader);
        }

        return nguoiDung;
    }

    /// <summary>
    /// Lấy tất cả users (Admin only)
    /// </summary>
    public async Task<IEnumerable<NguoiDung>> GetAllAsync()
    {
        var users = new List<NguoiDung>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_GetAllNguoiDungFull", connection);
        command.CommandType = CommandType.StoredProcedure;

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(MapToNguoiDung(reader));
        }

        return users;
    }

    #region Helper Methods

    /// <summary>
    /// Map SqlDataReader sang NguoiDung object
    /// </summary>
    private static NguoiDung MapToNguoiDung(SqlDataReader reader)
    {
        return new NguoiDung
        {
            Id = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
            TenDangNhap = reader["TenDangNhap"] as string,
            MatKhauHash = reader["MatKhauHash"] as string,
            VaiTro = reader["VaiTro"] as string
        };
    }

    #endregion
}
