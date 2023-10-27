import React from 'react';
import './App.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './photos/HomePage';
import SettingsPage from './settings/SettingsPage';
import { Container, createTheme, CssBaseline, ThemeProvider } from '@mui/material';
import CollectionsIcon from '@mui/icons-material/Collections';
import SettingsIcon from '@mui/icons-material/Settings';
import { PageRouteInfo } from './nav-bar/pageRouteInfo';
import { NavBar } from './nav-bar/NavBar';
import { useAppSelector } from './storeHooks';
import { selectTheme } from './appSlice';

const pages: PageRouteInfo[] = [
  {
    key: 'home',
    route: '/',
    displayName: 'All Photos',
    element: <HomePage />,
    iconElement: <CollectionsIcon fontSize="small" />,
  },
  {
    key: 'settings',
    route: '/settings',
    displayName: 'Settings',
    element: <SettingsPage />,
    iconElement: <SettingsIcon fontSize="small" />,
  },
];
function App() {
  const mode = useAppSelector(selectTheme);
  const theme = createTheme({
    palette: {
      mode: mode,
    },
  });

  return (
    <Router>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <NavBar routesInfo={pages} />
        <Container maxWidth={false}>
          <Routes>
            {pages.map((page) => (
              <Route path={page.route} element={page.element} key={page.key} />
            ))}
          </Routes>
        </Container>
      </ThemeProvider>
    </Router>
  );
}

export default App;
