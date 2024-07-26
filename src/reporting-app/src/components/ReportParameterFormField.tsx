import React from 'react';
import {
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormGroup,
  SelectChangeEvent
} from '@mui/material';
import { getInputType } from '@/utils/sql-type-converter';
import { ReportParameterModel } from '@/models';

interface ReportParameterFormFieldProps {
  parameter: ReportParameterModel;
  onChange: (name: string, value: any) => void;
}

const ReportParameterFormField: React.FC<ReportParameterFormFieldProps> = ({
  parameter,
  onChange
}) => {
  const inputType = getInputType(parameter.sqlDataType);

  const handleChange = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | { name?: string; value: unknown }>
  ) => {
    onChange(parameter.name, event.target.value);
  };

  const handleSelectChange = (
    event: SelectChangeEvent<unknown>
  ) => {
    onChange(parameter.name, event.target.value);
  };

  const renderInputField = () => {
    switch (inputType) {
      case 'number':
        return (
          <TextField
            type={inputType}
            label={parameter.name}
            value={parameter.currentValue ?? ''}
            onChange={handleChange}
            fullWidth
          />
        );
      case 'checkbox':
        return (
          <FormControl fullWidth>
            <InputLabel>{parameter.name}</InputLabel>
            <Select
              value={parameter.currentValue ?? ''}
              onChange={handleSelectChange}
              label={parameter.name}
            >
              <MenuItem value="true">True</MenuItem>
              <MenuItem value="false">False</MenuItem>
            </Select>
          </FormControl>
        );
      case 'date':
      case 'datetime-local':
      case 'time':
        return (
          <TextField
            type={inputType}
            label={parameter.name}
            value={parameter.currentValue ?? ''}
            onChange={handleChange}
            fullWidth
            InputLabelProps={{
              shrink: true
            }}
          />
        );
      default:
        return (
          <TextField
            type={inputType}
            label={parameter.name}
            value={parameter.currentValue ?? ''}
            onChange={handleChange}
            fullWidth
          />
        );
    }
  };

  return (
    <FormGroup>
      {renderInputField()}
    </FormGroup>
  );
};

export default ReportParameterFormField;
