import reportingClientBase from './reporting-client-base';
import { ActiveReportsResponse, ReportParametersResponse, ReportDetailsResponse, ExecuteReportResponse } from '@/models/responses';
import { ExecuteReportRequest, DownloadReportRequest } from '@/models/requests';

const UserClient = {
  getAllActiveReports: async (): Promise<ActiveReportsResponse> => {
    const response = await reportingClientBase.get<ActiveReportsResponse>('/v1/reporting/active-reports');
    return response.data;
  },

  getReportParameters: async (key: string): Promise<ReportParametersResponse> => {
    const response = await reportingClientBase.get<ReportParametersResponse>(`/v1/reporting/report/${key}/parameters`);
    return response.data;
  },  
  
  getReportDetails: async (key: string): Promise<ReportDetailsResponse> => {
    const response = await reportingClientBase.get<ReportDetailsResponse>(`/v1/reporting/report/${key}`);
    return response.data;
  },
  
  executeReport: async (request: ExecuteReportRequest): Promise<ExecuteReportResponse> => {
    const response = await reportingClientBase.post<ExecuteReportResponse>('/v1/reporting/report/data', request);
    return response.data;
  },
  
  downloadReport: async (request: DownloadReportRequest): Promise<Blob> => {
    const response = await reportingClientBase.post('/v1/reporting/report/file', request, { responseType: 'blob' });
    return response.data;
  },
};

export default UserClient;

