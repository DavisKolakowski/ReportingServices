import { SqlDataType } from '@/enums';

export interface ReportColumnDefinitionModel {
    columnId: number;
    name?: string;
    sqlDataType?: SqlDataType;
    isNullable: boolean;
    isIdentity: boolean;
}
  