import { ActivityType, ReportSourceType } from '@/enums';

export interface ReportSourceModel {
  id: number;
  type: ReportSourceType;
  schema?: string;
  name?: string;
  fullName?: string;
  lastActivityType: ActivityType;
  lastActivityByUser?: string;
  lastActivityDate: string;
}
