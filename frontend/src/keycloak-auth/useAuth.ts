import { useAppDispatch, useAppSelector } from '../storeHooks';
import {
  initKeycloak,
  prolongAuthToken,
  selectAuthenticated,
  selectTokenExpiration,
} from './authSlice';
import { useEffect, useRef } from 'react';
import { currentDateLinuxTime, dateFromUnixTime } from '../helpers/date-helper';
import keycloak from './keycloak';

export function useAuth() {
  const authenticated = useAppSelector(selectAuthenticated);
  const tokenExpiration = useAppSelector(selectTokenExpiration);
  const dispatch = useAppDispatch();

  const initializedOnce = useRef(false);
  const keycloakRef = useRef(keycloak);

  useEffect(() => {
    if (initializedOnce.current) {
      return;
    }
    const currentDateNumber = currentDateLinuxTime();

    const needAuth = !authenticated || !tokenExpiration || currentDateNumber > tokenExpiration;
    if (needAuth) {
      dispatch(initKeycloak(keycloakRef.current));
    }
    initializedOnce.current = true;
  }, []);

  useEffect(() => {
    keycloakRef.current.onTokenExpired = () => {
      dispatch(prolongAuthToken(keycloakRef.current));
    };
  }, [keycloakRef.current]);

  return keycloakRef.current;
}
