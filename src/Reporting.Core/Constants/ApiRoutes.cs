namespace Reporting.Core.Constants
{
    public static class ApiRoutes
    {
        private const string Root = "api";

        public static class V1
        {
            private const string Base = Root + "/v1";

            public static class Reporting
            {
                public const string GetAllActiveReports = Base + "/reporting/active-reports";
                public const string GetReportDetailsForUser = Base + "/reporting/report/{key}";
                public const string ExecuteReport = Base + "/reporting/report/data";
                public const string DownloadReport = Base + "/reporting/report/file";
            }

            public static class Admin
            {
                private const string AdminBase = Base + "/admin";
                public static class Reporting
                {
                    public const string GetAllReports = AdminBase + "/reporting/all-reports";
                    public const string GetReportDetailsForAdmin = AdminBase + "/reporting/report/{key}";
                    public const string GetReportSourceDetails = AdminBase + "/reporting/report-source/{id}";
                    public const string GetAvailableReportSourcesForNewReport = AdminBase + "/reporting/report/unused-report-sources";
                    public const string NewReport = AdminBase + "/reporting/report";
                    public const string UpdateReport = AdminBase + "/reporting/report";
                    public const string ActivateReport = AdminBase + "/reporting/report/activation";
                    public const string DeactivateReport = AdminBase + "/reporting/report/deactivation";
                    public const string DeleteReport = AdminBase + "/reporting/report";
                }
            }
        }
    }
}
