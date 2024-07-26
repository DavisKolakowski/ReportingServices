import { ReportParameterModel } from './report-parameter-model';
import { ReportColumnDefinitionModel } from './report-column-definition-model';
import { ReportModel } from './report-model';

export interface ReportDetailsModel {
    report: ReportModel;
    parameters?: ReportParameterModel[];
    columnDefinitions: ReportColumnDefinitionModel[];
}
