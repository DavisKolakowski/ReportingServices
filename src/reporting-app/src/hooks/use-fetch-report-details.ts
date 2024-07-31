import { useState, useEffect } from 'react';
import reportingClient from '../services/reporting-client';
import { ReportDetailsModel } from '../models';

const useFetchReportDetails = (reportKey: string) => {
  const [details, setDetails] = useState<ReportDetailsModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchDetails = async () => {
      try {
        const response = await reportingClient.getReportDetails(reportKey);
        setDetails(response.data);
      } catch (error) {
        console.error('Failed to fetch report details:', error);
        setError('Failed to fetch report details');
      } finally {
        setLoading(false);
      }
    };

    fetchDetails();
  }, [reportKey]);

  return { details, loading, error };
};

export default useFetchReportDetails;