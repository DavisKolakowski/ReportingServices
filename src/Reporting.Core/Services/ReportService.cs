namespace Reporting.Core.Services
{
    using System.Data;

    using DocumentFormat.OpenXml.Bibliography;

    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using Reporting.Core.Contracts;
    using Reporting.Core.Entities;
    using Reporting.Core.Extensions;
    using Reporting.Core.Helpers;
    using Reporting.Core.Models;
    using Reporting.Core.Utilities;

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

        public async Task<ReportSource> GetReportSourceAsync(Report report)
        {
            if (report == null)
            {
                _logger.LogWarning($"Invalid report provided.");
                throw new KeyNotFoundException($"Report is invalid or not provided.");
            }

            var reportSource = await _reportSourceRepository.GetByIdAsync(report.ReportSourceId);
            if (reportSource == null)
            {
                _logger.LogError($"Report source not found for report '{report.Key}'");
                throw new InvalidOperationException($"Report source not found for report '{report.Key}'");
            }
            return reportSource;
        }

        public async Task<ReportSource> GetReportSourceByIdAsync(int reportSourceId)
        {
            var reportSource = await _reportSourceRepository.GetByIdAsync(reportSourceId);
            if (reportSource == null)
            {
                _logger.LogError($"Report source not found for report source id '{reportSourceId}'");
                throw new InvalidOperationException($"Report source not found for report source id '{reportSourceId}'");
            }
            return reportSource;
        }

        public async Task<IEnumerable<ReportSourceHistory>> GetReportSourceActivityLogAsync(string sqlObjectName)
        {
            var reportSourceHistory = await _reportSourceRepository.GetActivityHistoryAsync(sqlObjectName);
            if (reportSourceHistory == null)
            {
                _logger.LogError($"Report source not found for report source '{sqlObjectName}'");
                throw new InvalidOperationException($"Report source not found for report source '{sqlObjectName}'");
            }
            return reportSourceHistory;
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

        public async Task<IEnumerable<Dictionary<string, object>>> GetReportDataAsTableAsync(ReportDetailsModel report)
        {
            if (string.IsNullOrWhiteSpace(report.Key))
            {
                _logger.LogWarning($"Invalid report key provided.");
                throw new KeyNotFoundException($"Report key is invalid or not provided.");
            }

            var reportSource = await _reportSourceRepository.GetByIdAsync(report.ReportSourceId);
            if (reportSource == null)
            {
                _logger.LogError($"Report source not found for report '{report.Key}'");
                throw new InvalidOperationException($"Report source not found for report '{report.Key}'");
            }

            var reportColumns = await _systemRepository.GetReportColumnDefinitionsAsync(reportSource.FullName);
            var reportColumnsArray = reportColumns.ToArray();

            if (reportColumnsArray.Length != report.ColumnDefinitions.Length)
            {
                _logger.LogError($"Report column definitions do not match for report '{report.Key}'");
                throw new InvalidOperationException($"Report column definitions do not match for report '{report.Key}'");
            }

            var dataTable = new DataTable();
            if (!report.HasParameters)
            {
                dataTable = await _reportRepository.ExecuteAsync(reportSource, reportColumnsArray);
                return dataTable.ToDictionaryList();
            }

            var reportParameters = await _systemRepository.GetReportParametersAsync(reportSource.FullName);
            if (report.HasParameters && reportParameters == null)
            {
                _logger.LogError($"Report '{report.Key}' requires parameters but none were found.");
                throw new InvalidOperationException($"Report '{report.Key}' requires parameters.");
            }

            if (report.Parameters == null || !report.Parameters.Any())
            {
                _logger.LogError($"Report '{report.Key}' requires parameters but none were provided.");
                throw new InvalidOperationException($"Report '{report.Key}' requires parameters.");
            }

            foreach (var parameter in reportParameters)
            {
                var parameterModel = report.Parameters.FirstOrDefault(p => p.Name == parameter.Name);
                if (parameterModel == null)
                {
                    _logger.LogError($"Parameter '{parameter.Name}' not found for report '{report.Key}'.");
                    throw new KeyNotFoundException($"Parameter '{parameter.Name}' not found for report '{report.Key}'.");
                }
                parameter.CurrentValue = parameterModel.CurrentValue;
            }

            var reportParametersArray = reportParameters.ToArray();

            dataTable = await _reportRepository.ExecuteAsync(reportSource, reportColumnsArray, reportParametersArray);
            return dataTable.ToDictionaryList();
        }

        public async Task<byte[]> GetReportDataAsBytesAsync(ReportDetailsModel report)
        {
            if (report == null)
            {
                _logger.LogWarning($"Invalid report details provided.");
                throw new KeyNotFoundException($"Report details are invalid or not provided.");
            }

            if (string.IsNullOrWhiteSpace(report.Key))
            {
                _logger.LogWarning($"Invalid report key provided.");
                throw new KeyNotFoundException($"Report key is invalid or not provided.");
            }

            if (report.Name.IsNullOrEmpty())
            {
                _logger.LogWarning($"Invalid report name provided.");
                throw new KeyNotFoundException($"Report name is invalid or not provided.");
            }
            
            if (report.Description.IsNullOrEmpty())
            {
                report.Description = "No description provided.";
            }

            var reportSource = await _reportSourceRepository.GetByIdAsync(report.ReportSourceId);
            if (reportSource == null)
            {
                _logger.LogError($"Report source not found for report '{report.Key}'");
                throw new InvalidOperationException($"Report source not found for report '{report.Key}'");
            }

            var reportColumns = await _systemRepository.GetReportColumnDefinitionsAsync(reportSource.FullName);
            var reportColumnsArray = reportColumns.ToArray();

            if (reportColumnsArray.Length != report.ColumnDefinitions.Length)
            {
                _logger.LogError($"Report column definitions do not match for report '{report.Key}'");
                throw new InvalidOperationException($"Report column definitions do not match for report '{report.Key}'");
            }

            var reportDataModel = new DataTable();

            if (!report.HasParameters)
            {
                reportDataModel = await _reportRepository.ExecuteAsync(reportSource, reportColumnsArray);
                return ReportWorkbookUtility.CreateExcelReport(report, reportDataModel);
            }

            var reportParameters = await _systemRepository.GetReportParametersAsync(reportSource.FullName);
            if (report.HasParameters && reportParameters == null)
            {
                _logger.LogError($"Report '{report.Key}' requires parameters but none were found.");
                throw new InvalidOperationException($"Report '{report.Key}' requires parameters.");
            }

            if (report.Parameters == null || !report.Parameters.Any())
            {
                _logger.LogError($"Report '{report.Key}' requires parameters but none were provided.");
                throw new InvalidOperationException($"Report '{report.Key}' requires parameters.");
            }

            foreach (var parameter in reportParameters)
            {
                var parameterModel = report.Parameters.FirstOrDefault(p => p.Name == parameter.Name);
                if (parameterModel == null)
                {
                    _logger.LogError($"Parameter '{parameter.Name}' not found for report '{report.Key}'.");
                    throw new KeyNotFoundException($"Parameter '{parameter.Name}' not found for report '{report.Key}'.");
                }
                parameter.CurrentValue = parameterModel.CurrentValue;
            }

            var reportParametersArray = reportParameters.ToArray();

            reportDataModel = await _reportRepository.ExecuteAsync(reportSource, reportColumnsArray, reportParametersArray);
            return ReportWorkbookUtility.CreateExcelReport(report, reportDataModel);
        }
    }
}
