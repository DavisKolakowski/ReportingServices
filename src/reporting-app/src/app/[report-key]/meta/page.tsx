import { Container } from '@mui/material';
import { ReportDetailsModel } from '@/models/report-details-model';

interface Props {
  reportDetails: ReportDetailsModel;
}

const Meta = ({ reportDetails }: Props) => {
  return (
    <Container>
      <h1>{reportDetails.report.name}</h1>
      <p>{reportDetails.report.description}</p>
      <p>Created by: {reportDetails.report.createdByUser}</p>
      <p>Created at: {reportDetails.report.createdAtDate}</p>
      <p>Last updated by: {reportDetails.report.lastUpdatedByUser}</p>
      <p>Last updated at: {reportDetails.report.lastUpdatedAtDate}</p>
    </Container>
  );
};

export default Meta;
