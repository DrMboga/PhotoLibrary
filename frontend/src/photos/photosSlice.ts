import { MediaInfo } from '../model/mediaInfo';
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { MEDIA_INFOS_MOCK } from '../model/mock/mediaInfosMock';
import { RootState } from '../store';

export interface PhotosStore {
  photos: MediaInfo[];
}

export const initialState: PhotosStore = {
  photos: [],
};

// TODO: delete this after using real API
// eslint-disable-next-line
function delay(ms: number): Promise<any> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export const getPhotos = createAsyncThunk('getPhotos', async (_) => {
  await delay(1000);
  const sortedMockPhotos = MEDIA_INFOS_MOCK.sort((a, b) => b.dateTimeOriginal - a.dateTimeOriginal);
  return sortedMockPhotos.slice(0, 20);
});

const photoSlice = createSlice({
  name: 'photos',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder.addCase(getPhotos.pending, (state) => {
      state.photos = [];
    });
    builder.addCase(getPhotos.fulfilled, (state, action) => {
      state.photos = action.payload;
    });
  },
});

export default photoSlice.reducer;

export const selectPhotos = (state: RootState) => state.photos.photos;
