import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { backendAPI } from '../../api/BackendApi';
import { ensureError } from '../../helpers/error-helper';
import { RootState } from '../../store';
import { ImportStepReport } from '../../model/media-info';

export interface ImporterState {
  loading: boolean;
  isImporterInProgress: boolean;
  error?: string;
  importSteps?: ImportStepReport[];
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

export const getImporterLogs = createAsyncThunk('getImporterLogs', async (authToken?: string) => {
  return await backendAPI.getImporterLogs(authToken);
});

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
        state.importSteps = undefined;
      })
      .addCase(getImporterStatus.fulfilled, (state, action) => {
        state.loading = false;
        state.error = undefined;
        state.isImporterInProgress = action.payload;
        state.importSteps = undefined;
      })
      .addCase(getImporterStatus.rejected, (state, action) => {
        state.loading = false;
        state.isImporterInProgress = false;
        state.importSteps = undefined;
        if (action.payload) {
          const error = ensureError(action.payload);
          state.error = error.message;
        } else {
          state.error = 'Unable to reach backend';
        }
      })
      .addCase(getImporterLogs.pending, (state) => {
        state.loading = true;
        state.error = undefined;
        state.importSteps = undefined;
      })
      .addCase(getImporterLogs.fulfilled, (state, action) => {
        state.loading = false;
        state.error = undefined;
        state.importSteps = action.payload;
      })
      .addCase(getImporterLogs.rejected, (state, action) => {
        state.loading = false;
        state.importSteps = undefined;
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
export const selectImporterSteps = (state: RootState) => state.importer.importSteps;
