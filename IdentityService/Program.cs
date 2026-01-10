using System.Text;
using IdentityService.Authorization;
using IdentityService.BLL;
using IdentityService.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. DEPENDENCY INJECTION =====
// Repository (ADO.NET - đọc connection string từ IConfiguration)
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Permission Handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// ===== 2. JWT AUTHENTICATION =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere_MustBeAtLeast32Characters!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Token hết hạn chính xác
    };
});

// ===== 3. AUTHORIZATION (RBAC) =====
builder.Services.AddAuthorization(options =>
{
    // Tạo policy cho mỗi permission
    // Bệnh nhân
    options.AddPolicy(Permissions.BenhNhan_Xem, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.BenhNhan_Xem)));
    options.AddPolicy(Permissions.BenhNhan_Them, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.BenhNhan_Them)));
    options.AddPolicy(Permissions.BenhNhan_Sua, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.BenhNhan_Sua)));
    options.AddPolicy(Permissions.BenhNhan_Xoa, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.BenhNhan_Xoa)));
    
    // Hồ sơ bệnh án
    options.AddPolicy(Permissions.HoSoBenhAn_Xem, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoSoBenhAn_Xem)));
    options.AddPolicy(Permissions.HoSoBenhAn_Them, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoSoBenhAn_Them)));
    options.AddPolicy(Permissions.HoSoBenhAn_Sua, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoSoBenhAn_Sua)));
    options.AddPolicy(Permissions.HoSoBenhAn_Xoa, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoSoBenhAn_Xoa)));
    
    // Hóa đơn
    options.AddPolicy(Permissions.HoaDon_Xem, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoaDon_Xem)));
    options.AddPolicy(Permissions.HoaDon_Them, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoaDon_Them)));
    options.AddPolicy(Permissions.HoaDon_Sua, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoaDon_Sua)));
    options.AddPolicy(Permissions.HoaDon_Xoa, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.HoaDon_Xoa)));
    
    // Thống kê
    options.AddPolicy(Permissions.ThongKe_Xem, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.ThongKe_Xem)));
    
    // Người dùng
    options.AddPolicy(Permissions.NguoiDung_Xem, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.NguoiDung_Xem)));
    options.AddPolicy(Permissions.NguoiDung_Them, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.NguoiDung_Them)));
    options.AddPolicy(Permissions.NguoiDung_Sua, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.NguoiDung_Sua)));
    options.AddPolicy(Permissions.NguoiDung_Xoa, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.NguoiDung_Xoa)));
});

// ===== 4. CONTROLLERS =====
builder.Services.AddControllers();

// ===== 5. SWAGGER (với hỗ trợ JWT) =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Identity Service API",
        Version = "v1",
        Description = "API xác thực và phân quyền cho Hospital Management System"
    });

    // Thêm nút Authorize trong Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token. Ví dụ: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===== 6. CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE =====

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Authentication PHẢI đặt trước Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
