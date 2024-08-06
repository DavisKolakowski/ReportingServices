import { useState } from 'react';
import { AlertColor } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import reportingClient from '../services/reporting-client';

const useDownloadReportButton = () => {
  const [downloadLoading, setDownloadLoading] = useState(false);
  const [snackbar, setSnackbar] = useState<{ message: string; severity: AlertColor } | null>(null);

  const downloadReport = async (key: string, parameters: Record<string, object>) => {
    setDownloadLoading(true);
    try {
      const response = await reportingClient.downloadReport(key, parameters);
      const blob = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      const currentDateTime = new Date().toISOString().replace(/[:.]/g, '-');
      a.href = url;
      a.download = `${key}-${currentDateTime}.xlsx`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
      setSnackbar({ message: 'Download started', severity: 'success' });
    } catch (error) {
      console.error('Error downloading report:', error);
      setSnackbar({ message: 'Download failed', severity: 'error' });
    } finally {
      setDownloadLoading(false);
    }
  };

  const handleSnackbarClose = () => {
    setSnackbar(null);
  };

  return {
    downloadReport,
    downloadLoading,
    snackbar,
    handleSnackbarClose,
  };
};

const useRefreshReportButton = () => {
  const [refreshLoading, setRefreshLoading] = useState(false);
  const [refreshSuccess, setRefreshSuccess] = useState(false);
  const [snackbar, setSnackbar] = useState<{ message: string; severity: AlertColor } | null>(null);
  const navigate = useNavigate();

  const handleRefreshClick = async (reportKey: string, parameters: Record<string, any>) => {
    const query = new URLSearchParams(parameters);
    setRefreshLoading(true);
    try {
      await navigate(`/reporting/index/${reportKey}?${query}`);
      setRefreshSuccess(true);
      setSnackbar({
        message: `Report ${reportKey} has been successfully refreshed${Object.keys(parameters).length ? ` with parameters ${JSON.stringify(parameters)}` : ''}`,
        severity: 'success'
      });
    } catch (error) {
      console.error('Refresh failed:', error);
      setSnackbar({
        message: 'Refresh failed',
        severity: 'error'
      });
    } finally {
      setRefreshLoading(false);
      setTimeout(() => setRefreshSuccess(false), 2000);
    }
  };

  const handleSnackbarClose = () => {
    setSnackbar(null);
  };

  return {
    refreshLoading,
    refreshSuccess,
    snackbar,
    handleSnackbarClose,
    handleRefreshClick
  };
};

const useShareReportButton = () => {
    const [shareSuccess, setShareSuccess] = useState(false);
    const [shareError, setShareError] = useState(false);
    const [snackbar, setSnackbar] = useState<{ message: string; severity: AlertColor } | null>(null);

    const handleShareClick = (reportKey: string, parameters: Record<string, any>) => {
        try {
            const query = new URLSearchParams(parameters);
            const url = `${window.location.origin}/reporting/index/${reportKey}?${query}`;
            navigator.clipboard.writeText(url);
            setShareSuccess(true);
            setSnackbar({ message: 'URL copied to clipboard', severity: 'success' });
            setTimeout(() => setShareSuccess(false), 2000);
        } catch (error) {
            console.error('Failed to share the report URL:', error);
            setShareError(true);
            setSnackbar({ message: 'Failed to copy URL', severity: 'error' });
            setTimeout(() => setShareError(false), 2000);
        }
    };

    const handleSnackbarClose = () => {
        setSnackbar(null);
    };

    return {
        handleShareClick,
        shareSuccess,
        shareError,
        snackbar,
        handleSnackbarClose,
    };
};

export { useDownloadReportButton, useRefreshReportButton, useShareReportButton };
