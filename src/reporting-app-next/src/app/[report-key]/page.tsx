import { useEffect, useState } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { UserApiRoutes } from '@/constants/api-routes';
import { ReportDetailsModel } from '@/models/report-details-model';
import { ReportDataModel } from '@/models/report-data-model';
import { Container, CircularProgress, Button } from '@mui/material';
import ReportParametersForm from '@/components/ReportParametersForm';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { getGridColumnType } from '@/utils/sql-type-converter';
import { SqlDataType } from '@/enums';

const fetchReportDetails = async (key: string) => {
  const res = await fetch(UserApiRoutes.GetReportDetails(key));
  if (!res.ok) {
    throw new Error('Failed to fetch report details');
  }
  return res.json();
};

const fetchReportData = async (key: string, parameters: Record<string, any> = {}) => {
  const queryString = new URLSearchParams(parameters).toString();
  const res = await fetch(`${UserApiRoutes.GetReportData(key)}?${queryString}`);
  if (!res.ok) {
    throw new Error('Failed to fetch report data');
  }
  return res.json();
};

const Report = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const reportKey = searchParams.get('reportKey');
  const [reportDetails, setReportDetails] = useState<ReportDetailsModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [parameters, setParameters] = useState<Record<string, any>>({});
  const [reportData, setReportData] = useState<ReportDataModel | null>(null);
  const [dataLoading, setDataLoading] = useState(false);

  useEffect(() => {
    if (reportKey) {
      fetchReportDetails(reportKey)
        .then((data) => setReportDetails(data))
        .catch((err) => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [reportKey]);

  useEffect(() => {
    const storedParameters = localStorage.getItem(`report-${reportKey}-parameters`);
    if (storedParameters) {
      setParameters(JSON.parse(storedParameters));
    }
  }, [reportKey]);

  const handleDownload = async () => {
    const queryString = new URLSearchParams(parameters).toString();
    const res = await fetch(`${UserApiRoutes.DownloadReport(reportKey as string)}?${queryString}`);
    if (!res.ok) {
      throw new Error('Failed to download report');
    }
    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${reportKey}.pdf`;
    a.click();
  };

  const handleRefreshData = async () => {
    setDataLoading(true);
    try {
      const data = await fetchReportData(reportKey as string, parameters);
      setReportData(data);
    } catch (error) {
      console.error(error);
    } finally {
      setDataLoading(false);
    }
  };

  const handleParameterChange = (name: string, value: any) => {
    setParameters((prev) => ({ ...prev, [name]: value }));
  };

  const handleParameterSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    localStorage.setItem(`report-${reportKey}-parameters`, JSON.stringify(parameters));
    handleRefreshData();
  };

  if (loading) {
    return <CircularProgress />;
  }

  if (!reportDetails) {
    return <p>Report not found</p>;
  }

  const columns: GridColDef[] = (reportData?.columnDefinitions || []).map((col) => ({
    field: col.name ?? '',
    headerName: col.name ?? '',
    type: getGridColumnType(col.sqlDataType as SqlDataType),
    width: 150,
  }));

  const rows = (reportData?.data || []).map((row, index) => ({
    id: index,
    ...row,
  }));

  return (
    <Container>
      <h1>{reportDetails.report.name}</h1>
      <p>{reportDetails.report.description}</p>
      {reportDetails.parameters && (
        <ReportParametersForm
          parameters={reportDetails.parameters}
          values={parameters}
          onChange={handleParameterChange}
          onSubmit={handleParameterSubmit}
        />
      )}
      <Button onClick={handleDownload} variant="contained" color="primary">
        Download
      </Button>
      <Button onClick={handleRefreshData} variant="contained" color="secondary">
        Refresh Data
      </Button>
      {dataLoading ? (
        <CircularProgress />
      ) : (
        <DataGrid rows={rows} columns={columns} />
      )}
    </Container>
  );
};

export default Report;
