using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("ocelot.json")
    .Build();

builder.Services.AddSwaggerForOcelot(configuration);

builder.Services.AddOcelot(configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerForOcelotUI(opt => {
  opt.PathToSwaggerGenerator = "/swagger/docs";

}).UseOcelot().Wait();

app.UseHttpsRedirection();
app.Run();

