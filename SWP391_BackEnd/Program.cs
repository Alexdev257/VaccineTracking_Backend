using ClassLib.Repositories; // Import UserRepo
using ClassLib.Service;    // Import UserService
using ClassLib.Helpers;
//using ClassLib.Models;
//using ClassLib.Repositories; // Import UserRepo
//using ClassLib.Service;    // Import UserService
using AutoMapper;
using ClassLib.DTO.Payment;
using ClassLib.Models;
using ClassLib.Service.Momo;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
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
            builder.Services.AddScoped<BookingRepository>();
            builder.Services.AddScoped<BookingService>();

            // Add Json NewtonSoft to show more information
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            // add jwthelper
            builder.Services.AddScoped<JwtHelper>();

            //Automapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            // read Jwt form appsetting.json
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

            //JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(otp =>
                 {
                otp.RequireHttpsMetadata = false;
                otp.SaveToken = true;
                otp.TokenValidationParameters = new TokenValidationParameters
                {
                    //ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(key),
                    //ValidateIssuer = true,
                    //ValidateAudience = true,
                    //ValidIssuer = jwtSettings["Issuer"],
                    //ValidAudience = jwtSettings["Audience"]
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            
             builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vaccine Tracking API",
                    Version = "v1"
                });

                // Thêm tùy chọn nhập Bearer Token vào Swagger UI
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter JWT Token here: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });


            // Update Json Soft

            // Auto Mapper Configurations
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Connect Momo Api
            builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
            //builder.Services.AddScoped<IMomoService, MomoService>;


            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });
            var app = builder.Build();


            // Auto Mapper Configurations
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Auto Mapper Configurations
            //builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
