import { ActivityType, ReportSourceSqlObjectType } from './enums';

export interface ReportSourceModel {
    id: number;
    type: ReportSourceSqlObjectType;
    schema: string;
    name: string;
    fullName: string;
    lastActivityType: ActivityType;
    lastActivityByUser: string;
    lastActivityDate: Date;
}
