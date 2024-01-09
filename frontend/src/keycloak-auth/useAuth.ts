import { useAppDispatch, useAppSelector } from '../storeHooks';
import { initKeycloak, selectAuthenticated, selectTokenExpiration } from './authSlice';
import { useEffect, useRef, useState } from 'react';
import { currentDateLinuxTime } from '../helpers/date-helper';
import keycloak from './keycloak';

/*
This "useAuth" hook is used in the App.tsx.
So, the first hook (with no dependency) is called every time when page reload or navigation takes place.
Here the "keycloak-js" library is used. (https://www.keycloak.org/docs/latest/securing_apps/#_javascript_adapter)
Also authSlice state is used to store the AuthToken and TokenExpiration parameters (with persist in the local storage).

The first scenario, when user is not authenticated and no data stored in the state.
1. First hook will call `initKeycloak` thunk in the `authSlice. Which will redirect user to the login page and then redirect back.
  In the end, a new Token and new TokenExpiration date will be stored in the state.
2. As soon is `tokenExpiration` parameter is changed in the state, the second hook will be fired.
  This hook will start the timeout for period 2 seconds before token expiration. (Usually a token lifetime is 5 minutes)
  The timeout callback will set the local value `needToProlongToken` to true
3. As soon as `needToProlongToken` is set to true, the third hook will start a new `initKeycloak` thunk
  and the new token and expiration time will be set in the store. And step 2 will be repeated.

Second scenario, when navigation or page reload happens and user is authenticated.
Then there is a valid token is in the store. And second hook will start the timer and steps 2 and 3 will be repeated every 5 minutes as normal.

With this strategy, the actual token will be always in the store. And every page in the application can use this:
const authToken = useAppSelector(selectToken);
And set `useEffect` dependent on the authToken.

Only one disadvantage of this strategy is that every 5 minutes when token need to be updated, the app will be redirected to the start page
because of `redirectUri` in the keycloak init method.
 */

export function useAuth() {
  const authenticated = useAppSelector(selectAuthenticated);
  const tokenExpiration = useAppSelector(selectTokenExpiration);
  const dispatch = useAppDispatch();

  const initializedOnce = useRef(false);

  const [needToProlongToken, setNeedToProlongToken] = useState(false);

  useEffect(() => {
    if (initializedOnce.current) {
      return;
    }
    const currentDateNumber = currentDateLinuxTime();

    const needAuth = !authenticated || !tokenExpiration || currentDateNumber > tokenExpiration;
    if (needAuth) {
      const keycloakInstance = keycloak();
      dispatch(initKeycloak(keycloakInstance));
    }
    initializedOnce.current = true;
  }, []);

  useEffect(() => {
    let timeout: NodeJS.Timeout | undefined;
    if (tokenExpiration) {
      const now = currentDateLinuxTime();
      // Interval is for update token. Set it 2 seconds earlier then token expires
      const interval = (tokenExpiration - now - 2) * 1000;
      if (interval <= 0) {
        setNeedToProlongToken(true);
      } else {
        setNeedToProlongToken(false);
        timeout = setTimeout(() => {
          setNeedToProlongToken(true);
        }, interval);
      }
    }

    return () => {
      if (timeout) {
        clearTimeout(timeout);
      }
    };
  }, [tokenExpiration]);

  useEffect(() => {
    if (needToProlongToken) {
      console.log('Prolonging auth token');
      const keycloakInstance = keycloak();
      dispatch(initKeycloak(keycloakInstance));
    }
  }, [needToProlongToken]);
}
