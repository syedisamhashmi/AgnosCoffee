

using AgnosCoffee.Api.Interfaces.Repositories;
using AgnosCoffee.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //? Set up DB connection
    builder.Services.Configure<AgnosCoffeeDatabaseSettings>(
      builder.Configuration.GetSection("AgnosCoffeeDatabase")
    );

    //? Add repository logic
    builder.Services.AddScoped<IMenuRepository, MenuRepository>();

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
  }
}

