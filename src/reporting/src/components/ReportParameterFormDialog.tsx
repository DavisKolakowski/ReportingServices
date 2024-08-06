import React from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Box, Typography } from '@mui/material';
import { useReportParameters } from '../hooks';
import ReportParameterForm from './ReportParameterForm';

interface ReportParameterFormDialogProps {
  reportKey: string;
  showDialog: boolean;
  onClose: () => void;
  onSubmit: (params: { key: string; parameters: Record<string, unknown> }) => void;
}

const ReportParameterFormDialog: React.FC<ReportParameterFormDialogProps> = ({
  reportKey,
  showDialog,
  onClose,
  onSubmit,
}) => {
  const { paramResponse, parameterValues, updateParameterValue } = useReportParameters(reportKey, showDialog);

  const handleSubmit = async () => {
    console.log('Submitting parameters:', parameterValues);
    await onSubmit({ key: reportKey, parameters: parameterValues });
    onClose();
  };

  return (
    <Dialog open={showDialog} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          <Typography variant="h6">Report Parameters</Typography>
        </Box>
      </DialogTitle>
      <DialogContent dividers>
        <ReportParameterForm parameters={paramResponse?.parameters || []} onUpdateParameterValue={updateParameterValue} />
      </DialogContent>
      <DialogActions>
        <Button color="secondary" onClick={onClose}>
          Cancel
        </Button>
        <Button color="primary" onClick={handleSubmit}>
          Submit
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default ReportParameterFormDialog;
