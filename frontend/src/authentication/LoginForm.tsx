// @flow
import * as React from 'react';
import {
  Alert,
  Box,
  Button,
  ButtonGroup,
  ClickAwayListener,
  FormControl,
  Grow,
  IconButton,
  InputAdornment,
  InputLabel,
  MenuList,
  OutlinedInput,
  Paper,
  Popper,
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useEffect, useRef, useState } from 'react';
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown';
import MenuItem from '@mui/material/MenuItem';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { login, register, selectAuthenticationStatus, selectAuthError } from './authSlice';
import { useNavigate } from 'react-router-dom';

const EmailInputName = 'email';
const PasswordInputName = 'password';

export function LoginForm() {
  const navigate = useNavigate();
  const authError = useAppSelector(selectAuthError);
  const authStatus = useAppSelector(selectAuthenticationStatus);
  const dispatch = useAppDispatch();
  const [showPassword, setShowPassword] = useState(false);
  const anchorRef = useRef<HTMLDivElement>(null);
  const [registerMenuOpen, setRegisterMenuOpen] = useState(false);

  const [needRegister, setNeedRegister] = useState(false);
  const [email, setEmail] = useState<string | undefined>();
  const [password, setPassword] = useState<string | undefined>();

  const handleClickShowPassword = () => setShowPassword((show) => !show);
  const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
  };

  const handleInputChange = (event: any) => {
    const { name, value } = event.target;
    switch (name) {
      case EmailInputName:
        setEmail(value);
        break;
      case PasswordInputName:
        setPassword(value);
        break;
    }
  };
  const submit = () => {
    if (needRegister) {
      dispatch(register({ email, password }));
    } else {
      dispatch(login({ email, password }));
    }
  };

  const handleMenuItemClick = (
    event: React.MouseEvent<HTMLLIElement, MouseEvent>,
    needRegister: boolean,
  ) => {
    setNeedRegister(!needRegister);
    setRegisterMenuOpen(false);
  };

  const handleToggle = () => {
    setRegisterMenuOpen((prevOpen) => !prevOpen);
  };

  const handleClose = (event: Event) => {
    if (anchorRef.current && anchorRef.current.contains(event.target as HTMLElement)) {
      return;
    }

    setRegisterMenuOpen(false);
  };

  useEffect(() => {
    if (authStatus === 'login success') {
      navigate('/');
    }
  }, [authStatus]);

  return (
    <Box
      sx={{
        display: 'flex',
        flexWrap: 'wrap',
        flexFlow: 'column',
        alignItems: 'center',
        marginTop: '10ch',
      }}
    >
      {authError && <Alert severity="error">{authError}</Alert>}
      {authStatus && <Alert severity="info">{authStatus}</Alert>}
      <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
        <InputLabel htmlFor="outlined-adornment-email">Email</InputLabel>
        <OutlinedInput
          id="outlined-adornment-email"
          type="text"
          name={EmailInputName}
          label="Email"
          onChange={handleInputChange}
        />
      </FormControl>
      <FormControl sx={{ m: 1, width: '25ch' }} variant="outlined">
        <InputLabel htmlFor="outlined-adornment-password">Password</InputLabel>
        <OutlinedInput
          id="outlined-adornment-password"
          type={showPassword ? 'text' : 'password'}
          endAdornment={
            <InputAdornment position="end">
              <IconButton
                aria-label="toggle password visibility"
                onClick={handleClickShowPassword}
                onMouseDown={handleMouseDownPassword}
                edge="end"
              >
                {showPassword ? <VisibilityOff /> : <Visibility />}
              </IconButton>
            </InputAdornment>
          }
          label="Password"
          name={PasswordInputName}
          onChange={handleInputChange}
        />
      </FormControl>
      <ButtonGroup
        variant="contained"
        ref={anchorRef}
        aria-label="split button"
        sx={{ marginTop: '1ch' }}
      >
        <Button onClick={submit}>{needRegister ? 'Register' : 'Login'}</Button>
        <Button
          size="small"
          aria-controls={registerMenuOpen ? 'split-button-menu' : undefined}
          aria-expanded={registerMenuOpen ? 'true' : undefined}
          aria-label="select login or register"
          aria-haspopup="menu"
          onClick={handleToggle}
        >
          <ArrowDropDownIcon />
        </Button>
      </ButtonGroup>
      <Popper
        sx={{
          zIndex: 1,
        }}
        open={registerMenuOpen}
        anchorEl={anchorRef.current}
        role={undefined}
        transition
        disablePortal
      >
        {({ TransitionProps, placement }) => (
          <Grow
            {...TransitionProps}
            style={{
              transformOrigin: placement === 'bottom' ? 'center top' : 'center bottom',
            }}
          >
            <Paper>
              <ClickAwayListener onClickAway={handleClose}>
                <MenuList id="register-button-menu" autoFocusItem>
                  <MenuItem
                    key="login-popper"
                    selected={!needRegister}
                    onClick={(event) => handleMenuItemClick(event, needRegister)}
                  >
                    Login
                  </MenuItem>
                  <MenuItem
                    key="register-popper"
                    selected={needRegister}
                    onClick={(event) => handleMenuItemClick(event, needRegister)}
                  >
                    Register
                  </MenuItem>
                </MenuList>
              </ClickAwayListener>
            </Paper>
          </Grow>
        )}
      </Popper>
    </Box>
  );
}
