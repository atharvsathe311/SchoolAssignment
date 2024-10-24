using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using SchoolAPI.Data;
using SchoolAPI.DTO;
using SchoolAPI.Helper;
using SchoolAPI.Models;
using SchoolAPI.Repository;
using SchoolAPI.Service;
using SchoolAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConnectionStrings"));
builder.Services.AddDbContext<SchoolDbContext>(
    options => options
    .UseMySql(builder.Configuration.GetConnectionString("ConnectionStrings"), serverVersion)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging());

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<StudentPostDTOValidator>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


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
