import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
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

// triggerMediaImport thunk
export const triggerMediaImport = createAsyncThunk(
  'triggerMediaImport',
  async (authToken?: string) => {
    await backendAPI.triggerMediaImport(authToken);
  },
);

export const importerSlice = createSlice({
  name: 'importerSlice',
  initialState: initialImporterState,
  reducers: {
    importerSignalRErrorOccurred: (state, action: PayloadAction<string>) => {
      state.error = action.payload;
    },
    importerNewStepAdded: (state, action: PayloadAction<ImportStepReport>) => {
      state.importSteps = [action.payload, ...(state.importSteps ?? [])];
    },
    importerStarted: (state) => {
      state.isImporterInProgress = true;
    },
    importerFinished: (state) => {
      state.isImporterInProgress = false;
    },
  },
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
      })
      .addCase(triggerMediaImport.rejected, (state, action) => {
        if (action.payload) {
          const error = ensureError(action.payload);
          state.error = error.message;
        } else {
          state.error = 'Unable to reach backend';
        }
      });
  },
});

export const {
  importerSignalRErrorOccurred,
  importerNewStepAdded,
  importerStarted,
  importerFinished,
} = importerSlice.actions;

export default importerSlice.reducer;

export const selectImporterLoading = (state: RootState) => state.importer.loading;
export const selectIsImporterInProgress = (state: RootState) => state.importer.isImporterInProgress;
export const selectImporterError = (state: RootState) => state.importer.error;
export const selectImporterSteps = (state: RootState) => state.importer.importSteps;
