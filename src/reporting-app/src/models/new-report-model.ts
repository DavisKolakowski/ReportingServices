export interface NewReportModel {
    key: string;
    reportSourceId: number;
    name: string;
    description: string;
    isActive: boolean;
    createdByUser: string;
    createdAtDate: Date;
}
