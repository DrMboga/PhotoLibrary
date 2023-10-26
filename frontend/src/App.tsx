import React from 'react';
import './App.css';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import HomePage from './photos/HomePage';
import SettingsPage from './settings/SettingsPage';
import { Container } from '@mui/material';
import CollectionsIcon from '@mui/icons-material/Collections';
import SettingsIcon from '@mui/icons-material/Settings';
import { PageRouteInfo } from './nav-bar/page-route-info';
import { NavBar } from './nav-bar/NavBar';

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
  return (
    <Router>
      <NavBar routesInfo={pages} />
      <Container maxWidth={false}>
        <Routes>
          {pages.map((page) => (
            <Route path={page.route} element={page.element} />
          ))}
        </Routes>
      </Container>
    </Router>
  );
}

export default App;
