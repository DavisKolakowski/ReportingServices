import { SqlDataType } from "./enums";

export interface ReportParameterModel {
    position: number;
    name: string;
    sqlDataType: SqlDataType;
    hasDefaultValue: boolean;
    currentValue?: any;
}
