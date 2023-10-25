import React from 'react';
import './App.css';
import { BrowserRouter as Router, Routes, Route, NavLink } from 'react-router-dom';
import HomePage from './photos/HomePage';
import SettingsPage from './settings/SettingsPage';

function App() {
  return (
    <Router>
      <div>
        <NavLink to="/">Home</NavLink>
        <NavLink to="/settings">Settings</NavLink>
      </div>

      {/*class Container*/}
      <div>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/settings" element={<SettingsPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
