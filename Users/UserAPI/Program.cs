using CommonLibrary.Constants;
using FluentValidation;
using FluentValidation.AspNetCore;
using UserAPI.Business.Data;
using UserAPI.Business.Repository;
using UserAPI.Business.Repository.Interfaces;
using UserAPI.Business.Services;
using UserAPI.Business.Services.Interfaces;
using UserAPI.Helper;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();
builder.AddMySqlDbContext<UserAPIDbContext>("SchoolUserDb");
builder.AddJwtAuthentication();
builder.AddSwaggerDocumentation("User API", "v1", "API for managing user data.");
builder.AddCorsPolicy("AllowAll");
builder.AddControllersWithFilters();
builder.AddExceptionHandling();

builder.Services.AddAutoMapper(typeof(AutoMapperProfileUser).Assembly);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
