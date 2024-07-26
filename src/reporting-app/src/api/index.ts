import UserClient from './reporting-user-client';
import AdminClient from './reporting-admin-client';

const ReportingClient = {
  User: UserClient,
  Admin: AdminClient
};

export default ReportingClient;

