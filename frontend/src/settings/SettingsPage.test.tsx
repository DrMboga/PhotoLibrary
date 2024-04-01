import React from 'react';
import { render } from '@testing-library/react';
import SettingsPage from './SettingsPage';
import { Router } from 'react-router-dom';

test('should render SettingsPage', () => {
  render(
    <Router location="importer">
      <SettingsPage />
    </Router>,
  );
});
