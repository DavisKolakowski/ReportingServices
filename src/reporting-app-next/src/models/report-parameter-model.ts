export interface ReportParameterModel {
    position: number;
    name: string;
    sqlDataType?: string;
    hasDefaultValue: boolean;
    currentValue?: any;
}
  