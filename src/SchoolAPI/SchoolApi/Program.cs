using System.Reflection;
using CommonLibrary.Constants;
using FluentValidation;
using FluentValidation.AspNetCore;
using RabbitMQ.Client;
using SchoolApi.Core.Data;
using SchoolApi.Core.Extensions;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
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
builder.Services.AddSingleton<ICommonSagaService, CommonSagaService>();
builder.Services.AddSingleton<IEmailService,EmailService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddHttpClient();

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

