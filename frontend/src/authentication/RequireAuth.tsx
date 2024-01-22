import React, { ReactElement, useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { clearLoginState, selectAuthenticated, selectTokenExpiration } from './authSlice';
import { useNavigate } from 'react-router-dom';
import { currentDateLinuxTime } from '../helpers/date-helper';
import { Alert } from '@mui/material';

export interface InputProperties {
  children: ReactElement;
}

export function RequireAuth({ children }: Readonly<InputProperties>) {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const authenticated = useAppSelector(selectAuthenticated);
  const tokenExpirationDate = useAppSelector(selectTokenExpiration);
  const [needToLogin, setNeedToLogin] = useState<boolean>(true);

  useEffect(() => {
    const currentDateNumber = currentDateLinuxTime();
    const needAuth =
      !authenticated || !tokenExpirationDate || currentDateNumber > tokenExpirationDate;
    setNeedToLogin(needAuth);
    if (needAuth) {
      dispatch(clearLoginState());
      navigate('/login');
    }
  }, [authenticated, tokenExpirationDate]);

  if (needToLogin) {
    return <Alert severity="warning">You are not authorized!</Alert>;
  } else {
    return children;
  }
}
