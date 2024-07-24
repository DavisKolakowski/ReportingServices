namespace Reporting.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Reporting.Core.Contracts;
    using Reporting.Core.Data;
    using Reporting.Core.Services;

    public static class AppServiceCollectionExtensions
    {
        public static void AddReportingServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportSourceRepository, ReportSourceRepository>();
            services.AddScoped<ISystemRepository, SystemRepository>();
            services.AddScoped<IDapperConnectionService>(provider => new DapperConnectionService(connectionString));
        }
    }
}
