import { ReportModel } from './report-model';
import { ReportParameterModel } from './report-parameter-model';
import { ReportColumnDefinitionModel } from './report-column-definition-model';

export interface ReportDataModel {
  report: ReportModel;
  parameters?: ReportParameterModel[];
  columnDefinitions: ReportColumnDefinitionModel[];
  data?: Record<string, any>[];
}
