
using Azure.Core;
using CafeReservationAPI.Constant;
using CafeReservationAPI.Data;
using CafeReservationAPI.Models;
using CafeReservationAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace CafeReservationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // DB
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            //register services
            builder.Services.AddScoped<TokenService>();
            // Auth
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            builder.Services.AddAuthorization();





            var app = builder.Build();

            app.UseCors(opt =>
            {
                opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                //.WithOrigins("localhost",
                //                    "127.0.0.1:4200",
                //                    "https://127.0.0.1:4200",
                //                    "http://127.0.0.1:4200",
                //                "localhost/",
                //                "https://localhost:4200",
                //                "http://localhost:4200", "::1");
            });



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
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (!dbContext.Users.Any())
                {

                    using var sha256 = SHA256.Create();
                    var bytes = Encoding.UTF8.GetBytes("123456");
                    var hash = sha256.ComputeHash(bytes);
                    string hashed = Convert.ToBase64String(hash);
                    dbContext.Users.Add(new User
                    {
                        Email = "admin@admin",
                        Name = "Admin",
                        PasswordHash = hashed,
                        Role = UserRoles.ADMIN,

                    });
                    dbContext.SaveChanges();
                }




            }

            app.Run();
        }
    }
}
