using AdminService.DTOs;
using Microsoft.Data.SqlClient;
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

        var whereClause = "WHERE 1=1";
        if (!string.IsNullOrEmpty(search.TenDangNhap))
            whereClause += " AND TenDangNhap LIKE @TenDangNhap";
        if (!string.IsNullOrEmpty(search.VaiTro))
            whereClause += " AND VaiTro = @VaiTro";

        // Count
        var countSql = $"SELECT COUNT(*) FROM NguoiDung {whereClause}";
        using (var countCmd = new SqlCommand(countSql, conn))
        {
            if (!string.IsNullOrEmpty(search.TenDangNhap))
                countCmd.Parameters.AddWithValue("@TenDangNhap", $"%{search.TenDangNhap}%");
            if (!string.IsNullOrEmpty(search.VaiTro))
                countCmd.Parameters.AddWithValue("@VaiTro", search.VaiTro);

            conn.Open();
            result.TotalRecords = (int)countCmd.ExecuteScalar();
            result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / search.PageSize);
        }

        // Data
        var offset = (search.PageNumber - 1) * search.PageSize;
        var dataSql = $@"
            SELECT Id, TenDangNhap, VaiTro FROM NguoiDung {whereClause}
            ORDER BY TenDangNhap
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using (var dataCmd = new SqlCommand(dataSql, conn))
        {
            if (!string.IsNullOrEmpty(search.TenDangNhap))
                dataCmd.Parameters.AddWithValue("@TenDangNhap", $"%{search.TenDangNhap}%");
            if (!string.IsNullOrEmpty(search.VaiTro))
                dataCmd.Parameters.AddWithValue("@VaiTro", search.VaiTro);
            dataCmd.Parameters.AddWithValue("@Offset", offset);
            dataCmd.Parameters.AddWithValue("@PageSize", search.PageSize);

            using var reader = dataCmd.ExecuteReader();
            while (reader.Read())
            {
                result.Data.Add(MapToUserDTO(reader));
            }
        }

        return result;
    }

    public UserDTO? GetById(Guid id)
    {
        if (string.IsNullOrEmpty(_connectionString)) return null;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT Id, TenDangNhap, VaiTro FROM NguoiDung WHERE Id = @Id", conn);
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
        using var cmd = new SqlCommand("SELECT Id, TenDangNhap, VaiTro FROM NguoiDung WHERE TenDangNhap = @TenDangNhap", conn);
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
        using var cmd = new SqlCommand("SELECT COUNT(1) FROM NguoiDung WHERE TenDangNhap = @TenDangNhap", conn);
        cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

        conn.Open();
        return (int)cmd.ExecuteScalar() > 0;
    }

    public UserDTO Create(CreateUserDTO dto)
    {
        if (string.IsNullOrEmpty(_connectionString)) 
            throw new Exception("Connection string not configured");

        var id = Guid.NewGuid();
        var hashedPassword = HashPassword(dto.MatKhau);

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"
            INSERT INTO NguoiDung (Id, TenDangNhap, MatKhauHash, VaiTro)
            VALUES (@Id, @TenDangNhap, @MatKhauHash, @VaiTro)", conn);

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@TenDangNhap", dto.TenDangNhap);
        cmd.Parameters.AddWithValue("@MatKhauHash", hashedPassword);
        cmd.Parameters.AddWithValue("@VaiTro", dto.VaiTro);

        conn.Open();
        cmd.ExecuteNonQuery();

        return new UserDTO { Id = id, TenDangNhap = dto.TenDangNhap, VaiTro = dto.VaiTro };
    }

    public UserDTO? Update(UpdateUserDTO dto)
    {
        if (string.IsNullOrEmpty(_connectionString)) return null;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"
            UPDATE NguoiDung SET TenDangNhap = @TenDangNhap, VaiTro = @VaiTro 
            WHERE Id = @Id", conn);

        cmd.Parameters.AddWithValue("@Id", dto.Id);
        cmd.Parameters.AddWithValue("@TenDangNhap", dto.TenDangNhap ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@VaiTro", dto.VaiTro ?? (object)DBNull.Value);

        conn.Open();
        var rows = cmd.ExecuteNonQuery();
        
        if (rows > 0)
            return GetById(dto.Id);

        return null;
    }

    public bool Delete(Guid id)
    {
        if (string.IsNullOrEmpty(_connectionString)) return false;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("DELETE FROM NguoiDung WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        conn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool ResetPassword(Guid userId, string newPassword)
    {
        if (string.IsNullOrEmpty(_connectionString)) return false;

        var hashedPassword = HashPassword(newPassword);

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("UPDATE NguoiDung SET MatKhauHash = @MatKhauHash WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", userId);
        cmd.Parameters.AddWithValue("@MatKhauHash", hashedPassword);

        conn.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public List<UserDTO> GetAll()
    {
        var result = new List<UserDTO>();
        if (string.IsNullOrEmpty(_connectionString)) return result;

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT Id, TenDangNhap, VaiTro FROM NguoiDung ORDER BY TenDangNhap", conn);

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
