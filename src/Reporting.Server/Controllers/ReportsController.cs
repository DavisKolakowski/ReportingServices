namespace Reporting.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Reporting.Core.Contracts;
    using Reporting.Core.Entities;
    using Reporting.Core.Models;

    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActiveReports()
        {
            var reports = await _reportService.GetAllActiveReportsAsync();
            var reportModels = reports.Select(r => new ReportModel
            {
                Key = r.Key,
                Name = r.Name,
                Description = r.Description,
                IsActive = r.IsActive,
                HasParameters = r.HasParameters
            });

            return Ok(reportModels);
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<ReportDetailsResponse>> GetReportDetails(string key)
        {
            var report = await _reportService.GetReportDetailsAsync(key);
            if (report == null)
            {
                return NotFound();
            }
            var reportDetailsResponse = new ReportDetailsResponse
            {
                Model = new ReportDetailsModel
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
                    }).ToArray()
                }
            };

            return Ok(reportDetailsResponse);
        }

        [HttpPost("execute")]
        public async Task<ActionResult<ExecuteReportResponse>> ExecuteReport(ExecuteReportRequest request)
        {
            var executeReportModel = request.Model;

            if (!ModelState.IsValid || executeReportModel == null)
            {
                return BadRequest(ModelState);
            }

            var data = await _reportService.GetReportDataAsTableAsync(executeReportModel);

            var response = new ExecuteReportResponse()
            {
                Model = executeReportModel,
                Data = data
            };

            return Ok(response);
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadReport(DownloadReportRequest request)
        {
            var executeReportModel = request.Model;

            if (!ModelState.IsValid || executeReportModel == null)
            {
                return BadRequest(ModelState);
            }

            var reportBytes = await _reportService.GetReportDataAsBytesAsync(executeReportModel);

            var fileName = $"{executeReportModel.Key}.xlsx";
            return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(NewReportRequest request)
        {
            var newReport = request.Model;
            newReport.Key = newReport.Key.ToLower();

            var createdReport = await _reportService.CreateReportAsync(newReport);
            var reportModel = new ReportModel
            {
                Key = createdReport.Key,
                Name = createdReport.Name,
                Description = createdReport.Description,
                IsActive = createdReport.IsActive,
                HasParameters = createdReport.HasParameters
            };

            return CreatedAtAction(nameof(GetReportDetails), new { key = createdReport.Key }, reportModel);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReport(UpdateReportRequest request)
        {
            var updateReport = request.Model;
            updateReport.Key = updateReport.Key.ToLower();

            var updatedReport = await _reportService.UpdateReportAsync(updateReport);
            var reportModel = new ReportModel
            {
                Key = updatedReport.Key,
                Name = updatedReport.Name,
                Description = updatedReport.Description,
                IsActive = updatedReport.IsActive,
                HasParameters = updatedReport.HasParameters
            };

            return Ok(reportModel);
        }

        [HttpPut("activate")]
        public async Task<IActionResult> ActivateReport(ActivateReportRequest request)
        {
            await _reportService.ActivateReportAsync(request.Model);
            return NoContent();
        }

        [HttpPut("disable")]
        public async Task<IActionResult> DisableReport(DisableReportRequest request)
        {
            await _reportService.DisableReportAsync(request.Model);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReport(DeleteReportRequest request)
        {
            await _reportService.DeleteReportAsync(request.Model);
            return NoContent();
        }
    }
}
