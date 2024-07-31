import defaultTheme from "@/themes/default-theme";
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import type { Metadata } from "next";
import { Inter } from "next/font/google";

const theme = createTheme(defaultTheme);

const inter = Inter({ subsets: ["latin"] });

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {children}
    </ThemeProvider>
  );
}
