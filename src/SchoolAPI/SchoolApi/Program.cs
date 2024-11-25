using System.Reflection;
using CommonLibrary.Constants;
using FluentValidation;
using FluentValidation.AspNetCore;
using RabbitMQ.Client;
using SchoolApi.Core.Data;
using SchoolApi.Core.Extensions;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
using SchoolApi.Helper;
using SchoolAPI.Helper;


var builder = WebApplication.CreateBuilder(args);

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);


var factory = new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:Host"],
    Port = int.Parse(builder.Configuration["RabbitMQ:Port"]),
    UserName = builder.Configuration["RabbitMQ:Username"],
    Password = builder.Configuration["RabbitMQ:Password"]
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var exchange = builder.Configuration["RabbitMQ:Exchange"];
var queue1 = builder.Configuration["RabbitMQ:Queue1"];
var queue2 = builder.Configuration["RabbitMQ:Queue2"];
var queue3 = builder.Configuration["RabbitMQ:Queue3"];
var routingKey1 = EventType.StudentCreated;
var routingKey2 = EventType.StudentUpdated;
var routingKey3 = EventType.StudentDeleted;

channel.ExchangeDeclare(exchange, ExchangeType.Topic, true);
channel.QueueDeclare(queue1, true, false, false, null);
channel.QueueDeclare(queue2, true, false, false, null);
channel.QueueDeclare(queue3, true, false, false, null);
channel.QueueBind(queue1, exchange, routingKey1, null);
channel.QueueBind(queue2, exchange, routingKey2, null);
channel.QueueBind(queue3, exchange, routingKey3, null);

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
builder.Services.AddSingleton<IEmailService,EmailService>();
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

