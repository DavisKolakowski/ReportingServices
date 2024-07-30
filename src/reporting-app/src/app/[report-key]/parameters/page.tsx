import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import { UserApiRoutes } from '@/constants/api-routes';
import { ReportParameterModel } from '@/models/report-parameter-model';
import ReportParametersForm from '@/components/ReportParametersForm';
import { Container, CircularProgress } from '@mui/material';

const fetchReportParameters = async (key: string) => {
  const res = await fetch(UserApiRoutes.GetReportParameters(key));
  if (!res.ok) {
    throw new Error('Failed to fetch report parameters');
  }
  return res.json();
};

const Parameters = () => {
  const router = useRouter();
  const { reportKey } = router.query;
  const [parameters, setParameters] = useState<ReportParameterModel[]>([]);
  const [values, setValues] = useState<Record<string, any>>({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (reportKey) {
      fetchReportParameters(reportKey as string)
        .then((data) => setParameters(data))
        .catch((err) => console.error(err))
        .finally(() => setLoading(false));
    }
  }, [reportKey]);

  const handleChange = (name: string, value: any) => {
    setValues((prevValues) => ({ ...prevValues, [name]: value }));
  };

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    sessionStorage.setItem(`report-${reportKey}-parameters`, JSON.stringify(values));
    router.push(`/report/${reportKey}`);
  };

  if (loading) {
    return <CircularProgress />;
  }

  return (
    <Container>
      <ReportParametersForm parameters={parameters} values={values} onChange={handleChange} onSubmit={handleSubmit} />
    </Container>
  );
};

export default Parameters;

