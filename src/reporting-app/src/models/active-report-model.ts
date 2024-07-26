export interface ActiveReportModel {
    key: string;
    name: string;
    description: string;
    hasParameters: boolean;
    createdByUser: string;
    createdAtDate: Date;
    lastUpdatedByUser?: string;
    lastUpdatedAtDate?: Date;
}