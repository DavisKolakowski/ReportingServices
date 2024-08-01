import React, { useState, useEffect } from 'react';
import { Box, Button, CircularProgress, Typography, Snackbar, Alert } from '@mui/material';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import { ReportDataModel } from '../models';
import reportingClient from '../services/reporting-client';
import ReportData from '../components/ReportData';
import ReportParameterForm from '../components/ReportParameterForm';
import ScrollToTopButton from '../components/ScrollToTopButton';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import RefreshIcon from '@mui/icons-material/Refresh';
import DownloadIcon from '@mui/icons-material/Download';
import DownloadingIcon from '@mui/icons-material/Downloading';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import ErrorIcon from '@mui/icons-material/Error';
import ShareIcon from '@mui/icons-material/Share';
import { green, red } from '@mui/material/colors';
import { useDownloadReportButton, useRefreshReportButton, useFetchReportDetails, useShareReportButton } from '../hooks';

const ReportView: React.FC = () => {
    const { reportKey } = useParams<{ reportKey: string }>();
    const location = useLocation();
    const navigate = useNavigate();

    const [data, setData] = useState<ReportDataModel['data'] | null>(null);
    const [loading, setLoading] = useState(true);
    const [parameters, setParameters] = useState<Record<string, any>>({});
    const [downloadSuccess, setDownloadSuccess] = useState(false);
    const [downloadError, setDownloadError] = useState(false);

    const { details, loading: loadingDetails, error: errorDetails } = useFetchReportDetails(reportKey!);
    const { downloadReport, downloadLoading, snackbar: downloadSnackbar, handleSnackbarClose: handleDownloadSnackbarClose } = useDownloadReportButton();
    const { refreshLoading, refreshSuccess, snackbar: refreshSnackbar, handleSnackbarClose: handleRefreshSnackbarClose, handleRefreshClick } = useRefreshReportButton();
    const { handleShareClick, snackbar: shareSnackbar, handleSnackbarClose: handleShareSnackbarClose } = useShareReportButton();

    useEffect(() => {
        if (reportKey) {
            const urlParams = new URLSearchParams(location.search);
            const params: Record<string, any> = {};
            urlParams.forEach((value, key) => {
                params[key] = value;
            });
            setParameters(params);

            const fetchReportData = async () => {
                try {
                    const response = await reportingClient.getReportData(reportKey, params);
                    setData(response.data.data);
                } catch (error) {
                    console.error('Failed to fetch report data:', error);
                } finally {
                    setLoading(false);
                }
            };

            fetchReportData();
        }
    }, [reportKey, location.search]);

    const handleDownloadClick = async () => {
        if (reportKey) {
            try {
                await downloadReport(reportKey, parameters);
                setDownloadSuccess(true);
                setTimeout(() => setDownloadSuccess(false), 2000);
            } catch {
                setDownloadError(true);
                setTimeout(() => setDownloadError(false), 2000);
            }
        }
    };

    const handleShareClickWrapper = () => {
        if (reportKey) {
            handleShareClick(reportKey, parameters);
        }
    };

    const handleRefreshButtonClick = async () => {
        handleRefreshClick(reportKey!, parameters);
    };

    const handleUpdateParameter = (param: { name: string; value: any }) => {
        setParameters((prevParams) => ({
            ...prevParams,
            [param.name]: param.value,
        }));
    };

    const handleBackClick = () => {
        navigate('/reporting/index');
    };

    const handleSnackbarClose = () => {
        if (refreshSnackbar) handleRefreshSnackbarClose();
        if (downloadSnackbar) handleDownloadSnackbarClose();
        if (shareSnackbar) handleShareSnackbarClose();
    };

    if (loading || loadingDetails) {
        return (
            <Box sx={{ height: 600, width: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                <CircularProgress />
            </Box>
        );
    }

    if (errorDetails) {
        return (
            <Box sx={{ height: 600, width: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                <Typography variant="h6" color="error">{errorDetails}</Typography>
            </Box>
        );
    }

    return (
        <Box marginLeft={1}>
            {details && (
                <Box mt={2}>
                    <Typography variant="h4">{details.report.name}</Typography>
                    <Typography variant="subtitle1">{details.report.description}</Typography>
                    <Typography variant="subtitle2">Created Date: {details.report.createdAtDate}</Typography>
                    <Typography variant="subtitle2">Author: {details.report.createdByUser}</Typography>
                    {details.parameters && details.parameters.length > 0 && (
                        <Box mt={2} maxWidth="sm">
                            <Typography variant="h6">Parameters:</Typography>
                            <ReportParameterForm
                                parameters={details.parameters}
                                initialValues={parameters}
                                onUpdateParameterValue={handleUpdateParameter}
                            />
                        </Box>
                    )}
                    <Box mt={2} display="flex" justifyContent="space-between" alignItems="center">
                        <Box display="flex" gap={1}>
                            <Button
                                variant="contained"
                                color="secondary"
                                onClick={handleBackClick}
                                startIcon={<ArrowBackIcon />}
                            >
                                Back
                            </Button>
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={handleRefreshButtonClick}
                                disabled={refreshLoading}
                                startIcon={refreshLoading ? <CircularProgress size={20} /> : (refreshSuccess ? <CheckCircleIcon /> : <RefreshIcon />)}
                                sx={refreshSuccess ? { bgcolor: green[500] } : {}}
                            >
                                {refreshLoading ? 'Refreshing...' : 'Refresh'}
                            </Button>
                        </Box>
                        <Box display="flex" gap={1}>
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={handleDownloadClick}
                                disabled={downloadLoading}
                                startIcon={
                                    downloadLoading
                                    ? <DownloadingIcon />
                                    : downloadSuccess
                                    ? <CheckCircleIcon />
                                    : downloadError
                                    ? <ErrorIcon />
                                    : <DownloadIcon />
                                }
                                sx={
                                    downloadSuccess
                                    ? { bgcolor: green[500] }
                                    : downloadError
                                    ? { bgcolor: red[500] }
                                    : {}
                                }
                            >
                                {
                                    downloadLoading
                                    ? 'Downloading...'
                                    : downloadSuccess
                                    ? 'Downloaded'
                                    : downloadError
                                    ? 'Failed'
                                    : 'Download'}
                            </Button>
                            <Button
                                variant="contained"
                                color="primary"
                                onClick={handleShareClickWrapper}
                                startIcon={<ShareIcon />}
                            >
                                Share
                            </Button>
                        </Box>
                    </Box>
                </Box>
            )}
            <Box mt={2}>
                {data ? (
                    <ReportData report={details?.report!} columnDefinitions={details?.columnDefinitions!} data={data} />
                ) : (
                    <Typography variant="h6" component="p">
                        No data available.
                    </Typography>
                )}
            </Box>
            {(refreshSnackbar || downloadSnackbar || shareSnackbar) && (
                <Snackbar
                    open
                    autoHideDuration={6000}
                    onClose={handleSnackbarClose}
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                >
                    <Alert onClose={handleSnackbarClose} severity={refreshSnackbar?.severity || downloadSnackbar?.severity || shareSnackbar?.severity}>
                        {refreshSnackbar?.message || downloadSnackbar?.message || shareSnackbar?.message}
                    </Alert>
                </Snackbar>
            )}
            <ScrollToTopButton />
        </Box>
    );
};

export default ReportView;
