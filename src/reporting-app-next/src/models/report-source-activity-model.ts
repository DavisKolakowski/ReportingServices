import { ActivityType } from '@/enums';

export interface ReportSourceActivityModel {
  activityType: ActivityType;
  activityByUser?: string;
  activityDate: string;
}
