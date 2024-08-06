import { DataGrid } from '@mui/x-data-grid';
import { Box } from '@mui/material';
import { ReportDataModel } from '../models';
import { useGridColumns, useGridRows } from '../hooks';

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
        getRowId={(row) => row.InternalId}
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
