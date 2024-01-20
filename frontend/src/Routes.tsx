import { PageRouteInfo } from './nav-bar/pageRouteInfo';
import { RequireAuth } from './authentication/RequireAuth';
import LibraryPage from './photos/LibraryPage';
import CollectionsIcon from '@mui/icons-material/Collections';
import SettingsPage from './settings/SettingsPage';
import SettingsIcon from '@mui/icons-material/Settings';
import { ImporterPage } from './settings/Importer/ImporterPage';
import { GeocodingSettingsPage } from './settings/Geocoding/GeocodingSettingsPage';
import { AboutPage } from './about/AboutPage';
import InfoIcon from '@mui/icons-material/Info';
import ImportExportIcon from '@mui/icons-material/ImportExport';
import EditLocationIcon from '@mui/icons-material/EditLocation';
import React from 'react';
import PhotoAlbumIcon from '@mui/icons-material/PhotoAlbum';
import { AlbumsPage } from './photos/AlbumsPage';

export const pages: PageRouteInfo[] = [
  {
    key: 'library',
    route: '/', // TODO: Change path when spotlight page added
    displayName: 'All Photos',
    element: (
      <RequireAuth>
        <LibraryPage />
      </RequireAuth>
    ),
    iconElement: <CollectionsIcon fontSize="small" />,
  },
  {
    key: 'album',
    route: '/album',
    displayName: 'Albums',
    element: (
      <RequireAuth>
        <AlbumsPage />
      </RequireAuth>
    ),
    iconElement: <PhotoAlbumIcon fontSize="small" />,
  },
  {
    key: 'settings',
    route: '/settings',
    displayName: 'Settings',
    element: (
      <RequireAuth>
        <SettingsPage />
      </RequireAuth>
    ),
    iconElement: <SettingsIcon fontSize="small" />,
    children: [
      {
        key: 'importer',
        route: '/settings/importer',
        displayName: 'Import',
        element: <ImporterPage />,
        iconElement: <ImportExportIcon fontSize="small" />,
      },
      {
        key: 'geocoding',
        route: '/settings/geocoding',
        displayName: 'Geocoding',
        element: <GeocodingSettingsPage />,
        iconElement: <EditLocationIcon fontSize="small" />,
      },
    ],
  },
  {
    key: 'about',
    route: '/about',
    displayName: 'About',
    element: <AboutPage />,
    iconElement: <InfoIcon fontSize="small" />,
  },
];
