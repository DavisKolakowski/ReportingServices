import React from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Box } from '@mui/material';
import { ReportDataModel, ReportColumnDefinitionModel } from '../models';
import { getGridColumnType } from '../utils';
import { SqlDataType } from '../enums';

const useGridColumns = (columnDefinitions: ReportColumnDefinitionModel[]): GridColDef[] => {
  return React.useMemo(() => 
    columnDefinitions
      .filter((col) => col.name !== 'InternalId')
      .map((col) => ({
        field: col.name ?? '',
        headerName: col.name ?? '',
        type: getGridColumnType(col.sqlDataType as SqlDataType),
        flex: 1,
        sortable: true,
      })), [columnDefinitions]
  );
};

const useGridRows = (data: any[]): any[] => {
  return React.useMemo(() => 
    data.map((row, index) => ({ id: index, ...row })), 
    [data]
  );
};

export default function ReportData({ report, columnDefinitions, data }: ReportDataModel) {
  const columns = useGridColumns(columnDefinitions);
  const rows = useGridRows(data ?? []);  

  if (!data || !columnDefinitions) {
    return <div>No data available.</div>;
  }

  return (
    <Box>
      <DataGrid 
        rows={rows}
        columns={columns}
        getRowId={(row) => row.id}
        initialState={{
          pagination: {
            paginationModel: {
              pageSize: 10,
            },
          },
        }}
        pageSizeOptions={[10, 25, 50, 100]}
        autoHeight={true}
      />
    </Box>
  );
}
