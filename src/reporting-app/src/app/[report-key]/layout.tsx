import { useEffect, useState } from 'react';
import { useRouter } from 'next/router';
import { UserApiRoutes } from '@/constants/api-routes';
import { ReportDetailsModel } from '@/models/report-details-model';
import { Container, CircularProgress, Button, Box } from '@mui/material';
import Link from 'next/link';

const fetchReportDetails = async (key: string) => {
  const res = await fetch(UserApiRoutes.GetReportDetails(key));
  if (!res.ok) {
    throw new Error('Failed to fetch report details');
  }
  return res.json();
};

const ReportLayout = ({ children }: { children: React.ReactNode }) => {
  const router = useRouter();
  const { reportKey } = router.query;
  const [reportDetails, setReportDetails] = useState<ReportDetailsModel | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (reportKey) {
      fetchReportDetails(reportKey as string)
        .then((data) => setReportDetails(data))
        .catch((err) => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [reportKey]);

  if (loading) {
    return <CircularProgress />;
  }

  if (!reportDetails) {
    return <p>Report not found</p>;
  }

  return (
    <Container>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box>
          <h1>{reportDetails.report.name}</h1>
          <p>{reportDetails.report.description}</p>
          <p>Created by: {reportDetails.report.createdByUser}</p>
          <p>Created at: {reportDetails.report.createdAtDate}</p>
          <Link href="/">
            <Button variant="contained">Back to Reports</Button>
          </Link>
        </Box>
        <Box>
          <Button variant="contained" color="primary" onClick={() => router.reload()}>
            Refresh Report Data
          </Button>
          <Button variant="contained" color="secondary" onClick={() => window.location.href = UserApiRoutes.DownloadReport(reportKey as string)}>
            Download Report
          </Button>
        </Box>
      </Box>
      {children}
    </Container>
  );
};

export default ReportLayout;

