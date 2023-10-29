import React from 'react';
import { useAppSelector } from '../storeHooks';
import { selectAuthError } from '../keycloak-auth/authSlice';

function HomePage() {
  const authError = useAppSelector(selectAuthError);
  return (
    <>
      <p>Hello there</p>
      {authError}
    </>
  );
}

export default HomePage;
