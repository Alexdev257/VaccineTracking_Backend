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
using ClassLib.Service.Vaccines;
using Microsoft.Extensions.Options;
using ClassLib.Middlewares;
using ClassLib.Service.PayPal;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using ClassLib.Service.VaccineCombo;
namespace SWP391_BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DbSwpVaccineTrackingContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            //// Add services to the container.
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<VaccineRepository>();
            builder.Services.AddScoped<VaccineService>();
            builder.Services.AddScoped<VaccineComboRepository>();
            builder.Services.AddScoped<VaccineComboService>();

            builder.Services.AddScoped<EmailRepository>();
            builder.Services.AddScoped<EmailService>();

            builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
            builder.Services.AddScoped<IMomoService, MomoService>();
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

            // Test FE
            builder.Services.AddCors(options =>
                    {
                        options.AddPolicy("AllowAll",
                            policy => policy.AllowAnyOrigin()
                                            .AllowAnyMethod()
                                            .AllowAnyHeader());
                    });





            // Test FE
            builder.Services.AddCors(options =>
                    {
                        options.AddPolicy("AllowAll",
                            policy => policy.AllowAnyOrigin()
                                            .AllowAnyMethod()
                                            .AllowAnyHeader());
                    });


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
                         ValidateLifetime = true, // Bắt buộc kiểm tra hạn sử dụng của token
                         ClockSkew = TimeSpan.Zero, // Không cho phép trễ hạn (default là 5 phút)
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = jwtSettings["Issuer"],
                         ValidAudience = jwtSettings["Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey(key)
                     };

                     // Xử lý lỗi token hết hạn và phản hồi 401 Unauthorized
                     otp.Events = new JwtBearerEvents
                     {
                         OnAuthenticationFailed = context =>
                         {
                             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                             {
                                 context.Response.Headers.Add("Token-Expired", "true");
                             }
                             return Task.CompletedTask;
                         }
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

            // read firebase from firebase-config.json
            //var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "firebase-config.json");
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile(firebaseConfigPath)
            //});

            //fix firebase
            //var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "firebase-config.json");
            //if (!File.Exists(firebaseConfigPath))
            //{
            //    throw new Exception("Firebase config file not found!");
            //}

            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile(firebaseConfigPath)
            //});


            // Connect Payment API
            builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
            builder.Services.AddScoped<IMomoService, MomoService>();
            builder.Services.Configure<PaypalOptionModel>(builder.Configuration.GetSection("PaypalAPI"));
            //builder.Services.AddScoped<IPayPalService, PayPalService>();
            builder.Services.AddHttpClient();

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });
            var app = builder.Build();
            // Test FE
            app.UseCors("AllowAll");

            //app.UseMiddleware<TokenExpiredMiddleware>();




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

//using ClassLib.Repositories; // Import UserRepo
//using ClassLib.Service;    // Import UserService
//using ClassLib.Helpers;
//using AutoMapper;
//using ClassLib.DTO.Payment;
//using ClassLib.Models;
//using ClassLib.Service.Momo;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Microsoft.OpenApi.Models;
//using Newtonsoft.Json;
//using ClassLib.Service.Vaccines;
//using Microsoft.Extensions.Options;
//using ClassLib.Middlewares;
//using ClassLib.Service.PayPal;
//using FirebaseAdmin;
//using Google.Apis.Auth.OAuth2;
//using ClassLib.Service.VaccineCombo;

//namespace SWP391_BackEnd
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            builder.Services.AddDbContext<DbSwpVaccineTrackingContext>(options =>
//                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//            //// Add services to the container.
//            builder.Services.AddScoped<UserRepository>();
//            builder.Services.AddScoped<UserService>();
//            builder.Services.AddScoped<BookingRepository>();
//            builder.Services.AddScoped<BookingService>();
//            builder.Services.AddScoped<VaccineRepository>();
//            builder.Services.AddScoped<VaccineService>();
//            builder.Services.AddScoped<VaccineComboRepository>();
//            builder.Services.AddScoped<VaccineComboService>();

//            builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
//            builder.Services.AddScoped<IMomoService, MomoService>();

//            // Add Json NewtonSoft to show more information
//            builder.Services.AddControllers()
//                .AddNewtonsoftJson(options =>
//                {
//                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//                });

//            // add jwthelper
//            builder.Services.AddScoped<JwtHelper>();

//            //Automapper
//            builder.Services.AddAutoMapper(typeof(MappingProfile));

//            // Test FE
//            builder.Services.AddCors(options =>
//            {
//                options.AddPolicy("AllowAll",
//                    policy => policy.AllowAnyOrigin()
//                                    .AllowAnyMethod()
//                                    .AllowAnyHeader());
//            });

//            // read Jwt from appsettings.json
//            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

//            //JWT
//            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                .AddJwtBearer(otp =>
//                {
//                    otp.RequireHttpsMetadata = false;
//                    otp.SaveToken = true;
//                    otp.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuer = true,
//                        ValidateAudience = true,
//                        ValidateLifetime = true, // Bắt buộc kiểm tra hạn sử dụng của token
//                        ClockSkew = TimeSpan.Zero, // Không cho phép trễ hạn (default là 5 phút)
//                        ValidateIssuerSigningKey = true,
//                        ValidIssuer = jwtSettings["Issuer"],
//                        ValidAudience = jwtSettings["Audience"],
//                        IssuerSigningKey = new SymmetricSecurityKey(key)
//                    };

//                    // Xử lý lỗi token hết hạn và phản hồi 401 Unauthorized
//                    otp.Events = new JwtBearerEvents
//                    {
//                        OnAuthenticationFailed = context =>
//                        {
//                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
//                            {
//                                context.Response.Headers.Add("Token-Expired", "true");
//                            }
//                            return Task.CompletedTask;
//                        }
//                    };
//                });

//            builder.Services.AddAuthorization();
//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo
//                {
//                    Title = "Vaccine Tracking API",
//                    Version = "v1"
//                });
//                // Thêm tùy chọn nhập Bearer Token vào Swagger UI
//                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    Description = "Enter JWT Token here: Bearer {token}",
//                    Name = "Authorization",
//                    In = ParameterLocation.Header,
//                    Type = SecuritySchemeType.Http,
//                    Scheme = "Bearer"
//                });

//                c.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            }
//                        },
//                        new string[] {}
//                    }
//                });
//            });

//            // read firebase from firebase-config.json
//            var firebaseConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "firebase-config.json");
//            if (!File.Exists(firebaseConfigPath))
//            {
//                throw new Exception("Firebase config file not found!");
//            }

//            FirebaseApp.Create(new AppOptions()
//            {
//                Credential = GoogleCredential.FromFile(firebaseConfigPath)
//            });

//            var app = builder.Build();
//            // Test FE
//            app.UseCors("AllowAll");

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();
//            app.UseAuthentication();
//            app.UseAuthorization();
//            app.MapControllers();
//            app.Run();
//        }
//    }
//}
