import React, { ReactElement } from 'react';
import { useAppSelector } from '../storeHooks';
import { selectAuthenticated } from './authSlice';

export interface InputProperties {
  children: ReactElement;
}

export function RequireAuth({ children }: InputProperties) {
  const authenticated = useAppSelector(selectAuthenticated);

  return authenticated ? children : <p>You are not authorized</p>;
}
