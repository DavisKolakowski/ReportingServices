import axios, { AxiosResponse } from 'axios';
import { 
    ReportModel, 
    ReportDetailsModel, 
    ReportParameterModel, 
    ReportColumnDefinitionModel, 
    ReportDataModel, 
    ReportSourceModel, 
    ReportAdminDetailsModel, 
} from '../models';
import {
  NewReportRequest, 
  UpdateReportRequest, 
  DeleteReportRequest 
} from '../requests';


const baseURL = '/internalapi';

const api = axios.create({ baseURL: baseURL })

const ReportingClient = {
    async getReports(): Promise<AxiosResponse<ReportModel[]>> {
        return api.get('/v1/reporting/active-reports');
    },
    async getReportDetails(key: string): Promise<AxiosResponse<ReportDetailsModel>> {
        return api.get(`/v1/reporting/report/${key}`);
    },
    async getReportParameters(key: string): Promise<AxiosResponse<ReportParameterModel[]>> {
        return api.get(`/v1/reporting/report/${key}/parameters`);
    },
    async getReportColumnDefinitions(key: string): Promise<AxiosResponse<ReportColumnDefinitionModel[]>> {
        return api.get(`/v1/reporting/report/${key}/columns`);
    },
    async getReportData(key: string, parameters?: { [key: string]: object }): Promise<AxiosResponse<ReportDataModel>> {
        return api.get(`/v1/reporting/report/${key}/data`, { params: parameters });
    },
    async downloadReport(key: string, parameters?: { [key: string]: object }): Promise<AxiosResponse<Blob>> {
        return api.get(`/v1/reporting/report/${key}/file`, { params: parameters, responseType: 'blob' });
    },
    Admin: {
        async getReports(): Promise<AxiosResponse<ReportModel[]>> {
            return api.get('/v1/admin/reporting/all-reports');
        },
        async getReportDetails(key: string): Promise<AxiosResponse<ReportAdminDetailsModel>> {
            return api.get(`/v1/admin/reporting/report/${key}`);
        },
        async getReportSourceDetails(id: number): Promise<AxiosResponse<ReportSourceModel>> {
            return api.get(`/v1/admin/reporting/report-source/${id}`);
        },
        async getAvailableReportSourcesForNewReport(): Promise<AxiosResponse<ReportSourceModel[]>> {
            return api.get('/v1/admin/reporting/report/unused-report-sources');
        },
        async newReport(model: NewReportRequest): Promise<AxiosResponse<void>> {
            return api.post('/v1/admin/reporting/report', model);
        },
        async updateReport(model: UpdateReportRequest): Promise<AxiosResponse<void>> {
            return api.put('/v1/admin/reporting/report', model);
        },
        async deleteReport(model: DeleteReportRequest): Promise<AxiosResponse<void>> {
            return api.delete('/v1/admin/reporting/report', { data: model });
        }
    }
};

export default ReportingClient;
