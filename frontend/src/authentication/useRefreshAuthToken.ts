import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { refresh, selectRefreshToken, selectTokenExpiration } from './authSlice';
import { useEffect, useState } from 'react';
import { currentDateLinuxTime } from '../helpers/date-helper';

/*
This hook is used in the App.tsx.
So, the first hook (with no dependency) is called every time when page reload or navigation takes place.
Here authSlice state is used to store the AuthToken and TokenExpiration parameters (with persist in the local storage).

The use scenario
1. As soon is `tokenExpiration` parameter is changed in the state, the appropriate hook is fired.
  This hook will start the timeout for period 2 seconds before token expiration. (Usually a token lifetime is 1 hour right after the login. It is
    stored in the local storage and each time page reloads, the hook calculating the time left until the expiration and stars timeout for this period)
  The timeout callback will set the local value `needToProlongToken` to true
2. As soon as `needToProlongToken` is set to true, the second hook will start a `prolongAuthToken` thunk
  and the new token and expiration time will be set in the store. And step 1 will be repeated.

With this strategy, the actual token will be always in the store. And every page in the application can use this:
const authToken = useAppSelector(selectToken);
And set `useEffect` dependent on the authToken.
 */
export function useRefreshAuthToken() {
  const tokenExpiration = useAppSelector(selectTokenExpiration);
  const refreshToken = useAppSelector(selectRefreshToken);
  const dispatch = useAppDispatch();

  const [needToProlongToken, setNeedToProlongToken] = useState(false);

  useEffect(() => {
    let timeout: NodeJS.Timeout | undefined;
    if (tokenExpiration) {
      const now = currentDateLinuxTime();
      // Interval is for update token. Set it 2 seconds earlier then token expires
      const interval = (tokenExpiration - now - 2) * 1000;
      console.log('Token prolong in', interval);
      if (interval > 0) {
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
    if (needToProlongToken && refreshToken) {
      setNeedToProlongToken(false);
      console.log('Prolonging auth token');
      dispatch(refresh(refreshToken));
    }
  }, [needToProlongToken, refreshToken]);
}
