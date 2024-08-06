import React, { useState, useEffect } from 'react';
import { Input, FormControl, Typography } from '@mui/material';
import { ReportParameterModel } from '../models';
import { getInputTypeFromSqlTypeString } from '../utils';

interface ReportParameterFormInputProps {
  parameter: ReportParameterModel;
  initialValue?: string;
  onUpdateParameterValue: (param: { name: string; value: unknown }) => void;
}

const ReportParameterFormInput: React.FC<ReportParameterFormInputProps> = ({ parameter, initialValue = '', onUpdateParameterValue }) => {
  const [inputValue, setInputValue] = useState<string>(initialValue);

  useEffect(() => {
    onUpdateParameterValue({ name: parameter.name, value: inputValue });
  }, [inputValue, parameter.name, onUpdateParameterValue]);

  return (
    <FormControl fullWidth margin="normal">
      <Typography variant="subtitle1">{parameter.name}</Typography>
      <Input
        id={parameter.name}
        name={parameter.name}
        type={getInputTypeFromSqlTypeString(parameter.sqlDataType as string)}
        value={inputValue}
        onChange={(e) => setInputValue(e.target.value)}
      />
    </FormControl>
  );
};

export default ReportParameterFormInput;
