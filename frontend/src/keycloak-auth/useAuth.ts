import { useAppDispatch, useAppSelector } from '../storeHooks';
import { initKeycloak, selectAuthenticated, selectTokenExpiration } from './authSlice';
import { useEffect } from 'react';
import { currentDateLinuxTime } from '../helpers/date-helper';

export function useAuth() {
  const authenticated = useAppSelector(selectAuthenticated);
  const tokenExpiration = useAppSelector(selectTokenExpiration);
  const dispatch = useAppDispatch();

  useEffect(() => {
    const currentDateNumber = currentDateLinuxTime();

    const needAuth = !authenticated || !tokenExpiration || currentDateNumber > tokenExpiration;
    if (needAuth) {
      dispatch(initKeycloak());
    }
  }, []);
}
