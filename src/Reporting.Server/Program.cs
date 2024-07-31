namespace Reporting.Server
{
    using System.Diagnostics.Contracts;
    using System.Security.Claims;

    using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.OpenApi.Models;

    using Reporting.Core.Contracts;
    using Reporting.Core.Data;
    using Reporting.Core.Extensions;
    using Reporting.Core.Providers;
    using Reporting.Core.Services;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new ParameterModelBinderProvider());
            });
            var reportingDbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(reportingDbConnectionString))
            {
                throw new System.Exception("Connection string is missing");
            }
            builder.Services.AddReportingServices(reportingDbConnectionString);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(o =>
            {
                o.AllowAnyOrigin();
                o.AllowAnyMethod();
                o.AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
