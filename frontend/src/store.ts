import { configureStore } from '@reduxjs/toolkit';
import appReducer from './appSlice';
import authReducer from './keycloak-auth/authSlice';
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
};

const persistedAppReducer = persistReducer(persistAppConfig, appReducer);
const persistedAuthReducer = persistReducer(persistAuthConfig, authReducer);

export const store = configureStore({
  reducer: {
    app: persistedAppReducer,
    auth: persistedAuthReducer,
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
