import reportingClientBase from './reporting-client-base';
import { AllReportsResponse, ReportAdminDetailsResponse, AvailableReportSourcesForNewReportResponse } from '@/models/responses';
import { NewReportRequest, UpdateReportRequest, ActivateReportRequest, DisableReportRequest, DeleteReportRequest } from '@/models/requests';

const AdminClient = {
  getAllReports: async (): Promise<AllReportsResponse> => {
    const response = await reportingClientBase.get<AllReportsResponse>('/v1/admin/reporting/all-reports');
    return response.data;
  },
  
  getReportDetailsForAdmin: async (key: string): Promise<ReportAdminDetailsResponse> => {
    const response = await reportingClientBase.get<ReportAdminDetailsResponse>(`/v1/admin/reporting/report/${key}`);
    return response.data;
  },
  
  getAvailableReportSourcesForNewReport: async (): Promise<AvailableReportSourcesForNewReportResponse> => {
    const response = await reportingClientBase.get<AvailableReportSourcesForNewReportResponse>('/v1/admin/reporting/report/unused-report-sources');
    return response.data;
  },
  
  createReport: async (request: NewReportRequest): Promise<void> => {
    await reportingClientBase.post('/v1/admin/reporting/report', request);
  },
  
  updateReport: async (request: UpdateReportRequest): Promise<void> => {
    await reportingClientBase.put('/v1/admin/reporting/report', request);
  },
  
  activateReport: async (request: ActivateReportRequest): Promise<void> => {
    await reportingClientBase.put('/v1/admin/reporting/report/activation', request);
  },
  
  deactivateReport: async (request: DisableReportRequest): Promise<void> => {
    await reportingClientBase.put('/v1/admin/reporting/report/deactivation', request);
  },
  
  deleteReport: async (request: DeleteReportRequest): Promise<void> => {
    await reportingClientBase.delete('/v1/admin/reporting/report', { data: request });
  },
};

export default AdminClient;

