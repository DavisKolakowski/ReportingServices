import { createTheme, Theme } from '@mui/material/styles';

const defaultAppTheme: Theme = createTheme({
  palette: {
    primary: {
      main: '#007fff',
      contrastText: '#ffffff',
    },
    secondary: {
      main: '#dc004e',
    },
    error: {
      main: '#f44336',
    },
    warning: {
      main: '#ffa726',
    },
    info: {
      main: '#2196f3',
    },
    success: {
      main: '#4caf50',
    },
    background: {
      default: '#f6f6f6',
      paper: '#ffffff',
    },
    text: {
      primary: '#333333',
      secondary: '#454545',
    },
  },
  typography: {
    fontFamily: 'Arial, Helvetica, sans-serif',
    fontSize: 14,
    h1: {
      fontSize: '2rem',
    },
    h2: {
      fontSize: '1.75rem',
    },
    h3: {
      fontSize: '1.5rem',
    },
    h4: {
      fontSize: '1.25rem',
    },
    h5: {
      fontSize: '1rem',
    },
    h6: {
      fontSize: '0.875rem',
    },
    body1: {
      fontSize: '1rem',
    },
    body2: {
      fontSize: '0.875rem',
    },
  },
  shape: {
    borderRadius: 3,
  }
});

export default defaultAppTheme;