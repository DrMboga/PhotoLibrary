import { ReactElement } from 'react';

export interface PageRouteInfo {
  key: string;
  route: string;
  displayName: string;
  element: ReactElement;
  iconElement: ReactElement;
}
