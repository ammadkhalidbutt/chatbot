using ColumnWiseSearch.DataLibrary;
using ColumnWiseSearch.Service;
using Microsoft.EntityFrameworkCore;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions =>
                {
                    // Keep compatibility level for SQL Server 2014
                    sqlServerOptions.UseCompatibilityLevel(120);

                    // Add retry logic for transient failures
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }
            )
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        });
        builder.Services.AddScoped<IProductRepository, ProductRepository>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

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