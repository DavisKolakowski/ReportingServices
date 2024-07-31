import { useEffect, useState } from 'react';
import { useRouter } from 'next/router';
import { UserApiRoutes } from '@/constants/api-routes';
import { ReportColumnDefinitionModel } from '@/models/report-column-definition-model';
import { Container, CircularProgress } from '@mui/material';

const fetchReportColumns = async (key: string) => {
  const res = await fetch(UserApiRoutes.GetReportColumnDefinitions(key));
  if (!res.ok) {
    throw new Error('Failed to fetch report columns');
  }
  return res.json();
};

const Columns = () => {
  const router = useRouter();
  const { reportKey } = router.query;
  const [columns, setColumns] = useState<ReportColumnDefinitionModel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (reportKey) {
      fetchReportColumns(reportKey as string)
        .then((data) => setColumns(data))
        .catch((err) => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [reportKey]);

  if (loading) {
    return <CircularProgress />;
  }

  return (
    <Container>
      <h1>Column Definitions</h1>
      <ul>
        {columns.map((column) => (
          <li key={column.columnId}>
            {column.name} ({column.sqlDataType})
          </li>
        ))}
      </ul>
    </Container>
  );
};

export default Columns;
