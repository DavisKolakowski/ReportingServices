import { ActivityType } from './enums/activity-type';

export interface ReportSourceActivityModel {
    activityType: ActivityType;
    activityByUser: string;
    activityDate: Date;
}
