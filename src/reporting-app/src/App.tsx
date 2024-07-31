import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ThemeProvider } from '@mui/material';
import { defaultAppTheme } from './theme/index';
import ReportView from './views/ReportView';
import ActiveReportsView from './views/ActiveReportsView';

function App() {
  return (
    <ThemeProvider theme={defaultAppTheme}>
      <Router>
          <Routes>
            <Route path="reportcatalog/index/" element={<ActiveReportsView />} />
            <Route path="reportcatalog/index/:reportKey" element={<ReportView />} />
          </Routes>
      </Router>
    </ThemeProvider>
  );
}

export default App;