import { ReportSourceModel } from './report-source-model';
import { ReportParameterModel } from './report-parameter-model';
import { ReportColumnDefinitionModel } from './report-column-definition-model';

export interface ReportAdminDetailsModel {
  key?: string;
  name?: string;
  description?: string;
  isActive: boolean;
  hasParameters: boolean;
  reportSourceId: number;
  reportSource: ReportSourceModel;
  parameters?: ReportParameterModel[];
  columnDefinitions?: ReportColumnDefinitionModel[];
  createdByUser?: string;
  createdAtDate: string;
  lastUpdatedByUser?: string;
  lastUpdatedAtDate?: string;
}
