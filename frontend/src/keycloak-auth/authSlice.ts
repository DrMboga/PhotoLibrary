import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { ensureError } from '../helpers/error-helper';
import { RootState } from '../store';
import Keycloak from 'keycloak-js';
import { currentDateLinuxTime } from '../helpers/date-helper';

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

export const initKeycloak = createAsyncThunk('init', async (keycloak: Keycloak) => {
  try {
    await keycloak.init({
      onLoad: 'login-required',
      enableLogging: true,
      redirectUri: process.env.REACT_APP_REDIRECT_URL,
    });

    let userName: string | undefined;
    let token: string | undefined;
    let tokenExpiration: number | undefined;
    let refreshToken: string | undefined;
    if (keycloak.tokenParsed) {
      userName = keycloak.tokenParsed['name'];
      token = keycloak.token;
      tokenExpiration = keycloak.tokenParsed.exp;
      refreshToken = keycloak.refreshToken;
    }
    return {
      authenticated: keycloak.authenticated,
      userName,
      token,
      tokenExpiration,
      refreshToken,
    };
  } catch (err) {
    throw ensureError(err);
  }
});

export const logoutKeycloak = createAsyncThunk('logout', async (keycloak: Keycloak) => {
  try {
    await keycloak.logout({ redirectUri: process.env.REACT_APP_REDIRECT_URL });
  } catch (err) {
    throw ensureError(err);
  }
});

/*
"keycloak-js" has method "prolong". But to use it, we need to keep the instance of Keycloak class whole application lifetime.
So, here is easier to call Keycloak API manually rather than using a "keycloak-js" method
 */
export const prolongAuthToken = createAsyncThunk(
  'prolongAuthToken',
  async (refreshToken?: string) => {
    const result: {
      token?: string;
      tokenExpiration?: number;
      refreshToken?: string;
    } = {};
    if (!refreshToken) {
      return result;
    }
    try {
      const baseUrl = process.env.REACT_APP_KEYCLOAK_URL ?? '';
      const realm = process.env.REACT_APP_KEYCLOAK_REALM ?? '';
      const clientId = process.env.REACT_APP_KEYCLOAK_CLIENT_ID ?? '';
      const prolongUrl = `${baseUrl}/realms/${realm}/protocol/openid-connect/token`;
      const formBody = `grant_type=refresh_token&client_id=${clientId}&refresh_token=${refreshToken}`;

      const prolongResponse = await fetch(prolongUrl, {
        method: 'POST',
        headers: { 'Content-type': 'application/x-www-form-urlencoded' },
        body: formBody,
      });

      if (prolongResponse.ok) {
        const serializedResponse = await prolongResponse.json();
        if (serializedResponse) {
          result.token = serializedResponse['access_token'];
          result.refreshToken = serializedResponse['refresh_token'];
          const expiresIn = serializedResponse['expires_in'];
          if (!isNaN(expiresIn)) {
            const currentTime = currentDateLinuxTime();
            result.tokenExpiration = currentTime + +expiresIn * 1000;
          }
          console.log('serializedResponse', result);
        }
      } else if (prolongResponse.status === 400) {
        const serializedResponse = await prolongResponse.json();
        if (serializedResponse) {
          const error = serializedResponse['error_description'];
          throw new Error(error);
        }
      } else {
        throw new Error(`Prolong token request returned a ${prolongResponse.status} status`);
      }

      return result;
    } catch (err) {
      console.warn('prolongAuthToken', err);
      throw ensureError(err);
    }
  },
);

export const authSlice = createSlice({
  name: 'authentication',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(initKeycloak.pending, (state) => {
        state.authenticated = false;
        state.token = undefined;
        state.refreshToken = undefined;
        state.userName = undefined;
        state.tokenExpiration = undefined;
        state.error = undefined;
      })
      .addCase(initKeycloak.fulfilled, (state, action) => {
        if (!action.payload) {
          return;
        }
        state.authenticated = action.payload.authenticated ?? false;
        state.token = action.payload.token;
        state.userName = action.payload.userName;
        state.tokenExpiration = action.payload.tokenExpiration;
        state.refreshToken = action.payload.refreshToken;
        state.error = undefined;
      })
      .addCase(initKeycloak.rejected, (state, action) => {
        if (action?.error?.message) {
          state.error = action.error.message;
        } else {
          const error = ensureError(action.error);
          state.error = error.message;
        }
      })
      .addCase(logoutKeycloak.fulfilled, (state) => {
        state.authenticated = false;
        state.token = undefined;
        state.refreshToken = undefined;
        state.userName = undefined;
        state.tokenExpiration = undefined;
        state.error = undefined;
      })
      .addCase(logoutKeycloak.rejected, (state, action) => {
        if (action?.error?.message) {
          state.error = action.error.message;
        } else {
          const error = ensureError(action.error);
          state.error = error.message;
        }
        state.authenticated = false;
        state.token = undefined;
        state.refreshToken = undefined;
        state.userName = undefined;
        state.tokenExpiration = undefined;
      })
      .addCase(prolongAuthToken.pending, (state) => {
        state.error = undefined;
      })
      .addCase(prolongAuthToken.fulfilled, (state, action) => {
        if (!action.payload) {
          return;
        }
        state.authenticated = true;
        state.token = action.payload.token;
        state.tokenExpiration = action.payload.tokenExpiration;
        state.refreshToken = action.payload.refreshToken;
        console.log('prolongAuthToken.fulfilled - new token');
      })
      .addCase(prolongAuthToken.rejected, (state, action) => {
        if (action?.error?.message) {
          state.error = action.error.message;
        } else {
          const error = ensureError(action.error);
          state.error = error.message;
        }
        state.authenticated = false;
        state.token = undefined;
        state.refreshToken = undefined;
        state.userName = undefined;
        state.tokenExpiration = undefined;
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
