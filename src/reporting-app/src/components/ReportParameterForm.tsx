import React from 'react';
import { Button, Grid } from '@mui/material';
import ReportParameterFormField from './ReportParameterFormField';
import { ReportParameterModel } from '@/models';

interface ReportParameterFormProps {
  parameters: ReportParameterModel[];
  onSubmit: () => void;
  onUpdateParameter: (name: string, value: any) => void;
}

const ReportParameterForm: React.FC<ReportParameterFormProps> = ({ parameters, onSubmit, onUpdateParameter }) => {
  return (
    <form onSubmit={(e) => { e.preventDefault(); onSubmit(); }}>
      <Grid container spacing={2}>
        {parameters.map((param) => (
          <Grid item xs={12} key={param.name}>
            <ReportParameterFormField parameter={param} onChange={onUpdateParameter} />
          </Grid>
        ))}
      </Grid>
      <Button type="submit" variant="contained" color="primary">Submit</Button>
    </form>
  );
};

export default ReportParameterForm;
