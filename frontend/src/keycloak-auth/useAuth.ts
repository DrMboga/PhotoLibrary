import { useAppDispatch, useAppSelector } from '../storeHooks';
import { initKeycloak, selectAuthenticated, selectTokenExpiration } from './authSlice';
import { useEffect, useRef } from 'react';
import { currentDateLinuxTime } from '../helpers/date-helper';

export function useAuth() {
  const authenticated = useAppSelector(selectAuthenticated);
  const tokenExpiration = useAppSelector(selectTokenExpiration);
  const dispatch = useAppDispatch();

  const initializedOnce = useRef(false);

  useEffect(() => {
    if (initializedOnce.current) {
      return;
    }
    const currentDateNumber = currentDateLinuxTime();

    const needAuth = !authenticated || !tokenExpiration || currentDateNumber > tokenExpiration;
    if (needAuth) {
      dispatch(initKeycloak());
    }
    initializedOnce.current = true;
  }, []);
}
