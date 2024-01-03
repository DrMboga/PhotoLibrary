import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { backendAPI } from '../../api/BackendApi';
import { ensureError } from '../../helpers/error-helper';
import { RootState } from '../../store';

export interface ImporterState {
  loading: boolean;
  isImporterInProgress: boolean;
  error?: string;
}

export const initialImporterState: ImporterState = {
  loading: false,
  isImporterInProgress: false,
};

export const getImporterStatus = createAsyncThunk(
  'getImporterStatus',
  async (authToken?: string) => {
    return await backendAPI.isImporterInProgress(authToken);
  },
);

export const importerSlice = createSlice({
  name: 'importerSlice',
  initialState: initialImporterState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getImporterStatus.pending, (state) => {
        state.loading = true;
        state.error = undefined;
        state.isImporterInProgress = false;
      })
      .addCase(getImporterStatus.fulfilled, (state, action) => {
        state.loading = false;
        state.error = undefined;
        state.isImporterInProgress = action.payload;
      })
      .addCase(getImporterStatus.rejected, (state, action) => {
        state.loading = false;
        state.isImporterInProgress = false;
        if (action.payload) {
          const error = ensureError(action.payload);
          state.error = error.message;
        } else {
          state.error = 'Unable to reach backend';
        }
      });
  },
});

export default importerSlice.reducer;

export const selectImporterLoading = (state: RootState) => state.importer.loading;
export const selectIsImporterInProgress = (state: RootState) => state.importer.isImporterInProgress;
export const selectImporterError = (state: RootState) => state.importer.error;
