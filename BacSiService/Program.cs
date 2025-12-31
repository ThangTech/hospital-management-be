using BacSiService.BLL.Services;
using BacSiService.BLL.Interfaces;
using BacSiService.DAL.Repositories;
using BacSiService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using BacSiService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HospitalManageContext>(options =>
    options.UseSqlServer(connectionString));
//builder.Services.AddCors();

// DI for repository and business
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorBusiness, DoctorBusiness>();

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
//app.UseCors();

app.Run();
