import React from 'react';
import { TextField, Button, Grid } from '@mui/material';
import { ReportParameterModel } from '@/models/report-parameter-model';

interface Props {
  parameters: ReportParameterModel[];
  values: Record<string, any>;
  onChange: (name: string, value: any) => void;
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void;
}

const ReportParametersForm: React.FC<Props> = ({ parameters, values, onChange, onSubmit }) => {
  const handleChange = (name: string) => (event: React.ChangeEvent<HTMLInputElement>) => {
    onChange(name, event.target.value);
  };

  return (
    <form onSubmit={onSubmit}>
      <Grid container spacing={2}>
        {parameters.map((param) => (
          <Grid item xs={12} key={param.name}>
            <TextField
              label={param.name}
              value={values[param.name] || ''}
              onChange={handleChange(param.name)}
              fullWidth
            />
          </Grid>
        ))}
      </Grid>
      <Button type="submit" variant="contained" color="primary">
        Submit
      </Button>
    </form>
  );
};

export default ReportParametersForm;



