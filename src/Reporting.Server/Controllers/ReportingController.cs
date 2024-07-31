namespace Reporting.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Reporting.Server.Constants;
    using Reporting.Core.Contracts;
    using Reporting.Core.Models;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Reporting.Core.Helpers;
    using Reporting.Core.Entities;
    using Reporting.Core.Binders;

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
        [HttpGet(ApiRoutes.V1.Reporting.GetReports)]
        public async Task<ActionResult<IEnumerable<ReportModel>>> GetAllActiveReports()
        {
            _logger.LogInformation("Getting all active reports.");
            var reports = await _reportService.GetAllActiveReportsAsync();
            var reportModels = reports.Select(report => new ReportModel
            {
                Key = report.Key,
                ReportSourceId = report.ReportSourceId,
                Name = report.Name,
                Description = report.Description,
                IsActive = report.IsActive,
                HasParameters = report.HasParameters,
                CreatedByUser = report.CreatedByUser,
                CreatedAtDate = report.CreatedAtDate,
                LastUpdatedByUser = report.UpdatedByUser,
                LastUpdatedAtDate = report.UpdatedAtDate
            });
            return Ok(reportModels);
        }

        [HttpGet(ApiRoutes.V1.Reporting.GetReportParameters)]
        public async Task<ActionResult<IEnumerable<ReportParameterModel>>> GetReportParameters(string key)
        {
            _logger.LogInformation("Getting parameters for report with key: {Key}", key);
            var parameters = await _reportService.GetParametersForReportKeyAsync(key);
            if (parameters == null)
            {
                _logger.LogWarning("Parameters for report with key: {Key} not found.", key);
                return NotFound();
            }

            var reportParameters = parameters.Select(p => new ReportParameterModel
            {
                Position = p.Position,
                Name = p.Name,
                SqlDataType = p.SqlDataType,
                HasDefaultValue = p.HasDefaultValue,
                CurrentValue = p.CurrentValue
            });

            return Ok(reportParameters);
        }

        [HttpGet(ApiRoutes.V1.Reporting.GetReportColumnDefinitions)]
        public async Task<ActionResult<IEnumerable<ReportColumnDefinitionModel>>> GetReportColumnDefinitions(string key)
        {
            _logger.LogInformation("Getting column definitions for report with key: {Key}", key);
            var columnDefinitions = await _reportService.GetColumnDefinitionsForReportKeyAsync(key);
            if (columnDefinitions == null)
            {
                _logger.LogWarning("Column definitions for report with key: {Key} not found.", key);
                return NotFound();
            }

            var reportColumnDefinitions = columnDefinitions.Select(c => new ReportColumnDefinitionModel
            {
                ColumnId = c.ColumnId,
                Name = c.Name,
                SqlDataType = c.SqlDataType,
                IsNullable = c.IsNullable,
                IsIdentity = c.IsIdentity
            });

            return Ok(reportColumnDefinitions);
        }

        /// <summary>
        /// Get details of a report by key.
        /// </summary>
        /// <param name="key">The key of the report.</param>
        /// <returns>Report details.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.GetReportDetails)]
        public async Task<ActionResult<ReportDetailsModel>> GetReportDetails(string key)
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
                Report = new ReportModel
                {
                    Key = report.Key,
                    ReportSourceId = report.ReportSourceId,
                    Name = report.Name,
                    Description = report.Description,
                    IsActive = report.IsActive,
                    HasParameters = report.HasParameters,
                    CreatedByUser = report.CreatedByUser,
                    CreatedAtDate = report.CreatedAtDate,
                    LastUpdatedByUser = report.UpdatedByUser,
                    LastUpdatedAtDate = report.UpdatedAtDate
                },
                Parameters = report.Parameters?.Select(p => new ReportParameterModel
                {
                    Position = p.Position,
                    Name = p.Name,
                    SqlDataType = p.SqlDataType,
                    HasDefaultValue = p.HasDefaultValue
                }),
                ColumnDefinitions = report.ColumnDefinitions.Select(c => new ReportColumnDefinitionModel
                {
                    ColumnId = c.ColumnId,
                    Name = c.Name,
                    SqlDataType = c.SqlDataType,
                    IsNullable = c.IsNullable,
                    IsIdentity = c.IsIdentity
                }),
            };

            return Ok(reportDetailsModel);
        }

        /// <summary>
        /// Execute a report.
        /// </summary>
        /// <param name="key">The report key.</param>
        /// <param name="queryParams">The report parameters as query string.</param>
        /// <returns>Execution result.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.GetReportData)]
        public async Task<ActionResult<ReportDataModel>> GetReportData(string key, [ModelBinder(BinderType = typeof(ParameterModelBinder))] Dictionary<string, object>? queryParams = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Report key is required for ExecuteReportRequest.");
                return BadRequest("Report key is required.");
            }

            _logger.LogInformation("Executing report with key: {Key}", key);

            var report = await _reportService.GetReportDetailsAsync(key);

            if (report == null)
            {
                _logger.LogWarning("Report with key: {Key} not found.", key);
                return NotFound();
            }

            if (report.HasParameters && (report.Parameters == null || queryParams == null || report.Parameters.Length != queryParams.Count || !report.Parameters.All(p => queryParams.ContainsKey(p.Name))))
            {
                _logger.LogWarning("Invalid parameters for report with key: {Key}", key);
                return BadRequest("Invalid or missing parameters.");
            }

            var reportDetails = new ReportDetailsModel
            {
                Report = new ReportModel
                {
                    Key = key,
                    ReportSourceId = report.ReportSourceId,
                    Name = report.Name,
                    Description = report.Description,
                    IsActive = report.IsActive,
                    HasParameters = report.HasParameters,
                    CreatedByUser = report.CreatedByUser,
                    CreatedAtDate = report.CreatedAtDate,
                    LastUpdatedByUser = report.UpdatedByUser,
                    LastUpdatedAtDate = report.UpdatedAtDate
                },
                Parameters = report.HasParameters && report.Parameters != null && queryParams != null && report.Parameters.Length == queryParams.Count
                    ? report.Parameters.Select(p => new ReportParameterModel
                    {
                        Position = p.Position,
                        Name = p.Name,
                        SqlDataType = p.SqlDataType,
                        HasDefaultValue = p.HasDefaultValue,
                        CurrentValue = queryParams[p.Name],
                    })
                    : Array.Empty<ReportParameterModel>(),
                ColumnDefinitions = report.ColumnDefinitions.Select(c => new ReportColumnDefinitionModel
                {
                    ColumnId = c.ColumnId,
                    Name = c.Name,
                    SqlDataType = c.SqlDataType,
                    IsNullable = c.IsNullable,
                    IsIdentity = c.IsIdentity
                }).ToArray()
            };

            var data = await _reportService.GetReportDataAsTableAsync(reportDetails);

            var reportDataModel = new ReportDataModel(reportDetails)
            {
                Data = data
            };

            return Ok(reportDataModel);
        }

        /// <summary>
        /// Download a report.
        /// </summary>
        /// <param name="key">The report key.</param>
        /// <param name="queryParams">The report parameters as query string.</param>
        /// <returns>The report file.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.DownloadReport)]
        public async Task<IActionResult> DownloadReport(string key, [ModelBinder(BinderType = typeof(ParameterModelBinder))] Dictionary<string, object>? queryParams = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Report key is required for DownloadReportRequest.");
                return BadRequest("Report key is required.");
            }

            _logger.LogInformation("Downloading report with key: {Key}", key);

            var report = await _reportService.GetReportDetailsAsync(key);

            if (report == null)
            {
                _logger.LogWarning("Report with key: {Key} not found.", key);
                return NotFound();
            }

            if (report.HasParameters && (report.Parameters == null || queryParams == null || report.Parameters.Length != queryParams.Count || !report.Parameters.All(p => queryParams.ContainsKey(p.Name))))
            {
                _logger.LogWarning("Invalid parameters for report with key: {Key}", key);
                return BadRequest("Invalid or missing parameters.");
            }

            var reportDetails = new ReportDetailsModel
            {
                Report = new ReportModel
                {
                    Key = key,
                    ReportSourceId = report.ReportSourceId,
                    Name = report.Name,
                    Description = report.Description,
                    IsActive = report.IsActive,
                    HasParameters = report.HasParameters,
                    CreatedByUser = report.CreatedByUser,
                    CreatedAtDate = report.CreatedAtDate,
                    LastUpdatedByUser = report.UpdatedByUser,
                    LastUpdatedAtDate = report.UpdatedAtDate
                },
                Parameters = report.HasParameters && report.Parameters != null && queryParams != null && report.Parameters.Length == queryParams.Count
                    ? report.Parameters.Select(p => new ReportParameterModel
                    {
                        Position = p.Position,
                        Name = p.Name,
                        SqlDataType = p.SqlDataType,
                        HasDefaultValue = p.HasDefaultValue,
                        CurrentValue = queryParams[p.Name],
                    })
                    : Array.Empty<ReportParameterModel>(),
                ColumnDefinitions = report.ColumnDefinitions.Select(c => new ReportColumnDefinitionModel
                {
                    ColumnId = c.ColumnId,
                    Name = c.Name,
                    SqlDataType = c.SqlDataType,
                    IsNullable = c.IsNullable,
                    IsIdentity = c.IsIdentity
                }).ToArray()
            };

            var reportBytes = await _reportService.GetReportDataAsBytesAsync(reportDetails);

            var fileName = $"{key}.xlsx";
            return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// Get all reports.
        /// </summary>
        /// <returns>A list of reports.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.Admin.GetReports)]
        public async Task<ActionResult<IEnumerable<ReportModel>>> GetAllReports()
        {
            _logger.LogInformation("Getting all reports.");
            var reports = await _reportService.GetAllReportsAsync();
            var reportModels = reports.Select(r => new ReportModel
            {
                Key = r.Key,
                ReportSourceId = r.ReportSourceId,
                Name = r.Name,
                Description = r.Description,
                HasParameters = r.HasParameters,
                IsActive = r.IsActive,
                CreatedByUser = r.CreatedByUser,
                CreatedAtDate = r.CreatedAtDate,
                LastUpdatedByUser = r.UpdatedByUser,
                LastUpdatedAtDate = r.UpdatedAtDate
            });
            return Ok(reportModels);
        }

        /// <summary>
        /// Get details of a report for an admin by key.
        /// </summary>
        /// <param name="key">The key of the report.</param>
        /// <returns>Report admin details.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.Admin.GetReportDetails)]
        public async Task<ActionResult<ReportAdminDetailsModel>> GetReportDetailsForAdmin(string key)
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
                }),
                ColumnDefinitions = report.ColumnDefinitions.Select(c => new ReportColumnDefinitionModel
                {
                    ColumnId = c.ColumnId,
                    Name = c.Name,
                    SqlDataType = c.SqlDataType,
                    IsNullable = c.IsNullable,
                    IsIdentity = c.IsIdentity
                }),
                CreatedByUser = report.CreatedByUser,
                CreatedAtDate = report.CreatedAtDate,
                LastUpdatedByUser = report.UpdatedByUser,
                LastUpdatedAtDate = report.UpdatedAtDate,
            };

            return Ok(reportAdminDetailsModel);
        }


        /// <summary>
        /// Gets the details of a report source by its ID.
        /// </summary>
        /// <param name="id">The ID of the report source.</param>
        /// <returns>A <see cref="ReportSourceDetailsModel"/> containing the report source details and its activity log.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.Admin.GetReportSourceDetails)]
        public async Task<ActionResult<ReportSourceDetailsModel>> GetReportSourceDetails(int id)
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
            var reportSourceDetails = new ReportSourceDetailsModel
            {
                Source = reportSourceModel,
                ActivityLog = reportSourceActivity.Select(a => new ReportSourceActivityModel
                {
                    ActivityType = a.ActivityType,
                    ActivityByUser = a.ActivityByUser,
                    ActivityDate = a.ActivityDate
                })
            };
            return Ok(reportSourceDetails);
        }


        /// <summary>
        /// Gets all available report sources for creating a new report.
        /// </summary>
        /// <returns>A <see cref="AvailableReportSourcesForNewReportResponse"/> containing all available report sources.</returns>
        [HttpGet(ApiRoutes.V1.Reporting.Admin.GetAvailableReportSourcesForNewReport)]
        public async Task<ActionResult<IEnumerable<ReportSourceModel>>> GetAvailableReportSourcesForNewReport()
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

            return Ok(reportSourceModels);
        }

        /// <summary>
        /// Create a new report.
        /// </summary>
        /// <param name="request">The new report request model.</param>
        /// <returns>The created report.</returns>
        [HttpPost(ApiRoutes.V1.Reporting.Admin.NewReport)]
        public async Task<IActionResult> CreateReport(NewReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for NewReportRequest.");
                return BadRequest(ModelState);
            }

            var newReport = new NewReportModel
            {
                Key = request.Key.ToLower(),
                ReportSourceId = request.ReportSourceId,
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CreatedByUser = this.User.Identity?.Name ?? Environment.UserName,
                CreatedAtDate = DateTime.UtcNow
            };

            _logger.LogInformation("Creating new report with key: {Key}", newReport.Key);
            var createdReport = await _reportService.CreateReportAsync(newReport);

            return CreatedAtAction(nameof(GetReportDetailsForAdmin), new { key = createdReport.Key }, newReport);
        }

        /// <summary>
        /// Update an existing report.
        /// </summary>
        /// <param name="request">The update report request model.</param>
        /// <returns>The updated report.</returns>
        [HttpPut(ApiRoutes.V1.Reporting.Admin.UpdateReport)]
        public async Task<IActionResult> UpdateReport(UpdateReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateReportRequest.");
                return BadRequest(ModelState);
            }

            var updateReport = new UpdateReportModel 
            { 
                Key = request.Key.ToLower(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                UpdatedByUser = this.User.Identity?.Name ?? Environment.UserName,
                UpdatedAtDate = DateTime.UtcNow
            };

            _logger.LogInformation("Updating report with key: {Key}", updateReport.Key);
            var updatedReport = await _reportService.UpdateReportAsync(updateReport);
            var reportModel = new ReportModel
            {
                Key = updatedReport.Key,
                ReportSourceId = updatedReport.ReportSourceId,
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
        /// Delete a report.
        /// </summary>
        /// <param name="request">The delete report request model.</param>
        [HttpDelete(ApiRoutes.V1.Reporting.Admin.DeleteReport)]
        public async Task<IActionResult> DeleteReport(DeleteReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for DeleteReportRequest.");
                return BadRequest(ModelState);
            }

            var deleteReportModel = new DeleteReportModel
            {
                Key = request.Key.ToLower()
            };

            _logger.LogInformation("Deleting report with key: {Key}", deleteReportModel.Key);
            await _reportService.DeleteReportAsync(deleteReportModel);
            return NoContent();
        }
    }
}
