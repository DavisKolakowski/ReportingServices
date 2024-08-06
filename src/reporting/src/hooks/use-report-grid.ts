import React from 'react';
import { GridColDef } from '@mui/x-data-grid';
import { ReportColumnDefinitionModel } from '../models';
import { getGridColumnType } from '../utils';
import { SqlDataType } from '../enums';

export const useGridColumns = (columnDefinitions: ReportColumnDefinitionModel[]): GridColDef[] => {
  return React.useMemo(
    () =>
      columnDefinitions
        .filter((col) => col.name !== 'InternalId')
        .map((col) => {
          const columnType = getGridColumnType(col.sqlDataType as SqlDataType);
          return {
            field: col.name ?? '',
            headerName: col.name ?? '',
            type: columnType,
            flex: 1,
            sortable: true,
            valueGetter: (params) => {
              const value = params;
              if (columnType === 'date' || columnType === 'dateTime') {
                return value ? new Date(params) : '';
              }
              return value;
            },
          };
        }),
    [columnDefinitions]
  );
};

export const useGridRows = (data: any[]): any[] => {
  return React.useMemo(() => data.map((row, index) => ({ id: index, ...row })), [data]);
};
