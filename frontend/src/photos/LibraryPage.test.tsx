import React from 'react';
import { render } from '@testing-library/react';
import LibraryPage from './LibraryPage';
import { store } from '../store';
import { Provider } from 'react-redux';

test('should render LibraryPage', () => {
  render(
    <Provider store={store}>
      <LibraryPage />
    </Provider>,
  );
});
