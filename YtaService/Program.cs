using YtaService.BLL;
using YtaService.BLL.Interfaces;
using YtaService.DAL;
using YtaService.DAL.Interfaces;
using YtaService.DAL.Helper;
using System.Text.Json.Serialization; // <--- 1. QUAN TRỌNG: Thêm thư viện này

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- 2. QUAN TRỌNG: SỬA ĐOẠN NÀY ---
// Thay vì chỉ AddControllers(), ta thêm AddJsonOptions để cắt vòng lặp
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.WriteIndented = true;
});
// -----------------------------------

// 1. Đăng ký DatabaseHelper
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<GiuongBenhDatabaseHelper>();

// 2. Đăng ký Repository (DAL)
builder.Services.AddScoped<IYtaRepository, YtaRepository>();
builder.Services.AddScoped<IGiuongBenhRepository, GiuongBenhRepository>();

// 3. Đăng ký Business (BLL)
builder.Services.AddScoped<IYtaBusiness, YtaBusiness>();
builder.Services.AddScoped<IGiuongBenhBusiness, GiuongBenhBusiness>();

// Config Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();