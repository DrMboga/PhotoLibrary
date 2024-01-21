import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from '../store';
import { authenticationApi } from '../api/AuthenticationApi';
import { ensureError } from '../helpers/error-helper';
import { currentDateLinuxTime } from '../helpers/date-helper';

export interface AuthState {
  authenticated: boolean;
  userName?: string;
  token?: string;
  refreshToken?: string;
  tokenExpiration?: number;
  error?: string;
  authStatus?: string;
}

export const initialState: AuthState = {
  authenticated: false,
};

export const register = createAsyncThunk(
  'register',
  async (payload: { email?: string; password?: string }) => {
    return await authenticationApi.register(payload.email, payload.password);
  },
);
export const login = createAsyncThunk(
  'login',
  async (payload: { email?: string; password?: string }) => {
    return await authenticationApi.login(payload.email, payload.password);
  },
);

export const authSlice = createSlice({
  name: 'authentication',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(register.pending, (state) => {
        state.authenticated = false;
        state.userName = undefined;
        state.token = undefined;
        state.tokenExpiration = undefined;
        state.refreshToken = undefined;
        state.error = undefined;
        state.authStatus = undefined;
      })
      .addCase(register.rejected, (state, action) => {
        state.error = ensureError(action.error).message;
      })
      .addCase(register.fulfilled, (state) => {
        state.authStatus = 'Successfully registered. Now please login.';
      })
      .addCase(login.pending, (state) => {
        state.authenticated = false;
        state.userName = undefined;
        state.token = undefined;
        state.tokenExpiration = undefined;
        state.refreshToken = undefined;
        state.error = undefined;
        state.authStatus = undefined;
      })
      .addCase(login.rejected, (state, action) => {
        state.error = ensureError(action.error).message;
      })
      .addCase(login.fulfilled, (state, action) => {
        const currentDate = currentDateLinuxTime();
        const tokenExpiration = currentDate + action.payload.expiresIn; // expiresIn in seconds
        console.log('login success', tokenExpiration);

        state.authenticated = true;
        state.userName = action.payload.accessToken;
        state.token = action.payload.accessToken;
        state.tokenExpiration = tokenExpiration;
        state.refreshToken = action.payload.refreshToken;
        state.error = undefined;
        state.authStatus = 'login success';
      });
  },
});

export default authSlice.reducer;

export const selectAuthenticated = (state: RootState) => state.auth.authenticated;
export const selectUserName = (state: RootState) => state.auth.userName;
export const selectToken = (state: RootState) => state.auth.token;
export const selectRefreshToken = (state: RootState) => state.auth.refreshToken;
export const selectTokenExpiration = (state: RootState) => state.auth.tokenExpiration;
export const selectAuthError = (state: RootState) => state.auth.error;
export const selectAuthenticationStatus = (state: RootState) => state.auth.authStatus;
