using AdminService.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace AdminService.DAL;

public class UserManagementRepository : IUserManagementRepository
{
    private readonly string? _connectionString;

    public UserManagementRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public PagedResult<UserDTO> Search(UserSearchDTO search)
    {
        var result = new PagedResult<UserDTO>
        {
            Data = new List<UserDTO>(),
            PageNumber = search.PageNumber,
            PageSize = search.PageSize
        };

        if (string.IsNullOrEmpty(_connectionString)) return result;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_SearchNguoiDung", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@TenDangNhap", (object?)search.TenDangNhap ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@VaiTro", (object?)search.VaiTro ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PageNumber", search.PageNumber);
        cmd.Parameters.AddWithValue("@PageSize", search.PageSize);

        var totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(totalRecordsParam);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Data.Add(MapToUserDTO(reader));
        }

        reader.Close();
        result.TotalRecords = (int)totalRecordsParam.Value;
        result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / search.PageSize);

        return result;
    }

    public UserDTO? GetById(Guid id)
    {
        if (string.IsNullOrEmpty(_connectionString)) return null;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_GetNguoiDungById", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToUserDTO(reader);

        return null;
    }

    public UserDTO? GetByTenDangNhap(string tenDangNhap)
    {
        if (string.IsNullOrEmpty(_connectionString)) return null;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_GetNguoiDungByTenDangNhap", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToUserDTO(reader);

        return null;
    }

    public bool Exists(string tenDangNhap)
    {
        if (string.IsNullOrEmpty(_connectionString)) return false;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_CheckNguoiDungExists", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

        var existsParam = new SqlParameter("@Exists", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(existsParam);

        conn.Open();
        cmd.ExecuteNonQuery();

        return (bool)existsParam.Value;
    }

    public UserDTO Create(CreateUserDTO dto)
    {
        if (string.IsNullOrEmpty(_connectionString)) 
            throw new Exception("Connection string not configured");

        var id = Guid.NewGuid();
        var hashedPassword = HashPassword(dto.MatKhau);

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_CreateNguoiDung", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@TenDangNhap", dto.TenDangNhap);
        cmd.Parameters.AddWithValue("@MatKhauHash", hashedPassword);
        cmd.Parameters.AddWithValue("@VaiTro", dto.VaiTro);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToUserDTO(reader);

        return new UserDTO { Id = id, TenDangNhap = dto.TenDangNhap, VaiTro = dto.VaiTro };
    }

    public UserDTO? Update(UpdateUserDTO dto)
    {
        if (string.IsNullOrEmpty(_connectionString)) return null;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_UpdateNguoiDung", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", dto.Id);
        cmd.Parameters.AddWithValue("@TenDangNhap", (object?)dto.TenDangNhap ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@VaiTro", (object?)dto.VaiTro ?? DBNull.Value);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return MapToUserDTO(reader);

        return null;
    }

    public bool Delete(Guid id)
    {
        if (string.IsNullOrEmpty(_connectionString)) return false;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_DeleteNguoiDung", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);

        var rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(rowsAffectedParam);

        conn.Open();
        cmd.ExecuteNonQuery();

        return (int)rowsAffectedParam.Value > 0;
    }

    public bool ResetPassword(Guid userId, string newPassword)
    {
        if (string.IsNullOrEmpty(_connectionString)) return false;

        var hashedPassword = HashPassword(newPassword);

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_ResetPassword", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", userId);
        cmd.Parameters.AddWithValue("@MatKhauHash", hashedPassword);

        var rowsAffectedParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(rowsAffectedParam);

        conn.Open();
        cmd.ExecuteNonQuery();

        return (int)rowsAffectedParam.Value > 0;
    }

    public List<UserDTO> GetAll()
    {
        var result = new List<UserDTO>();
        if (string.IsNullOrEmpty(_connectionString)) return result;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("sp_GetAllNguoiDung", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(MapToUserDTO(reader));
        }

        return result;
    }

    private static UserDTO MapToUserDTO(SqlDataReader reader)
    {
        return new UserDTO
        {
            Id = (Guid)reader["Id"],
            TenDangNhap = reader["TenDangNhap"] as string,
            VaiTro = reader["VaiTro"] as string
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
