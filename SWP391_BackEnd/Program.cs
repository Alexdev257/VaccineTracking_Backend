//using ClassLib.Models;
//using ClassLib.Repositories; // Import UserRepo
//using ClassLib.Service;    // Import UserService
using AutoMapper;
using ClassLib.Models;
using ClassLib.Repositories;
using ClassLib.Service;
using Microsoft.EntityFrameworkCore;
namespace SWP391_BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DbSwpVaccineTracking2Context>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //// Add services to the container.
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<UserService>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Update Json Soft
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });
            var app = builder.Build();

            // Auto Mapper Configurations
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

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
}
