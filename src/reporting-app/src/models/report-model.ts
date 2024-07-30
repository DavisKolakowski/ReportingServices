export interface ReportModel {
    key: string;
    reportSourceId: number;
    name: string;
    description?: string;
    isActive: boolean;
    hasParameters: boolean;
    createdByUser?: string;
    createdAtDate: string;
    lastUpdatedByUser?: string;
    lastUpdatedAtDate?: string;
}
  