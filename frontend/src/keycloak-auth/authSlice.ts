import keycloak from './keycloak';
import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { ensureError } from '../helpers/error-helper';
import { RootState } from '../store';
import Keycloak from 'keycloak-js';
import { currentDateLinuxTime } from '../helpers/date-helper';

export interface AuthState {
  authenticated: boolean;
  userName?: string;
  token?: string;
  tokenExpiration?: number;
  error?: string;
}

export const initialState: AuthState = {
  authenticated: false,
};

export const initKeycloak = createAsyncThunk('init', async (_, { rejectWithValue }) => {
  try {
    const authenticated = await keycloak.init({
      onLoad: 'login-required',
      enableLogging: true,
      redirectUri: process.env.REACT_APP_REDIRECT_URL,
    });

    const currentDateNumber = currentDateLinuxTime();

    let userName: string | undefined;
    let token: string | undefined;
    let tokenExpiration: number | undefined;
    if (keycloak.tokenParsed) {
      userName = keycloak.tokenParsed['name'];
      token = keycloak.token;
      tokenExpiration = keycloak.tokenParsed.exp;
      if (tokenExpiration) {
        tokenExpiration = currentDateNumber + tokenExpiration / 1000;
      }
    }
    return { authenticated: keycloak.authenticated, userName, token, tokenExpiration };
  } catch (err) {
    rejectWithValue(err);
  }
});

export const logoutKeycloak = createAsyncThunk('logout', async (_, { rejectWithValue }) => {
  try {
    await keycloak.logout({ redirectUri: process.env.REACT_APP_REDIRECT_URL });
  } catch (err) {
    rejectWithValue(err);
  }
});

export const authSlice = createSlice({
  name: 'authentication',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(initKeycloak.fulfilled, (state, action) => {
        if (!action.payload) {
          return;
        }
        state.authenticated = action.payload.authenticated ?? false;
        state.token = action.payload.token;
        state.userName = action.payload.userName;
        state.tokenExpiration = action.payload.tokenExpiration;
        state.error = undefined;
      })
      .addCase(initKeycloak.rejected, (state, action) => {
        const error = ensureError(action.payload);
        state.error = error.message;
      })
      .addCase(logoutKeycloak.fulfilled, (state) => {
        state.authenticated = false;
        state.token = undefined;
        state.userName = undefined;
        state.tokenExpiration = undefined;
        state.error = undefined;
      })
      .addCase(logoutKeycloak.rejected, (state, action) => {
        const error = ensureError(action.payload);
        state.error = error.message;
        state.authenticated = false;
        state.token = undefined;
        state.userName = undefined;
        state.tokenExpiration = undefined;
      });
  },
});

export default authSlice.reducer;

export const selectAuthenticated = (state: RootState) => state.auth.authenticated;
export const selectUserName = (state: RootState) => state.auth.userName;
export const selectToken = (state: RootState) => state.auth.token;
export const selectTokenExpiration = (state: RootState) => state.auth.tokenExpiration;
export const selectAuthError = (state: RootState) => state.auth.error;
