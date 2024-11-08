using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Core.Data;
using SchoolAPI.Helper;
using SchoolApi.Core.Repository;
using SchoolApi.Core.Service;
using SchoolAPI.GlobalExceptionHandling;
using SchoolAPI.Constants;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var serverVersion = ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ConnectionStrings"));
builder.Services.AddDbContext<SchoolDbContext>(
    options => options
    .UseMySql(builder.Configuration.GetConnectionString("ConnectionStrings"), serverVersion)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging());

builder.Services.AddControllers(options =>
{
    // Customize model state invalid response
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "This field is required.");
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorDetails = new 
        {
            Message = ErrorMessages.ValidationError,
            StatusCode = StatusCodes.Status400BadRequest,
            ExceptionErrors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(err => err.ErrorMessage).ToArray()
            )
        };
 
        return new BadRequestObjectResult(errorDetails);
    };
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
// builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GeneraliseExceptionHandler>();



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

app.UseExceptionHandler(o => {});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
