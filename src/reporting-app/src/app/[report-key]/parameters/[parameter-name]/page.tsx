import { Container } from '@mui/material';
import { ReportParameterModel } from '@/models/report-parameter-model';

interface Props {
  parameter: ReportParameterModel;
  value: any;
}

const Parameter = ({ parameter, value }: Props) => {
  return (
    <Container>
      <h1>{parameter.name}</h1>
      <p>Current value: {value}</p>
    </Container>
  );
};

export default Parameter;
