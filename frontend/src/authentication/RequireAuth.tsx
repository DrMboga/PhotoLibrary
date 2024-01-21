import React, { ReactElement, useEffect, useState } from 'react';
import { useAppSelector } from '../storeHooks';
import { selectAuthenticated, selectTokenExpiration } from './authSlice';
import { Navigate } from 'react-router-dom';
import { currentDateLinuxTime } from '../helpers/date-helper';

export interface InputProperties {
  children: ReactElement;
}

export function RequireAuth({ children }: Readonly<InputProperties>) {
  const authenticated = useAppSelector(selectAuthenticated);
  const tokenExpirationDate = useAppSelector(selectTokenExpiration);
  const [needToLogin, setNeedToLogin] = useState<boolean>(true);

  useEffect(() => {
    const currentDateNumber = currentDateLinuxTime();
    const needAuth =
      !authenticated || !tokenExpirationDate || currentDateNumber > tokenExpirationDate;
    setNeedToLogin(needAuth);
  }, [authenticated, tokenExpirationDate]);

  if (needToLogin) {
    return <Navigate to="/login" />;
  } else {
    return children;
  }
}
