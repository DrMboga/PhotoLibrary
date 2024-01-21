import { createSlice } from '@reduxjs/toolkit';
import { RootState } from '../store';

export interface AuthState {
  authenticated: boolean;
  userName?: string;
  token?: string;
  refreshToken?: string;
  tokenExpiration?: number;
  error?: string;
}

export const initialState: AuthState = {
  authenticated: false,
};

export const authSlice = createSlice({
  name: 'authentication',
  initialState,
  reducers: {},
});

export default authSlice.reducer;

export const selectAuthenticated = (state: RootState) => state.auth.authenticated;
export const selectUserName = (state: RootState) => state.auth.userName;
export const selectToken = (state: RootState) => state.auth.token;
export const selectRefreshToken = (state: RootState) => state.auth.refreshToken;
export const selectTokenExpiration = (state: RootState) => state.auth.tokenExpiration;
export const selectAuthError = (state: RootState) => state.auth.error;
