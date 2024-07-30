namespace Reporting.Server.Constants
{
    public static class ApiRoutes
    {

        public static class V1
        {

            public static class Reporting
            {
                public const string GetReports = "api/v1/reporting/active-reports";
                public const string GetReportDetails = "api/v1/reporting/report/{key}";
                public const string GetReportParameters = "api/v1/reporting/report/{key}/parameters";
                public const string GetReportColumnDefinitions = "api/v1/reporting/report/{key}/columns";             
                public const string GetReportData = "api/v1/reporting/report/{key}/data";
                public const string DownloadReport = "api/v1/reporting/report/{key}/file";

            	public static class Admin
            	{             
                    public const string GetReports = "api/v1/admin/reporting/all-reports";
                    public const string GetReportDetails = "api/v1/admin/reporting/report/{key}";
                    public const string GetReportSourceDetails = "api/v1/admin/reporting/report-source/{id}";
                    public const string GetAvailableReportSourcesForNewReport = "api/v1/admin/reporting/report/unused-report-sources";
                    public const string NewReport = "api/v1/admin/reporting/report";
                    public const string UpdateReport = "api/v1/admin/reporting/report";
                    public const string DeleteReport = "api/v1/admin/reporting/report";
            	}
            }
        }
    }
}
