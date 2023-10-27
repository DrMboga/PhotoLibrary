import { createSlice } from '@reduxjs/toolkit';
import { RootState } from './store';

export interface AppState {
  theme: 'light' | 'dark';
}

export const initialAppState: AppState = {
  theme: 'light',
};

export const appSlice = createSlice({
  name: 'app',
  initialState: initialAppState,
  reducers: {
    toggleTheme: (state) => {
      state.theme = state.theme === 'light' ? 'dark' : 'light';
    },
  },
});

export const { toggleTheme } = appSlice.actions;

export const selectTheme = (state: RootState) => state.app.theme;

export default appSlice.reducer;
