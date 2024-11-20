using System.Reflection;
using CommonLibrary.Constants;
using FluentValidation;
using FluentValidation.AspNetCore;
using SchoolApi.Core.Data;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
using SchoolApi.Helper;
using SchoolAPI.Helper;


var builder = WebApplication.CreateBuilder(args);

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.AddSerilogLogging();
builder.AddMySqlDbContext<SchoolDbContext>("ConnectionStrings");
builder.AddJwtAuthentication();
builder.AddSwaggerDocumentation("School API", "v1", "API for managing student records in the school system.",xmlPath);
builder.AddCorsPolicy("AllowAll");
builder.AddControllersWithFilters();
builder.AddExceptionHandling();

builder.Services.AddSingleton<RabbitMQProducer>();
builder.Services.AddHostedService<RabbitMQConsumer>();


builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(o => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();

