import { ReportSourceModel } from './report-source-model';
import { ReportSourceActivityModel } from './report-source-activity-model';

export interface ReportSourceDetailsModel {
  source: ReportSourceModel;
  activityLog?: ReportSourceActivityModel[];
}
