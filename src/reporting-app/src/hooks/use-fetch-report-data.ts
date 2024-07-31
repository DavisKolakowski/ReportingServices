import { useState, useEffect } from 'react';
import reportingClient from '../services/reporting-client';
import { ReportDataModel } from '../models';

const useFetchReportData = (reportKey: string, parameters: Record<string, any>) => {
  const [data, setData] = useState<ReportDataModel['data'] | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchReportData = async () => {
      try {
        const response = await reportingClient.getReportData(reportKey, parameters);
        setData(response.data.data);
      } catch (error) {
        console.error('Failed to fetch report data:', error);
        setError('Failed to fetch report data');
      } finally {
        setLoading(false);
      }
    };

    fetchReportData();
  }, [reportKey, parameters]);

  return { data, loading, error };
};

export default useFetchReportData;