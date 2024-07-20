namespace Reporting.Core.Services
{
    using Microsoft.Extensions.Logging;

    using Reporting.Core.Contracts;
    using Reporting.Core.Entities;
    using Reporting.Core.Models;

    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportSourceRepository _reportSourceRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IReportRepository reportRepository,
            IReportSourceRepository reportSourceRepository,
            ISystemRepository systemRepository,
            ILogger<ReportService> logger)
        {
            _reportRepository = reportRepository;
            _reportSourceRepository = reportSourceRepository;
            _systemRepository = systemRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _reportRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Report>> GetAllActiveReportsAsync()
        {
            return await _reportRepository.GetAllActiveAsync();
        }

        public async Task<IEnumerable<ReportSource>> GetAllAvailableReportSourcesAsync()
        {
            return await _reportSourceRepository.GetUnusedAsync();
        }

        public async Task<Report?> GetReportByKeyAsync(string reportKey)
        {
            return await _reportRepository.GetByKeyAsync(reportKey);
        }

        public async Task<Report?> GetReportDetailsAsync(string reportKey)
        {
            var report = await GetReportByKeyAsync(reportKey);
            if (report == null)
            {
                _logger.LogWarning($"Report with key '{reportKey}' not found.");
                throw new KeyNotFoundException($"Report with key '{reportKey}' not found.");
            }

            var reportSource = await _reportSourceRepository.GetByIdAsync(report.ReportSourceId);
            if (reportSource == null)
            {
                _logger.LogError($"Report source not found for report '{report.Key}'");
                throw new InvalidOperationException($"Report source not found for report '{report.Key}'");
            }

            if (report.HasParameters)
            {
                var reportParameters = await _systemRepository.GetReportParametersAsync(reportSource.FullName);
                report.Parameters = reportParameters.ToArray();
            }

            var reportColumnDefinitions = await _systemRepository.GetReportColumnDefinitionsAsync(reportSource.FullName);
            report.ColumnDefinitions = reportColumnDefinitions.ToArray();

            return report;
        }

        public async Task<IEnumerable<ReportSourceHistory>> GetReportActivityAsync(string reportKey)
        {
            var report = await GetReportByKeyAsync(reportKey);
            if (report == null)
            {
                return Enumerable.Empty<ReportSourceHistory>();
            }

            var reportSource = await _reportSourceRepository.GetByIdAsync(report.ReportSourceId);
            if (reportSource == null)
            {
                _logger.LogError($"Report source not found for report '{report.Key}'");
                throw new InvalidOperationException($"Report source not found for report '{report.Key}'");
            }

            return await _reportSourceRepository.GetActivityHistoryAsync(reportSource.FullName);
        }

        public async Task<Report> CreateReportAsync(NewReportModel newReport)
        {
            var report = new Report
            {
                Key = newReport.Key,
                Name = newReport.Name,
                Description = newReport.Description,
                CreatedByUser = newReport.CreatedByUser,
                CreatedAtDate = newReport.CreatedAtDate,
                IsActive = newReport.IsActive,
                HasParameters = newReport.HasParameters,
                ReportSourceId = newReport.ReportSourceId
            };

            return await _reportRepository.CreateAsync(report);
        }

        public async Task<Report> UpdateReportAsync(UpdateReportModel updateReport)
        {
            var existingReport = await _reportRepository.GetByKeyAsync(updateReport.Key);
            if (existingReport == null)
            {
                throw new KeyNotFoundException($"Report with key '{updateReport.Key}' not found.");
            }

            existingReport.Name = updateReport.Name;
            existingReport.Description = updateReport.Description;
            existingReport.UpdatedByUser = updateReport.UpdatedByUser;
            existingReport.UpdatedAtDate = updateReport.UpdatedAtDate;
            existingReport.IsActive = updateReport.IsActive;
            existingReport.HasParameters = updateReport.HasParameters;

            return await _reportRepository.UpdateAsync(existingReport);
        }

        public async Task ActivateReportAsync(ActivateReportModel report)
        {
            var existingReport = await _reportRepository.GetByKeyAsync(report.Key);
            if (existingReport == null)
            {
                throw new KeyNotFoundException($"Report with key '{report.Key}' not found.");
            }

            existingReport.IsActive = true;
            existingReport.UpdatedByUser = report.UpdatedByUser;
            existingReport.UpdatedAtDate = report.UpdatedAtDate;

            await _reportRepository.UpdateAsync(existingReport);
        }

        public async Task DisableReportAsync(DisableReportModel report)
        {
            var existingReport = await _reportRepository.GetByKeyAsync(report.Key);
            if (existingReport == null)
            {
                throw new KeyNotFoundException($"Report with key '{report.Key}' not found.");
            }

            existingReport.IsActive = false;
            existingReport.UpdatedByUser = report.UpdatedByUser;
            existingReport.UpdatedAtDate = report.UpdatedAtDate;

            await _reportRepository.UpdateAsync(existingReport);
        }

        public async Task DeleteReportAsync(DeleteReportModel report)
        {
            var existingReport = await _reportRepository.GetByKeyAsync(report.Key);
            if (existingReport == null)
            {
                throw new KeyNotFoundException($"Report with key '{report.Key}' not found.");
            }

            await _reportRepository.DeleteAsync(existingReport);
        }

        public async Task<ReportData> GetReportDataAsync(string reportKey, Dictionary<string, object>? parameters)
        {
            var report = await GetReportDetailsAsync(reportKey);
            
            if (report == null)
            {
                _logger.LogWarning($"Report with key '{reportKey}' not found.");
                throw new KeyNotFoundException($"Report with key '{reportKey}' not found.");
            }

            if (report.HasParameters == true && report.Parameters != null)
            {                 
                if (parameters == null)
                {
                    _logger.LogError($"Report '{reportKey}' requires parameters.");
                    throw new InvalidOperationException($"Report '{reportKey}' requires parameters.");
                }

                foreach (var parameter in report.Parameters)
                {                                    
                    if (!parameters.ContainsKey(parameter.Name))
                    {
                        _logger.LogError($"Parameter '{parameter.Name}' not found for report '{reportKey}'.");
                        throw new KeyNotFoundException($"Parameter '{parameter.Name}' not found for report '{reportKey}'.");
                    }
                    parameter.CurrentValue = parameters[parameter.Name];
                }
            }

            var reportSource = await _reportSourceRepository.GetByIdAsync(report.ReportSourceId);
            if (reportSource == null)
            {
                _logger.LogError($"Report source not found for report '{report.Key}'");
                throw new InvalidOperationException($"Report source not found for report '{report.Key}'");
            }

            var reportData = await _reportRepository.ExecuteAsync(reportSource, report.ColumnDefinitions, report.Parameters);

            return reportData;
        }
    }
}
