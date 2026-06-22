using FileService.Infrastructure.Authentication;
using FileService.Infrastructure.Persistence;
using FileService.Presentation.Services;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FileService.Presentation.Protos;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FileService.Domain.Interfaces;
using FileService.Application.Interfaces;
using FileService.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FileService.Application.Features.UploadFile;
using Amazon.S3;

namespace FileService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FileDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IAmazonS3>(sp =>
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = "https://storage.yandexcloud.net"
                };
                return new AmazonS3Client(
                    builder.Configuration["YandexStorage:AccessKey"],
                    builder.Configuration["YandexStorage:SecretKey"],
                    config);
            });

            builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FileDbContext>());
            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder.Services.AddScoped<IFileStorage, YandexFileRepository>();

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(UploadFileCommand).Assembly);
            });

            builder.Services.AddGrpc().AddJsonTranscoding();

            var app = builder.Build();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGrpcService<FileGrpcService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}