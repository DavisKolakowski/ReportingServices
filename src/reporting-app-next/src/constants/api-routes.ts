const getUserRoute = (endpoint: string): string => {
    return `https://localhost:7260/api/v1/${endpoint}`;
};

const getAdminRoute = (endpoint: string): string => {
    return `https://localhost:7260/api/v1/${endpoint}`;
};

const UserApiRoutes = {
    GetReports: getUserRoute("reporting/active-reports"),
    GetReportDetails: (key: string) => getUserRoute(`reporting/report/${key}`),
    GetReportParameters: (key: string) => getUserRoute(`reporting/report/${key}/parameters`),
    GetReportColumnDefinitions: (key: string) => getUserRoute(`reporting/report/${key}/columns`),    
    GetReportData: (key: string) => getUserRoute(`reporting/report/${key}/data`),
    DownloadReport: (key: string) => getUserRoute(`reporting/report/${key}/file`),
};

const AdminApiRoutes = {
    GetReports: getAdminRoute("admin/reporting/all-reports"),
    GetReportDetails: (key: string) => getAdminRoute(`admin/reporting/report/${key}`),
    GetReportSourceDetails: (id: number) => getAdminRoute(`admin/reporting/report-source/${id}`),
    GetAvailableReportSourcesForNewReport: getAdminRoute("admin/reporting/report/unused-report-sources"),
    NewReport: getAdminRoute("admin/reporting/report"),
    UpdateReport: getAdminRoute("admin/reporting/report"),
    DeleteReport: getAdminRoute("admin/reporting/report"),
};

export { UserApiRoutes, AdminApiRoutes };
  