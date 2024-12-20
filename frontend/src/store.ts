import { configureStore } from '@reduxjs/toolkit';
import appReducer from './appSlice';
import authReducer from './authentication/authSlice';
import photosReducer from './photos/photosSlice';
import {
  persistStore,
  persistReducer,
  FLUSH,
  REHYDRATE,
  PAUSE,
  PERSIST,
  PURGE,
  REGISTER,
} from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import aboutReducer from './about/aboutSlice';
import importerSlice from './settings/Importer/importerSlice';

// Persist appReducer to the local storage via redux-persist
const persistAppConfig = {
  key: 'photo-lib-app',
  version: 1,
  storage,
};

const persistAuthConfig = {
  key: 'photo-lib-auth',
  version: 1,
  storage,
  blacklist: ['error', 'authStatus'],
};

const persistPhotosConfig = {
  key: 'photo-lib-photos',
  version: 1,
  storage,
  blacklist: ['photos', 'loadingTop', 'loadingBottom', 'error'],
};

const persistedAppReducer = persistReducer(persistAppConfig, appReducer);
const persistedAuthReducer = persistReducer(persistAuthConfig, authReducer);
const persistedPhotoReducer = persistReducer(persistPhotosConfig, photosReducer);

export const store = configureStore({
  reducer: {
    app: persistedAppReducer,
    auth: persistedAuthReducer,
    photos: persistedPhotoReducer,
    about: aboutReducer,
    importer: importerSlice,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
      },
    }),
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export const persistor = persistStore(store);
