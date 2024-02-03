import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../store';

export interface PhotosStore {
  dateOfFirstPhoto?: number;
  dateOfLastPhoto?: number;
  loadingTop: boolean;
  loadingBottom: boolean;
  error?: string;
}

export const initialState: PhotosStore = {
  loadingTop: false,
  loadingBottom: false,
};

const photoSlice = createSlice({
  name: 'photos',
  initialState,
  reducers: {
    setLoadingTop: (state) => {
      state.loadingTop = true;
    },
    setLoadingBottom: (state) => {
      state.loadingBottom = true;
    },
    changeDateOfFirstPhoto: (state, action: PayloadAction<number>) => {
      state.loadingTop = false;
      state.loadingBottom = false;
      state.dateOfFirstPhoto = action.payload;
      state.error = undefined;
    },
    changeDateOfLastPhoto: (state, action: PayloadAction<number>) => {
      state.loadingTop = false;
      state.loadingBottom = false;
      state.dateOfLastPhoto = action.payload;
      state.error = undefined;
    },
    errorOccurred: (state, action: PayloadAction<string>) => {
      state.error = action.payload;
    },
  },
});

export const {
  setLoadingTop,
  setLoadingBottom,
  changeDateOfFirstPhoto,
  changeDateOfLastPhoto,
  errorOccurred,
} = photoSlice.actions;

export default photoSlice.reducer;

export const selectPhotosLoadingTop = (state: RootState) => state.photos.loadingTop;
export const selectPhotosLoadingBottom = (state: RootState) => state.photos.loadingBottom;
export const selectDateOfFirstPhoto = (state: RootState) => state.photos.dateOfFirstPhoto;
export const selectDateOfLastPhoto = (state: RootState) => state.photos.dateOfLastPhoto;
export const selectPhotosLibraryError = (state: RootState) => state.photos.error;
