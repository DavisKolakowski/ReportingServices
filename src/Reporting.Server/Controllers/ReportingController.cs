namespace Reporting.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Reporting.Core.Constants;
    using Reporting.Core.Contracts;
    using Reporting.Core.Models;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportingController> _logger;

        public ReportingController(IReportService reportService, ILogger<ReportingController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active reports.
        /// </summary>
        /// <returns>A list of active reports.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.GetAllActiveReports)]
        public async Task<ActionResult<ActiveReportsResponse>> GetAllActiveReports()
        {
            _logger.LogInformation("Getting all active reports.");
            var reports = await _reportService.GetAllActiveReportsAsync();
            var reportModels = reports.Select(r => new ActiveReportModel
            {
                Key = r.Key,
                Name = r.Name,
                Description = r.Description,
                HasParameters = r.HasParameters,
                CreatedByUser = r.CreatedByUser,
                CreatedAtDate = r.CreatedAtDate,
                LastUpdatedByUser = r.UpdatedByUser ?? null,
                LastUpdatedAtDate = r.UpdatedAtDate ?? null
            });
            var activeReportsResponse = new ActiveReportsResponse
            {
                Model = reportModels
            };
            return Ok(activeReportsResponse);
        }

        /// <summary>
        /// Get details of a report by key.
        /// </summary>
        /// <param name="key">The key of the report.</param>
        /// <returns>Report details.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.GetReportDetailsForUser)]
        public async Task<ActionResult<ReportDetailsResponse>> GetReportDetails(string key)
        {
            _logger.LogInformation("Getting details for report with key: {Key}", key);
            var report = await _reportService.GetReportDetailsAsync(key);
            if (report == null)
            {
                _logger.LogWarning("Report with key: {Key} not found.", key);
                return NotFound();
            }
            var reportDetailsModel = new ReportDetailsModel
            {
                Key = report.Key,
                Name = report.Name,
                Description = report.Description,
                IsActive = report.IsActive,
                HasParameters = report.HasParameters,
                ReportSourceId = report.ReportSourceId,
                Parameters = report.Parameters?.Select(p => new ReportParameterModel
                {
                    Position = p.Position,
                    Name = p.Name,
                    SqlDataType = p.SqlDataType,
                    HasDefaultValue = p.HasDefaultValue
                }).ToArray(),
                ColumnDefinitions = report.ColumnDefinitions.Select(c => new ReportColumnDefinitionModel
                {
                    ColumnId = c.ColumnId,
                    Name = c.Name,
                    SqlDataType = c.SqlDataType,
                    IsNullable = c.IsNullable,
                    IsIdentity = c.IsIdentity
                }).ToArray(),
                CreatedByUser = report.CreatedByUser,
                CreatedAtDate = report.CreatedAtDate,
                LastUpdatedByUser = report.UpdatedByUser ?? null,
                LastUpdatedAtDate = report.UpdatedAtDate ?? null,
            };
            var reportDetailsResponse = new ReportDetailsResponse
            {
                Model = reportDetailsModel,
            };

            return Ok(reportDetailsResponse);
        }

        /// <summary>
        /// Execute a report.
        /// </summary>
        /// <param name="request">The execute report request model.</param>
        /// <returns>Execution result.</returns>
        [HttpPost(ApiRoutes.V1.Reporting.ExecuteReport)]
        public async Task<ActionResult<ExecuteReportResponse>> ExecuteReport(ExecuteReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for ExecuteReportRequest.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Executing report with key: {Key}", request.Model.Key);
            var data = await _reportService.GetReportDataAsTableAsync(request.Model);

            var response = new ExecuteReportResponse
            {
                Model = request.Model,
                Data = data
            };

            return Ok(response);
        }

        /// <summary>
        /// Download a report.
        /// </summary>
        /// <param name="request">The download report request model.</param>
        /// <returns>The report file.</returns>
        [HttpPost(ApiRoutes.V1.Reporting.DownloadReport)]
        public async Task<IActionResult> DownloadReport(DownloadReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for DownloadReportRequest.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Downloading report with key: {Key}", request.Model.Key);
            var reportBytes = await _reportService.GetReportDataAsBytesAsync(request.Model);

            var fileName = $"{request.Model.Key}.xlsx";
            return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// Get all reports.
        /// </summary>
        /// <returns>A list of reports.</returns>
        [HttpGet(ApiRoutes.V1.Admin.Reporting.GetAllReports)]
        public async Task<ActionResult<AllReportsResponse>> GetAllReports()
        {
            _logger.LogInformation("Getting all reports.");
            var reports = await _reportService.GetAllReportsAsync();
            var reportModels = reports.Select(r => new ReportModel
            {
                Key = r.Key,
                Name = r.Name,
                Description = r.Description,
                HasParameters = r.HasParameters,
                IsActive = r.IsActive,
                CreatedByUser = r.CreatedByUser,
                CreatedAtDate = r.CreatedAtDate,
                LastUpdatedByUser = r.UpdatedByUser ?? null,
                LastUpdatedAtDate = r.UpdatedAtDate ?? null
            });
            var activeReportsResponse = new AllReportsResponse
            {
                Model = reportModels
            };
            return Ok(activeReportsResponse);
        }

        /// <summary>
        /// Get details of a report for an admin by key.
        /// </summary>
        /// <param name="key">The key of the report.</param>
        /// <returns>Report admin details.</returns>
        [HttpGet(ApiRoutes.V1.Admin.Reporting.GetReportDetailsForAdmin)]
        public async Task<ActionResult<ReportAdminDetailsResponse>> GetReportDetailsForAdmin(string key)
        {
            _logger.LogInformation("Getting details for report with key: {Key}", key);
            var report = await _reportService.GetReportDetailsAsync(key);
            if (report == null)
            {
                _logger.LogWarning("Report with key: {Key} not found.", key);
                return NotFound();
            }

            var reportSource = await _reportService.GetReportSourceAsync(report);
            if (reportSource == null)
            {
                _logger.LogWarning("Report source with id: {Id} not found.", report.ReportSourceId);
                return NotFound();
            }

            var reportAdminDetailsModel = new ReportAdminDetailsModel
            {
                Key = report.Key,
                Name = report.Name,
                Description = report.Description,
                IsActive = report.IsActive,
                HasParameters = report.HasParameters,
                ReportSourceId = report.ReportSourceId,
                ReportSource = new ReportSourceModel
                {
                    Id = reportSource.Id,
                    Type = reportSource.Type,
                    Schema = reportSource.Schema,
                    Name = reportSource.Name,
                    LastActivityType = reportSource.LastActivityType,
                    LastActivityByUser = reportSource.LastActivityByUser,
                    LastActivityDate = reportSource.LastActivityDate
                },
                Parameters = report.Parameters?.Select(p => new ReportParameterModel
                {
                    Position = p.Position,
                    Name = p.Name,
                    SqlDataType = p.SqlDataType,
                    HasDefaultValue = p.HasDefaultValue
                }).ToArray(),
                ColumnDefinitions = report.ColumnDefinitions.Select(c => new ReportColumnDefinitionModel
                {
                    ColumnId = c.ColumnId,
                    Name = c.Name,
                    SqlDataType = c.SqlDataType,
                    IsNullable = c.IsNullable,
                    IsIdentity = c.IsIdentity
                }).ToArray(),
                CreatedByUser = report.CreatedByUser,
                CreatedAtDate = report.CreatedAtDate,
                LastUpdatedByUser = report.UpdatedByUser ?? null,
                LastUpdatedAtDate = report.UpdatedAtDate ?? null,
            };
            var reportDetailsResponse = new ReportAdminDetailsResponse
            {
                Model = reportAdminDetailsModel,
            };

            return Ok(reportDetailsResponse);
        }

        
        /// <summary>
        /// Gets the details of a report source by its ID.
        /// </summary>
        /// <param name="id">The ID of the report source.</param>
        /// <returns>A <see cref="ReportSourceResponse"/> containing the report source details and its activity log.</returns>
        [HttpGet(ApiRoutes.V1.Admin.Reporting.GetReportSourceDetails)]
        public async Task<ActionResult<ReportSourceResponse>> GetReportSourceDetails(int id)
        {
            _logger.LogInformation("Getting all report sources.");
            var reportSource = await _reportService.GetReportSourceByIdAsync(id);
            var reportSourceModel = new ReportSourceModel
            {
                Id = reportSource.Id,
                Type = reportSource.Type,
                Schema = reportSource.Schema,
                Name = reportSource.Name,
                LastActivityType = reportSource.LastActivityType,
                LastActivityByUser = reportSource.LastActivityByUser,
                LastActivityDate = reportSource.LastActivityDate
            };
            var reportSourceActivity = await _reportService.GetReportSourceActivityLogAsync(reportSource.FullName);
            var reportSourcesResponse = new ReportSourceResponse
            {
                Model = reportSourceModel,
                ActivityLog = reportSourceActivity.Select(a => new ReportSourceActivityModel
                {
                    ActivityType = a.ActivityType,
                    ActivityByUser = a.ActivityByUser,
                    ActivityDate = a.ActivityDate
                })
            };
            return Ok(reportSourcesResponse);
        }


        /// <summary>
        /// Gets all available report sources for creating a new report.
        /// </summary>
        /// <returns>A <see cref="AvailableReportSourcesForNewReportResponse"/> containing all available report sources.</returns>
        [HttpGet(ApiRoutes.V1.Admin.Reporting.GetAvailableReportSourcesForNewReport)]
        public async Task<ActionResult<AvailableReportSourcesForNewReportResponse>> GetAvailableReportSourcesForNewReport()
        {
            _logger.LogInformation("Getting all available report sources for new report.");
            var reportSources = await _reportService.GetAllAvailableReportSourcesAsync();
            var reportSourceModels = reportSources.Select(r => new ReportSourceModel
            {
                Id = r.Id,
                Type = r.Type,
                Schema = r.Schema,
                Name = r.Name,
                LastActivityType = r.LastActivityType,
                LastActivityByUser = r.LastActivityByUser,
                LastActivityDate = r.LastActivityDate
            });
            var availableReportSourcesResponse = new AvailableReportSourcesForNewReportResponse
            {
                Model = reportSourceModels
            };
            return Ok(availableReportSourcesResponse);
        }

        /// <summary>
        /// Create a new report.
        /// </summary>
        /// <param name="request">The new report request model.</param>
        /// <returns>The created report.</returns>
        [HttpPost(ApiRoutes.V1.Admin.Reporting.NewReport)]
        public async Task<IActionResult> CreateReport(NewReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for NewReportRequest.");
                return BadRequest(ModelState);
            }

            var newReport = request.Model;
            newReport.Key = newReport.Key.ToLower();

            _logger.LogInformation("Creating new report with key: {Key}", newReport.Key);
            var createdReport = await _reportService.CreateReportAsync(newReport);

            return CreatedAtAction(nameof(GetReportDetailsForAdmin), new { key = createdReport.Key }, newReport);
        }

        /// <summary>
        /// Update an existing report.
        /// </summary>
        /// <param name="request">The update report request model.</param>
        /// <returns>The updated report.</returns>
        [HttpPut(ApiRoutes.V1.Admin.Reporting.UpdateReport)]
        public async Task<IActionResult> UpdateReport(UpdateReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateReportRequest.");
                return BadRequest(ModelState);
            }

            var updateReport = request.Model;
            updateReport.Key = updateReport.Key.ToLower();

            _logger.LogInformation("Updating report with key: {Key}", updateReport.Key);
            var updatedReport = await _reportService.UpdateReportAsync(updateReport);
            var reportModel = new ReportModel
            {
                Key = updatedReport.Key,
                Name = updatedReport.Name,
                Description = updatedReport.Description,
                IsActive = updatedReport.IsActive,
                HasParameters = updatedReport.HasParameters,
                CreatedByUser = updatedReport.CreatedByUser,
                CreatedAtDate = updatedReport.CreatedAtDate,
                LastUpdatedByUser = updatedReport.UpdatedByUser ?? null,
                LastUpdatedAtDate = updatedReport.UpdatedAtDate ?? null
            };

            return Ok(reportModel);
        }

        /// <summary>
        /// Activate a report.
        /// </summary>
        /// <param name="request">The activate report request model.</param>
        [HttpPut(ApiRoutes.V1.Admin.Reporting.ActivateReport)]
        public async Task<IActionResult> ActivateReport(ActivateReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for ActivateReportRequest.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Activating report with key: {Key}", request.Model.Key);
            await _reportService.ActivateReportAsync(request.Model);
            return NoContent();
        }

        /// <summary>
        /// Disable a report.
        /// </summary>
        /// <param name="request">The disable report request model.</param>
        [HttpPut(ApiRoutes.V1.Admin.Reporting.DeactivateReport)]
        public async Task<IActionResult> DisableReport(DisableReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for DisableReportRequest.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Disabling report with key: {Key}", request.Model.Key);
            await _reportService.DisableReportAsync(request.Model);
            return NoContent();
        }

        /// <summary>
        /// Delete a report.
        /// </summary>
        /// <param name="request">The delete report request model.</param>
        [HttpDelete(ApiRoutes.V1.Admin.Reporting.DeleteReport)]
        public async Task<IActionResult> DeleteReport(DeleteReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for DeleteReportRequest.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Deleting report with key: {Key}", request.Model.Key);
            await _reportService.DeleteReportAsync(request.Model);
            return NoContent();
        }
    }
}