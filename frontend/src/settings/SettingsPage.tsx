import React from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import { Box, Button, IconButton } from '@mui/material';
import { pages } from '../Routes';

function SettingsPage() {
  const settingsChildrenPages = pages.find((p) => p.key === 'settings')!.children!;
  const routeLocation = useLocation();
  return (
    <Box sx={{ width: '100%', display: 'flex' }}>
      <Box sx={{ display: 'flex', flexDirection: 'column' }}>
        {settingsChildrenPages.map((page) => (
          <Button
            key={page.key}
            sx={{ my: 1, display: 'block' }}
            color="secondary"
            href={page.route}
            variant={routeLocation.pathname === page.route ? 'contained' : 'outlined'}
          >
            <IconButton>{page.iconElement}</IconButton>
            {page.displayName}
          </Button>
        ))}
      </Box>
      <Box sx={{ flexGrow: 1, display: 'flex', margin: '10px' }}>
        <Outlet />
      </Box>
    </Box>
  );
}

export default SettingsPage;
