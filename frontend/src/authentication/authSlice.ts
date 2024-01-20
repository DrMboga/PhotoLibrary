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
  authenticated: true, // TODO: Change to false when connect the Auth provider
  userName: 'FakeUser', // TODO: This should be set up after Login thunk when connect AuthProvider
  token: 'FakeToken', // TODO: This should be set up after Login thunk when connect AuthProvider
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
