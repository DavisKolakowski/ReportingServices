import { useState, useEffect } from 'react';
import reportingClient from '../services/reporting-client';
import { ReportParameterModel } from '../models';

const useReportParameters = (reportKey: string, showDialog: boolean) => {
  const [paramResponse, setParamResponse] = useState<{ parameters?: ReportParameterModel[] } | null>(null);
  const [parameterValues, setParameterValues] = useState<Record<string, unknown>>({});

  useEffect(() => {
    if (showDialog) {
      const fetchParameters = async () => {
        try {
          const response = await reportingClient.getReportParameters(reportKey);
          setParamResponse({ parameters: response.data });
        } catch (error) {
          console.error('Failed to fetch report parameters:', error);
        }
      };
      fetchParameters();
    }
  }, [reportKey, showDialog]);

  const updateParameterValue = (param: { name: string; value: unknown }) => {
    setParameterValues((prev) => ({ ...prev, [param.name]: param.value }));
  };

  return {
    paramResponse,
    parameterValues,
    updateParameterValue,
  };
};

export default useReportParameters;
