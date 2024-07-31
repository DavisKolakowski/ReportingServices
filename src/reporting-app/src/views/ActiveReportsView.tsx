import React, { useState, useEffect } from 'react';
import { List, CircularProgress, Box, Typography, Button, Snackbar, Alert } from '@mui/material';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import AssessmentIcon from '@mui/icons-material/Assessment';
import DownloadIcon from '@mui/icons-material/Download';
import DownloadingIcon from '@mui/icons-material/Downloading';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import ErrorIcon from '@mui/icons-material/Error';
import { useNavigate } from 'react-router-dom';
import reportingClient from '../services/reporting-client';
import { ReportModel } from '../models';
import ReportParameterFormDialog from '../components/ReportParameterFormDialog';
import ScrollToTopButton from '../components/ScrollToTopButton';
import { useDownloadReportButton } from '../hooks';
import { ReportActionTypes } from '../enums';
import { LoadingState } from '../types';
import { green, red } from '@mui/material/colors';

const ActiveReportsView: React.FC = () => {
  const [reports, setReports] = useState<ReportModel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showDialog, setShowDialog] = useState(false);
  const [selectedReport, setSelectedReport] = useState<ReportModel | null>(null);
  const [currentAction, setCurrentAction] = useState<ReportActionTypes | null>(null);
  const [loadingReports, setLoadingReports] = useState<Record<string, LoadingState>>({});
  const navigate = useNavigate();
  const { downloadReport, snackbar, handleSnackbarClose } = useDownloadReportButton();

  useEffect(() => {
    const fetchReports = async () => {
      try {
        const response = await reportingClient.getReports();
        setReports(response.data);
      } catch (error) {
        console.error('Failed to fetch active reports:', error);
        setError('Failed to fetch reports');
      } finally {
        setLoading(false);
      }
    };

    fetchReports();
  }, []);

  const handleReportSelect = (key: string, parameters?: Record<string, object>) => {
    const query = new URLSearchParams(
      Object.fromEntries(Object.entries(parameters || {}).map(([k, v]) => [k, String(v)]))
    ).toString();
    navigate(`/reportcatalog/index/${key}?${query}`);
  };

  const handleDownloadSelect = async (key: string, parameters?: Record<string, object>) => {
    setLoadingReports((prev) => ({ ...prev, [key]: true }));
    try {
      await downloadReport(key, parameters || {});
      setLoadingReports((prev) => ({ ...prev, [key]: 'success' }));
    } catch {
      setLoadingReports((prev) => ({ ...prev, [key]: 'error' }));
    } finally {
      setTimeout(() => setLoadingReports((prev) => ({ ...prev, [key]: false })), 2000);
    }
  };

  const handleButtonClick = (report: ReportModel, action: ReportActionTypes) => {
    if (report.hasParameters) {
      setCurrentAction(action);
      setSelectedReport(report);
      setShowDialog(true);
    } else {
      switch (action) {
        case ReportActionTypes.VIEW: {
          handleReportSelect(report.key);
          break;
        }
        case ReportActionTypes.DOWNLOAD: {
          handleDownloadSelect(report.key);
          break;
        }
      }
    }
  };

  const handleDialogSubmit = async ({ key, parameters }: { key: string; parameters: Record<string, any> }) => {
    switch (currentAction) {
      case ReportActionTypes.VIEW: {
        handleReportSelect(key, parameters);
        break;
      }
      case ReportActionTypes.DOWNLOAD: {
        await handleDownloadSelect(key, parameters);
        break;
      }
    }
    setShowDialog(false);
    setSelectedReport(null);
  };

  const closeDialog = () => {
    setShowDialog(false);
    setSelectedReport(null);
  };

  if (loading) {
    return (
      <Box sx={{ height: 600, width: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ height: 600, width: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
        <Typography variant="h6" color="error">{error}</Typography>
      </Box>
    );
  }

  if (!reports.length) {
    return (
      <Box sx={{ height: 600, width: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
        <Typography variant="h6">No active reports available</Typography>
      </Box>
    );
  }

  return (
    <Box mt={2}>
      <Typography variant="h4" gutterBottom marginLeft={2}>
        Custom Reports
      </Typography>
      <List>
        {reports.map((item) => (
          <ListItem key={item.key}>
            <ListItemText primary={item.name} secondary={item.description} />
            <Box display="flex" gap={1}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => handleButtonClick(item, ReportActionTypes.VIEW)}
                disabled={loadingReports[item.key] === true}
                startIcon={<AssessmentIcon />}
              >
                View
              </Button>
              <Button
                variant="contained"
                color="secondary"
                onClick={() => handleButtonClick(item, ReportActionTypes.DOWNLOAD)}
                disabled={loadingReports[item.key] === true}
                startIcon={
                  loadingReports[item.key] === true
                    ? <DownloadingIcon />
                    : loadingReports[item.key] === 'success'
                    ? <CheckCircleIcon />
                    : loadingReports[item.key] === 'error'
                    ? <ErrorIcon />
                    : <DownloadIcon />
                }
                sx={
                  loadingReports[item.key] === 'success'
                    ? { bgcolor: green[500] }
                    : loadingReports[item.key] === 'error'
                    ? { bgcolor: red[500] }
                    : {}
                }
              >
                {loadingReports[item.key] === true
                  ? 'Processing...'
                  : loadingReports[item.key] === 'success'
                  ? 'Downloaded'
                  : loadingReports[item.key] === 'error'
                  ? 'Failed'
                  : 'Download'}
              </Button>
            </Box>
            {selectedReport && selectedReport.key === item.key && (
              <ReportParameterFormDialog
                reportKey={item.key}
                showDialog={showDialog}
                onClose={closeDialog}
                onSubmit={handleDialogSubmit}
              />
            )}
          </ListItem>
        ))}
      </List>
      <ScrollToTopButton />
      {snackbar && (
        <Snackbar
          open
          autoHideDuration={6000}
          onClose={handleSnackbarClose}
          anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        >
          <Alert onClose={handleSnackbarClose} severity={snackbar.severity}>
            {snackbar.message}
          </Alert>
        </Snackbar>
      )}
    </Box>
  );
};

export default ActiveReportsView;
