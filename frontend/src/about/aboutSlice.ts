import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { backendAPI } from '../api/BackendApi';
import { ensureError } from '../helpers/error-helper';
import { RootState } from '../store';

export interface AboutState {
  loading: boolean;
  frontendVersion?: string;
  backendInfo?: string;
  backendAccessError?: string;
}

export const initialAboutState: AboutState = {
  loading: false,
  frontendVersion: process.env.REACT_APP_VERSION,
};

export const loadBackendAbout = createAsyncThunk(
  'loadBackendAbout',
  async (_, { rejectWithValue }) => {
    return await backendAPI.aboutBackend();
  },
);

export const aboutSlice = createSlice({
  name: 'aboutSlice',
  initialState: initialAboutState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(loadBackendAbout.pending, (state) => {
        state.loading = true;
        state.backendAccessError = undefined;
        state.backendInfo = undefined;
      })
      .addCase(loadBackendAbout.fulfilled, (state, action) => {
        state.loading = false;
        state.backendAccessError = undefined;
        state.backendInfo = action.payload;
      })
      .addCase(loadBackendAbout.rejected, (state, action) => {
        state.loading = false;
        if (action.payload) {
          const error = ensureError(action.payload);
          state.backendAccessError = error.message;
        } else {
          state.backendAccessError = 'Unable to reach backend';
        }
        state.backendInfo = undefined;
      });
  },
});

export default aboutSlice.reducer;

export const selectBackendAboutLoading = (state: RootState) => state.about.loading;
export const selectFrontendVersion = (state: RootState) => state.about.frontendVersion;
export const selectBackendAboutInfo = (state: RootState) => state.about.backendInfo;
export const selectBackendAccessError = (state: RootState) => state.about.backendAccessError;
