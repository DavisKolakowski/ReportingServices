import { ReportParameterModel } from '../report-parameter-model';

export interface ReportParametersResponse {
    reportKey: string;
    model: ReportParameterModel[];
}