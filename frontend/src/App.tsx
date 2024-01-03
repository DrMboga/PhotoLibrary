import React from 'react';
import './App.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Container, createTheme, CssBaseline, ThemeProvider } from '@mui/material';
import { NavBar } from './nav-bar/NavBar';
import { useAppSelector } from './storeHooks';
import { selectTheme } from './appSlice';
import { useAuth } from './keycloak-auth/useAuth';
import { pages } from './Routes';

function App() {
  const keycloakRef = useAuth();

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
        <NavBar routesInfo={pages} keycloakRef={keycloakRef} />
        <Container maxWidth={false}>
          <Routes>
            {pages.map((page) => (
              <Route path={page.route} element={page.element} key={page.key}>
                {page.children &&
                  page.children.map((child) => (
                    <Route path={child.route} element={child.element} key={child.key} />
                  ))}
              </Route>
            ))}
          </Routes>
        </Container>
      </ThemeProvider>
    </Router>
  );
}

export default App;
