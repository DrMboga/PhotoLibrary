import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../store';

export interface PhotosStore {
  dateOfFirstPhoto?: number;
  dateOfLastPhoto?: number;
  loadingTop: boolean;
  loadingBottom: boolean;
}

export const initialState: PhotosStore = {
  loadingTop: false,
  loadingBottom: false,
};

const photoSlice = createSlice({
  name: 'photos',
  initialState,
  reducers: {
    scrollToTop: (state) => {
      state.loadingTop = true;
    },
    scrollToBottom: (state) => {
      state.loadingBottom = true;
    },
    changeDateOfFirstPhoto: (state, action: PayloadAction<number>) => {
      state.loadingTop = false;
      state.loadingBottom = false;
      state.dateOfFirstPhoto = action.payload;
    },
    changeDateOfLastPhoto: (state, action: PayloadAction<number>) => {
      state.loadingTop = false;
      state.loadingBottom = false;
      state.dateOfLastPhoto = action.payload;
    },
  },
});

export const { scrollToTop, scrollToBottom, changeDateOfFirstPhoto, changeDateOfLastPhoto } =
  photoSlice.actions;

export default photoSlice.reducer;

export const selectPhotosLoadingTop = (state: RootState) => state.photos.loadingTop;
export const selectPhotosLoadingBottom = (state: RootState) => state.photos.loadingBottom;
export const selectDateOfFirstPhoto = (state: RootState) => state.photos.dateOfFirstPhoto;
export const selectDateOfLastPhoto = (state: RootState) => state.photos.dateOfLastPhoto;
