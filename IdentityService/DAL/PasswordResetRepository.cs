using System.Data;
using System.Security.Cryptography;
using IdentityService.Models;
using Microsoft.Data.SqlClient;

namespace IdentityService.DAL;

/// <summary>
/// Repository quản lý OTP và ResetToken cho luồng quên mật khẩu
/// Sử dụng Stored Procedures + ADO.NET
/// </summary>
public class PasswordResetRepository : IPasswordResetRepository
{
    private readonly string _connectionString;
    private const int MaxOtpAttempts = 5;

    public PasswordResetRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("ConnectionString không được cấu hình");
    }

    /// <summary>
    /// Tạo OTP 6 số mới, tự động vô hiệu OTP cũ
    /// </summary>
    public async Task<string> CreateOtpAsync(Guid userId)
    {
        var otpCode = GenerateOtp();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_CreatePasswordResetOtp", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@OtpCode", otpCode);
        command.Parameters.AddWithValue("@ExpiryMinutes", 5);

        await connection.OpenAsync();
        await command.ExecuteReaderAsync();

        return otpCode;
    }

    /// <summary>
    /// Lấy OTP active của user (chưa dùng, chưa hết hạn)
    /// </summary>
    public async Task<PasswordResetToken?> GetActiveOtpAsync(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_GetActiveOtp", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToPasswordResetToken(reader);
        }

        return null;
    }

    /// <summary>
    /// Tăng số lần thử OTP sai, trả về số lần đã thử
    /// </summary>
    public async Task<int> IncrementAttemptAsync(Guid tokenId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_IncrementOtpAttempt", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TokenId", tokenId);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return reader["AttemptCount"] == DBNull.Value ? 0 : (int)reader["AttemptCount"];
        }

        return 0;
    }

    /// <summary>
    /// Vô hiệu một token/OTP cụ thể
    /// </summary>
    public async Task InvalidateTokenAsync(Guid tokenId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_InvalidateToken", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TokenId", tokenId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Vô hiệu tất cả token/OTP active của user
    /// </summary>
    public async Task InvalidateAllUserTokensAsync(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_InvalidateAllUserTokens", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Tạo ResetToken ngẫu nhiên sau khi OTP xác thực thành công
    /// </summary>
    public async Task<string> CreateResetTokenAsync(Guid userId)
    {
        var resetToken = GenerateSecureToken();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_CreateResetToken", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@ResetToken", resetToken);
        command.Parameters.AddWithValue("@ExpiryMinutes", 10);

        await connection.OpenAsync();
        await command.ExecuteReaderAsync();

        return resetToken;
    }

    /// <summary>
    /// Lấy ResetToken chưa dùng, chưa hết hạn
    /// </summary>
    public async Task<PasswordResetToken?> GetActiveResetTokenAsync(string resetToken)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_GetActiveResetToken", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@ResetToken", resetToken);

        await connection.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapToPasswordResetToken(reader);
        }

        return null;
    }

    /// <summary>
    /// Đánh dấu ResetToken đã sử dụng (1 lần dùng duy nhất)
    /// </summary>
    public async Task MarkTokenUsedAsync(Guid tokenId)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("sp_MarkTokenUsed", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@TokenId", tokenId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    #region Private Helpers

    /// <summary>
    /// Tạo OTP 6 số ngẫu nhiên (cryptographically secure)
    /// </summary>
    private static string GenerateOtp()
    {
        // Dùng RandomNumberGenerator để tránh PRNG bias
        var bytes = new byte[4];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        var number = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
        return number.ToString("D6"); // Luôn đủ 6 chữ số, ví dụ: 042819
    }

    /// <summary>
    /// Tạo ResetToken ngẫu nhiên 256-bit (URL-safe base64)
    /// </summary>
    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('='); // URL-safe
    }

    /// <summary>
    /// Map SqlDataReader sang PasswordResetToken
    /// </summary>
    private static PasswordResetToken MapToPasswordResetToken(SqlDataReader reader)
    {
        return new PasswordResetToken
        {
            Id          = reader["Id"] == DBNull.Value ? Guid.Empty : (Guid)reader["Id"],
            UserId      = reader["UserId"] == DBNull.Value ? Guid.Empty : (Guid)reader["UserId"],
            OtpCode     = reader.HasColumn("OtpCode") ? reader["OtpCode"] as string : null,
            ResetToken  = reader.HasColumn("ResetToken") ? reader["ResetToken"] as string : null,
            OtpExpiry   = reader.HasColumn("OtpExpiry") && reader["OtpExpiry"] != DBNull.Value
                            ? (DateTime?)reader["OtpExpiry"] : null,
            TokenExpiry = reader.HasColumn("TokenExpiry") && reader["TokenExpiry"] != DBNull.Value
                            ? (DateTime?)reader["TokenExpiry"] : null,
            AttemptCount = reader.HasColumn("AttemptCount") && reader["AttemptCount"] != DBNull.Value
                            ? (int)reader["AttemptCount"] : 0,
            IsUsed      = reader.HasColumn("IsUsed") && reader["IsUsed"] != DBNull.Value
                            && (bool)reader["IsUsed"],
            CreatedAt   = reader.HasColumn("CreatedAt") && reader["CreatedAt"] != DBNull.Value
                            ? (DateTime)reader["CreatedAt"] : DateTime.UtcNow
        };
    }

    #endregion
}

/// <summary>
/// Extension helper để kiểm tra SqlDataReader có column không
/// </summary>
internal static class SqlDataReaderExtensions
{
    public static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
