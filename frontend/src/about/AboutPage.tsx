// @flow
import * as React from 'react';
import { Box, Button, Container, Typography } from '@mui/material';
import CopyrightIcon from '@mui/icons-material/Copyright';
import LinkedInIcon from '@mui/icons-material/LinkedIn';
import GitHubIcon from '@mui/icons-material/GitHub';
export function AboutPage() {
  const version = process.env.REACT_APP_VERSION;

  return (
    <Container fixed>
      <Box sx={{ marginTop: '50px' }}>
        <Typography variant="h4"> Photo library application</Typography>
        <Typography variant="subtitle1">Version {version}</Typography>
      </Box>
      <Box sx={{ marginTop: '10px' }}></Box>
      <Box sx={{ marginTop: '100px', display: 'flex', alignItems: 'center', gap: '5px' }}>
        <CopyrightIcon /> 2024 Mikhail Shabanov
      </Box>
      <Box sx={{ marginTop: '10px', display: 'flex', alignItems: 'center', gap: '5px' }}>
        <Button
          variant="outlined"
          size="small"
          href="https://www.linkedin.com/in/shabanov/"
          sx={{ display: 'flex', alignItems: 'center', gap: '5px' }}
        >
          <LinkedInIcon type="small" /> Mikhail Shabanov
        </Button>
        <Button
          variant="outlined"
          size="small"
          href="https://github.com/DrMboga"
          sx={{ display: 'flex', alignItems: 'center', gap: '5px' }}
        >
          <GitHubIcon type="small" /> Mikhail Shabanov
        </Button>
      </Box>
    </Container>
  );
}
