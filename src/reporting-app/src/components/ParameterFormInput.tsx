import React from 'react';
import { TextField, MenuItem } from '@mui/material';
import { ReportParameterModel } from '@/models/report-parameter-model';

interface Props {
  parameter: ReportParameterModel;
  value: any;
  onChange: (name: string, value: any) => void;
}

const ParameterFormInput: React.FC<Props> = ({ parameter, value, onChange }) => {
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    onChange(parameter.name, event.target.value);
  };

  switch (parameter.sqlDataType) {
    case 'int':
    case 'float':
    case 'decimal':
      return (
        <TextField
          label={parameter.name}
          value={value || ''}
          onChange={handleChange}
          type="number"
          fullWidth
        />
      );
    case 'bit':
      return (
        <TextField
          label={parameter.name}
          value={value || ''}
          onChange={handleChange}
          select
          fullWidth
        >
          <MenuItem value="true">True</MenuItem>
          <MenuItem value="false">False</MenuItem>
        </TextField>
      );
    case 'datetime':
      return (
        <TextField
          label={parameter.name}
          value={value || ''}
          onChange={handleChange}
          type="datetime-local"
          fullWidth
        />
      );
    default:
      return (
        <TextField
          label={parameter.name}
          value={value || ''}
          onChange={handleChange}
          fullWidth
        />
      );
  }
};

export default ParameterFormInput;
