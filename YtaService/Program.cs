using YtaService.BLL;
using YtaService.BLL.Interfaces;
using YtaService.DAL;
using YtaService.DAL.Interfaces;
using YtaService.DAL.Helper;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// QuestPDF License (Community)
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.WriteIndented = true;
});

// 1. Đăng ký DatabaseHelper
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<GiuongBenhDatabaseHelper>();

// 2. Đăng ký Repository (DAL)
builder.Services.AddScoped<IYtaRepository, YtaRepository>();
builder.Services.AddScoped<IGiuongBenhRepository, GiuongBenhRepository>();

// 3. Đăng ký Business (BLL)
builder.Services.AddScoped<IYtaBusiness, YtaBusiness>();
builder.Services.AddScoped<IGiuongBenhBusiness, GiuongBenhBusiness>();

// ===== JWT AUTHENTICATION =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyHere_MustBeAtLeast32Characters!";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Config Swagger với JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "YTa Service API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token từ IdentityService"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});



// Đăng ký Repository và Business
builder.Services.AddScoped<IHoSoBenhAnRepository, HoSoBenhAnRepository>();
builder.Services.AddScoped<IHoSoBenhAnBusiness, HoSoBenhAnBusiness>();


// Đăng ký cho module Nhập Viện
builder.Services.AddScoped<INhapVienRepository, NhapVienRepository>();
builder.Services.AddScoped<INhapVienBusiness, NhapVienBusiness>();

// Đăng ký cho module Xuất Viện
builder.Services.AddScoped<IXuatVienRepository, XuatVienRepository>();
builder.Services.AddScoped<IXuatVienBusiness, XuatVienBusiness>();

// Đăng ký cho module Hóa Đơn
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
builder.Services.AddScoped<IHoaDonBusiness, HoaDonBusiness>();
builder.Services.AddScoped<IHoaDonReportBusiness, HoaDonReportBusiness>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication PHẢI đặt trước Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();