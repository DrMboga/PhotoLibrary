import { MediaInfo } from '../model/mediaInfo';
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { MEDIA_INFOS_MOCK } from '../model/mock/mediaInfosMock';
import { RootState } from '../store';
import { ensureError } from '../helpers/error-helper';

const ChunkSize = 30;

export interface PhotosStore {
  dateOfFirstPhoto?: number;
  dateOfLastPhoto?: number;
  loadingTop: boolean;
  loadingBottom: boolean;
  photos: MediaInfo[];
}

export const initialState: PhotosStore = {
  loadingTop: false,
  loadingBottom: false,
  photos: [],
};

// TODO: delete this after using real API
// eslint-disable-next-line
function delay(ms: number): Promise<any> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}
const sortByDateDesc = (a: MediaInfo, b: MediaInfo) => b.dateTimeOriginal - a.dateTimeOriginal;

export const getPhotos = createAsyncThunk('getPhotos', async (dateFrom?: number) => {
  await delay(1000);
  const sortedMockPhotos = MEDIA_INFOS_MOCK.sort(sortByDateDesc);
  if (dateFrom) {
    return sortedMockPhotos.filter((m) => m.dateTimeOriginal <= dateFrom).slice(0, ChunkSize);
  } else {
    return sortedMockPhotos.slice(0, ChunkSize);
  }
});

export const getPreviousPhotosChunk = createAsyncThunk(
  'getPreviousPhotosChunk',
  async (dateTo: number) => {
    try {
      await delay(1000);
      const sortedMockPhotos = MEDIA_INFOS_MOCK.sort(sortByDateDesc);
      // TODO: Do the same compare on backend to not send the same chunk of data
      if (dateTo === sortedMockPhotos[0].dateTimeOriginal) {
        return [];
      }
      const borderPhoto = sortedMockPhotos.find((m) => m.dateTimeOriginal === dateTo);
      if (borderPhoto) {
        const borderIndex = sortedMockPhotos.indexOf(borderPhoto);
        const start = borderIndex - ChunkSize > 0 ? borderIndex - ChunkSize : 0;
        return sortedMockPhotos.slice(start, start + ChunkSize);
      } else {
        return [];
      }
    } catch (err) {
      throw ensureError(err);
    }
  },
);

export const getNextPhotosChunk = createAsyncThunk(
  'getNextPhotosChunk',
  async (dateFrom: number) => {
    try {
      await delay(1000);
      const sortedMockPhotos = MEDIA_INFOS_MOCK.sort(sortByDateDesc);
      return sortedMockPhotos.filter((m) => m.dateTimeOriginal < dateFrom).slice(0, ChunkSize);
    } catch (err) {
      throw ensureError(err);
    }
  },
);

const photoSlice = createSlice({
  name: 'photos',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getPhotos.pending, (state) => {
        state.photos = [];
        state.loadingTop = true;
      })
      .addCase(getPhotos.fulfilled, (state, action) => {
        state.loadingTop = false;
        state.loadingBottom = false;
        state.photos = action.payload;
        if (action.payload.length > 0) {
          state.dateOfFirstPhoto = action.payload[0].dateTimeOriginal;
          state.dateOfLastPhoto = action.payload[action.payload.length - 1].dateTimeOriginal;
        }
      })
      .addCase(getNextPhotosChunk.pending, (state) => {
        state.loadingTop = false;
        state.loadingBottom = true;
      })
      .addCase(getNextPhotosChunk.fulfilled, (state, action) => {
        state.loadingTop = false;
        state.loadingBottom = false;
        if (action.payload.length === 0) {
          return;
        }
        let photosArray = [...state.photos, ...action.payload];
        if (state.photos.length > ChunkSize) {
          // Remove first ChunkSize elements
          photosArray = photosArray.slice(ChunkSize);
        }
        if (photosArray.length > 0) {
          state.dateOfFirstPhoto = photosArray[0].dateTimeOriginal;
          state.dateOfLastPhoto = photosArray[photosArray.length - 1].dateTimeOriginal;
        }
        state.photos = photosArray;
      })
      .addCase(getNextPhotosChunk.rejected, (state, action) => {
        console.log('getNextPhotosChunk rejected', action);
      })
      .addCase(getPreviousPhotosChunk.pending, (state) => {
        state.loadingTop = true;
        state.loadingBottom = false;
      })
      .addCase(getPreviousPhotosChunk.fulfilled, (state, action) => {
        state.loadingTop = false;
        state.loadingBottom = false;
        if (action.payload.length === 0) {
          return;
        }
        let photosArray = [...action.payload, ...state.photos];
        if (state.photos.length > 2 * ChunkSize) {
          // Remove last ChunkSize elements
          photosArray = photosArray.slice(0, state.photos.length - ChunkSize);
        }
        if (photosArray.length > 0) {
          state.dateOfFirstPhoto = photosArray[0].dateTimeOriginal;
          state.dateOfLastPhoto = photosArray[photosArray.length - 1].dateTimeOriginal;
        }
        state.photos = photosArray;
      })
      .addCase(getPreviousPhotosChunk.rejected, (state, action) => {
        console.log('getPreviousPhotosChunk rejected', action);
      });
  },
});

export default photoSlice.reducer;

export const selectPhotosLoadingTop = (state: RootState) => state.photos.loadingTop;
export const selectPhotosLoadingBottom = (state: RootState) => state.photos.loadingBottom;
export const selectDateOfFirstPhoto = (state: RootState) => state.photos.dateOfFirstPhoto;
export const selectDateOfLastPhoto = (state: RootState) => state.photos.dateOfLastPhoto;
export const selectPhotos = (state: RootState) => state.photos.photos;
