import React, { useEffect } from 'react';
import ReportParameterFormInput from './ReportParameterFormInput';
import { ReportParameterModel } from '../models';

interface ReportParameterFormProps {
  parameters: ReportParameterModel[];
  initialValues?: Record<string, any>;
  onUpdateParameterValue: (param: { name: string; value: unknown }) => void;
}

const useParameterValues = (parameters: ReportParameterModel[], initialValues: Record<string, any>, onUpdateParameterValue: (param: { name: string; value: unknown }) => void) => {
  useEffect(() => {
    parameters.forEach(param => {
      if (initialValues[param.name]) {
        onUpdateParameterValue({ name: param.name, value: initialValues[param.name] });
      }
    });
  }, [initialValues, onUpdateParameterValue, parameters]);
};

const ReportParameterForm: React.FC<ReportParameterFormProps> = ({ parameters, initialValues = {}, onUpdateParameterValue }) => {
  useParameterValues(parameters, initialValues, onUpdateParameterValue);

  return (
    <div>
      {parameters.map((param) => (
        <ReportParameterFormInput
          key={param.name}
          parameter={param}
          initialValue={initialValues[param.name] || ''}
          onUpdateParameterValue={onUpdateParameterValue}
        />
      ))}
    </div>
  );
};

export default ReportParameterForm;


