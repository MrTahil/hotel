using HMZ_rt.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static HMZ_rt.Controllers.UserAccounts_controller;
using HMZ_rt.Controllers;
using System.Text.Json;

namespace HMZ_rt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });


            var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            // SmtpSettings hozzáadása a DI konténerhez
            builder.Services.AddSingleton(smtpSettings);
            builder.Services.AddTransient<IEmailService, EmailService>();


            builder.Services.AddDbContext<HmzRtContext>(o =>
            {
                var connection = builder.Configuration.GetConnectionString("MySql");
                o.UseMySQL(connection);
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
                    };
                options.Events = new JwtBearerEvents
                {
                    //OnChallenge = async context =>
                    //{
                    //    context.HandleResponse();
                    //    context.Response.ContentType = "application/json";

                    //    var message = "";
                    //    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    //    {
                    //        message = "Ehhez a művelethez be kell jelentkezned, vagy nincs elég jogosultságod";
                    //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //    }
                    //    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    //    {
                    //        message = "Nincs elég jogosultságod, vagy még nem igazoltad vissza az emailed";
                    //        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    //    }
                    //    else
                    //    {
                    //        // Ha más státuszkód van beállítva, akkor is kezeljük, de itt most csak a 401 és 403-ra koncentrálunk
                    //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //        message = "Ehhez a művelethez be kell jelentkezned";
                    //    }

                    //    await context.Response.WriteAsync(JsonSerializer.Serialize(message));
                    //}
                };
            });
            builder.Services.AddScoped<TokenService>();



            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
            builder.Services.AddScoped<TokenService>();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "Enter JWT Bearer token",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.Urls.Add("https://0.0.0.0:7047");
            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
