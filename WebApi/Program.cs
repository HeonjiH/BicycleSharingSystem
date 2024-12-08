
using Microsoft.EntityFrameworkCore;

namespace BicycleSharingSystem.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddAuthorization();

        // builder 보다 아래, app 보다 위 중 편한 곳에 위치
        var connectionString = "server=localhost;user=root;password=1111;database=workshopdb";
        var serverVersion = new MySqlServerVersion(new Version(9, 0));
        builder.Services.AddDbContext<BicycleSharingContext>(options => options.UseMySql(connectionString, serverVersion));

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        //컨트롤러 사용할 수 있게 추가
        builder.Services.AddControllers();

        //스웨거 사용하도록
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var app = builder.Build();

        // create scope
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<BicycleSharingContext>();
            Task.Run(context.InitializeDatabaseAsync);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            //swagger사용할 수 있게
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        //컨트롤러
        app.MapControllers();

        app.Run();
    }
}
