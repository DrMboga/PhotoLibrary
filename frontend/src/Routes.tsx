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
import { LoginForm } from './authentication/LoginForm';
import { MediasOfTheDayPage } from './photos/MediasOfTheDayPage';
import TodayIcon from '@mui/icons-material/Today';
import { MediasByLabelPage } from './photos/MediasByLabelPage';
import SellIcon from '@mui/icons-material/Sell';
import PlaceIcon from '@mui/icons-material/Place';
import { GeoLocationSummaryPage } from './geolocation/GeoLocationSummaryPage';
import { GeoLocationRegionPage } from './geolocation/GeoLocationRegionPage';
import DeleteIcon from '@mui/icons-material/Delete';
import { DeletedMediasPage } from './photos/DeletedMediasPage';

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
    showMenuButton: true,
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
    showMenuButton: true,
  },
  {
    key: 'mediaoftheday',
    route: '/mediaoftheday',
    displayName: 'Today',
    element: (
      <RequireAuth>
        <MediasOfTheDayPage />
      </RequireAuth>
    ),
    iconElement: <TodayIcon fontSize="small" />,
    showMenuButton: true,
  },
  {
    key: 'mediasbylabel',
    route: '/mediasbylabel',
    displayName: 'Labels',
    element: (
      <RequireAuth>
        <MediasByLabelPage />
      </RequireAuth>
    ),
    iconElement: <SellIcon fontSize="small" />,
    showMenuButton: true,
  },
  {
    key: 'places',
    route: '/places',
    displayName: 'Places',
    element: (
      <RequireAuth>
        <GeoLocationSummaryPage />
      </RequireAuth>
    ),
    iconElement: <PlaceIcon fontSize="small" />,
    showMenuButton: true,
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
        showMenuButton: true,
      },
      {
        key: 'geocoding',
        route: '/settings/geocoding',
        displayName: 'Geocoding',
        element: <GeocodingSettingsPage />,
        iconElement: <EditLocationIcon fontSize="small" />,
        showMenuButton: true,
      },
    ],
    showMenuButton: true,
  },
  {
    key: 'deleted',
    route: '/deleted',
    displayName: 'Deleted',
    element: (
      <RequireAuth>
        <DeletedMediasPage />
      </RequireAuth>
    ),
    iconElement: <DeleteIcon fontSize="small" />,
    showMenuButton: true,
  },
  {
    key: 'about',
    route: '/about',
    displayName: 'About',
    element: <AboutPage />,
    iconElement: <InfoIcon fontSize="small" />,
    showMenuButton: true,
  },
  {
    key: 'login',
    route: '/login',
    displayName: 'Login',
    element: <LoginForm />,
    iconElement: <InfoIcon fontSize="small" />,
    showMenuButton: false,
  },
  {
    key: 'region-info',
    route: '/region/:region',
    displayName: 'Region',
    element: (
      <RequireAuth>
        <GeoLocationRegionPage />
      </RequireAuth>
    ),
    iconElement: <InfoIcon fontSize="small" />,
    showMenuButton: false,
  },
];
