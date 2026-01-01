using KhoaPhongService.BLL.Interfaces;
using KhoaPhongService.BLL;
using KhoaPhongService.DAL.Interfaces;
using KhoaPhongService.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IKhoaPhongRepository, KhoaPhongRepository>();
builder.Services.AddScoped<IKhoaPhongBusiness, KhoaPhongBusiness>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
