import { useState, useEffect } from 'react';
import { UserApiRoutes } from '@/constants/api-routes';
import { ReportModel } from '@/models/report-model';
import { Container, List, ListItem, ListItemText, Button, CircularProgress } from '@mui/material';
import { useRouter } from 'next/router';

const fetchReports = async () => {
  const res = await fetch(UserApiRoutes.GetReports);
  if (!res.ok) {
    throw new Error('Failed to fetch reports');
  }
  return res.json();
};

const Reports = () => {
  const router = useRouter();
  const [reports, setReports] = useState<ReportModel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchReports()
      .then((data) => setReports(data))
      .catch((err) => console.error(err))
      .finally(() => setLoading(false));
  }, []);

  const handleDownload = (key: string) => {
    // Handle the download logic
    console.log(`Download report with key: ${key}`);
  };

  const handleView = (key: string, hasParameters: boolean) => {
    if (hasParameters) {
      router.push(`/report/${key}/parameters`);
    } else {
      router.push(`/report/${key}`);
    }
  };

  if (loading) {
    return <CircularProgress />;
  }

  return (
    <Container>
      <List>
        {reports.map((report) => (
          <ListItem key={report.key}>
            <ListItemText primary={report.name} secondary={report.description} />
            <Button variant="contained" color="primary" onClick={() => handleDownload(report.key)}>
              Download
            </Button>
            <Button variant="contained" color="secondary" onClick={() => handleView(report.key, report.hasParameters)}>
              View
            </Button>
          </ListItem>
        ))}
      </List>
    </Container>
  );
};

export default Reports;
