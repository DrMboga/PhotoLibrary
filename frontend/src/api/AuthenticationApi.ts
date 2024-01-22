import { backendUrl, checkStatus, parseJSON } from './ApiHelper';
import { AuthenticationResponse } from '../model/authentication-response';

const authenticationApi = {
  async register(email?: string, password?: string): Promise<void> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    const body = `{
  "email": "${email ?? ''}",
  "password": "${password ?? ''}"
}`;
    const response = await fetch(`${backendUrl}/register`, {
      method: 'post',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body,
    });
    if (response.status === 400) {
      const errorResult = await response.json();
      const errorText = JSON.stringify(errorResult.errors);
      throw new Error(errorText);
    } else {
      checkStatus(response);
    }
  },

  async login(email?: string, password?: string): Promise<AuthenticationResponse> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    const body = `{
  "email": "${email}",
  "password": "${password}"
}`;
    const response = await fetch(`${backendUrl}/login?useCookies=false&useSessionCookies=false`, {
      method: 'post',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body,
    });
    if (response.status === 400) {
      const errorResult = await response.json();
      const errorText = JSON.stringify(errorResult.errors);
      throw new Error(errorText);
    } else {
      const responseWithStatus = checkStatus(response);
      return parseJSON(responseWithStatus);
    }
  },

  async refresh(refreshToken?: string): Promise<AuthenticationResponse> {
    if (!backendUrl) {
      throw new Error('Please specify Backend URL in environment settings');
    }
    const body = `{
  "refreshToken": "${refreshToken}"
}`;
    const response = await fetch(`${backendUrl}/refresh`, {
      method: 'post',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body,
    });
    const responseWithStatus = checkStatus(response);
    return parseJSON(responseWithStatus);
  },

  // TODO: logout
};

export { authenticationApi };
