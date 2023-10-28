import * as React from 'react';
import { PageRouteInfo } from './pageRouteInfo';
import { useState } from 'react';
import {
  AppBar,
  Avatar,
  Box,
  Button,
  Container,
  IconButton,
  ListItemIcon,
  Menu,
  Toolbar,
  Tooltip,
  Typography,
} from '@mui/material';
import CameraIcon from '@mui/icons-material/Camera';
import MenuIcon from '@mui/icons-material/Menu';
import { Link } from 'react-router-dom';
import MenuItem from '@mui/material/MenuItem';
import DarkModeIcon from '@mui/icons-material/DarkMode';
import LightModeIcon from '@mui/icons-material/LightMode';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { selectTheme, toggleTheme } from '../appSlice';
import { logoutKeycloak, selectAuthenticated, selectUserName } from '../keycloak-auth/authSlice';

type Props = {
  routesInfo: PageRouteInfo[];
};

const logo = 'Photos';

export const NavBar = ({ routesInfo }: Props) => {
  const theme = useAppSelector(selectTheme);
  const authenticated = useAppSelector(selectAuthenticated);
  const userName = useAppSelector(selectUserName);
  const dispatch = useAppDispatch();
  const [anchorElNav, setAnchorElNav] = useState<null | HTMLElement>(null);
  const [anchorElUser, setAnchorElUser] = useState<null | HTMLElement>(null);

  const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElNav(event.currentTarget);
  };
  const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleCloseNavMenu = () => {
    setAnchorElNav(null);
  };

  const handleCloseUserMenu = () => {
    setAnchorElUser(null);
  };

  const handleLogout = () => {
    dispatch(logoutKeycloak());
    handleCloseUserMenu();
  };

  const handleToggleTheme = () => {
    dispatch(toggleTheme());
  };

  return (
    <AppBar position="static">
      <Container maxWidth="xl">
        <Toolbar disableGutters>
          <CameraIcon sx={{ display: { xs: 'none', md: 'flex' }, mr: 1 }}></CameraIcon>
          <Typography
            variant="h6"
            noWrap
            component="a"
            href="/"
            sx={{
              mr: 2,
              display: { xs: 'none', md: 'flex' },
              fontFamily: 'monospace',
              fontWeight: 700,
              letterSpacing: '.3rem',
              color: 'inherit',
              textDecoration: 'none',
            }}
          >
            {logo}
          </Typography>

          <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
            <IconButton
              size="large"
              aria-label="account of current user"
              aria-controls="menu-appbar"
              aria-haspopup="true"
              onClick={handleOpenNavMenu}
              color="inherit"
            >
              <MenuIcon />
            </IconButton>
            <Menu
              id="menu-appbar"
              anchorEl={anchorElNav}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'left',
              }}
              keepMounted
              transformOrigin={{
                vertical: 'top',
                horizontal: 'left',
              }}
              open={Boolean(anchorElNav)}
              onClose={handleCloseNavMenu}
              sx={{
                display: { xs: 'block', md: 'none' },
              }}
            >
              {routesInfo.map((page) => (
                <Link to={page.route} key={page.key}>
                  <MenuItem key={page.key} onClick={handleCloseNavMenu}>
                    <ListItemIcon>{page.iconElement}</ListItemIcon>
                    <Typography textAlign="center">{page.displayName}</Typography>
                  </MenuItem>
                </Link>
              ))}
            </Menu>
          </Box>

          <CameraIcon sx={{ display: { xs: 'flex', md: 'none' }, mr: 1 }}></CameraIcon>
          <Typography
            variant="h5"
            noWrap
            component="a"
            href="#app-bar-with-responsive-menu"
            sx={{
              mr: 2,
              display: { xs: 'flex', md: 'none' },
              flexGrow: 1,
              fontFamily: 'monospace',
              fontWeight: 700,
              letterSpacing: '.3rem',
              color: 'inherit',
              textDecoration: 'none',
            }}
          >
            {logo}
          </Typography>
          <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
            {routesInfo.map((page) => (
              <Button
                key={page.key}
                onClick={handleCloseNavMenu}
                sx={{ my: 2, color: 'white', display: 'block' }}
                href={page.route}
              >
                <IconButton sx={{ color: 'white' }}>{page.iconElement}</IconButton>
                {page.displayName}
              </Button>
            ))}
          </Box>

          <Box sx={{ flexGrow: 0 }}>
            <Tooltip title={theme === 'light' ? 'Dark' : 'Light'}>
              <IconButton onClick={handleToggleTheme} sx={{ p: 2 }}>
                {theme === 'light' ? <DarkModeIcon sx={{ color: 'white' }} /> : <LightModeIcon />}
              </IconButton>
            </Tooltip>
          </Box>

          {authenticated && (
            <Box sx={{ flexGrow: 0 }}>
              <Tooltip title={userName}>
                <IconButton onClick={handleOpenUserMenu} sx={{ p: 1 }}>
                  <Avatar alt={userName} src="/public/avatar/2.jpg" />
                </IconButton>
              </Tooltip>
              <Menu
                sx={{ mt: '45px' }}
                id="menu-appbar"
                anchorEl={anchorElUser}
                anchorOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                keepMounted
                transformOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                open={Boolean(anchorElUser)}
                onClose={handleCloseUserMenu}
              >
                <MenuItem key="log-out" onClick={handleLogout}>
                  <Typography textAlign="center">Logout</Typography>
                </MenuItem>
              </Menu>
            </Box>
          )}
        </Toolbar>
      </Container>
    </AppBar>
  );
};
