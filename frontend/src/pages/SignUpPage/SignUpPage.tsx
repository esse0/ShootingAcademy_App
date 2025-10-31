import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import FormLabel from '@mui/material/FormLabel';
import FormControl from '@mui/material/FormControl';
import TextField from '@mui/material/TextField';
import Stack from '@mui/material/Stack';
import {Link as RouterLink, useNavigate} from 'react-router'
import './SignUpPage.css'
import { RegisterType } from '../../types/RegisterType';
import { useApi } from '../../hooks/useApi';
import { useEffect } from "react";
import axios from 'axios';
import { Link, Typography } from '@mui/material';
import { UserModelWithProfilePhoto } from '../../types/UserModelWithProfilePhoto';

export default function SignUp() {
  const [emailError, setEmailError] = React.useState(false);
  const [emailErrorMessage, setEmailErrorMessage] = React.useState('');
  const [passwordError, setPasswordError] = React.useState(false);
  const [passwordErrorMessage, setPasswordErrorMessage] = React.useState('');
  const [nameError, setNameError] = React.useState(false);
  const [nameErrorMessage, setNameErrorMessage] = React.useState('');
  const [lastNameError, setLastNameError] = React.useState(false);
  const [lastNameErrorMessage, setLastNameErrorMessage] = React.useState('');

  const navigate = useNavigate();

  const {statusCode, execute: executeRegister} = useApi<UserModelWithProfilePhoto, RegisterType>(async (body)=>{
    return axios.post('/api/auth/register', body);
  });

  const validateInputs = () => {
  const email = document.getElementById('email') as HTMLInputElement;
  const password = document.getElementById('password') as HTMLInputElement;
  const name = document.getElementById('name') as HTMLInputElement;
  const lastName = document.getElementById('lastName') as HTMLInputElement;

  let isValid = true;

  if (!email.value || !/\S+@\S+\.\S+/.test(email.value)) {
    setEmailError(true);
    setEmailErrorMessage('Please enter a valid email address.');
    isValid = false;
  } else {
    setEmailError(false);
    setEmailErrorMessage('');
  }

  if (!password.value || password.value.length < 6) {
    setPasswordError(true);
    setPasswordErrorMessage('Password must be at least 6 characters long.');
    isValid = false;
  } else {
    setPasswordError(false);
    setPasswordErrorMessage('');
  }

  if (!name.value || name.value.length < 1) {
    setNameError(true);
    setNameErrorMessage('First name is required.');
    isValid = false;
  } else {
    setNameError(false);
    setNameErrorMessage('');
  }

  if (!lastName.value || lastName.value.length < 1) {
    setLastNameError(true);
    setLastNameErrorMessage('Last name is required.');
    isValid = false;
  } else {
    setLastNameError(false);
    setLastNameErrorMessage('');
  }

  return isValid;
};


const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
  event.preventDefault();

  if (!validateInputs()) return;

  const formData = new FormData(event.currentTarget);
  const data = {
    name: formData.get('name') as string ?? '',
    lastName: formData.get('lastName') as string ?? '',
    email: formData.get('email') as string ?? '',
    password: formData.get('password') as string ?? '',
  };
  
    executeRegister(data);
  }

  useEffect(()=>{
    if(statusCode == 200)
        navigate('/signin');
  }, [statusCode])

  return (
      <Stack className='signup_container' direction="column" justifyContent="space-between">
          <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
          >
            <FormControl>
              <FormLabel className='signup_text' htmlFor="name">Name</FormLabel>
              <TextField
                className='signup_text'
                autoComplete="name"
                name="name"
                required
                fullWidth
                id="name"
                placeholder="Jon"
                error={nameError}
                helperText={nameErrorMessage}
                color={nameError ? 'error' : 'primary'}
              />
            </FormControl>
            <FormControl>
              <FormLabel className='signup_text' htmlFor="lastName">Last name</FormLabel>
              <TextField
                className='signup_text'
                autoComplete="family-name"
                name="lastName"
                required
                fullWidth
                id="lastName"
                placeholder="Snow"
                error={lastNameError}
                helperText={lastNameErrorMessage}
                color={lastNameError ? 'error' : 'primary'}
              />
            </FormControl>
            <FormControl>
              <FormLabel className='signup_text' htmlFor="email">Email</FormLabel>
              <TextField
                className='signup_text'
                required
                fullWidth
                id="email"
                placeholder="your@email.com"
                name="email"
                autoComplete="email"
                variant="outlined"
                error={emailError}
                helperText={emailErrorMessage}
                color={passwordError ? 'error' : 'primary'}
              />
            </FormControl>
            <FormControl>
              <FormLabel className='signup_text' htmlFor="password">Password</FormLabel>
              <TextField
                className='signup_text'
                required
                fullWidth
                name="password"
                placeholder="••••••"
                type="password"
                id="password"
                autoComplete="new-password"
                variant="outlined"
                error={passwordError}
                helperText={passwordErrorMessage}
                color={passwordError ? 'error' : 'primary'}
              />
            </FormControl>
            {/* <FormControlLabel
              className='signup_text'
              control={<Checkbox value="allowExtraEmails" color="primary" />}
              label="I want to receive updates via email."
            /> */}
            <Button
              className='signup_text'
              sx={{ bgcolor: '#455CC7'}}
              type="submit"
              fullWidth
              variant="contained"
              onClick={validateInputs}
            >
              Sign up
            </Button>
          </Box>
          {/* <Divider>
            <Typography className='signup_text' sx={{ color: 'text.secondary' }}>or</Typography>
          </Divider>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <Button
              className='signup_text'
              sx={{ color: '#455CC7', borderColor: '#455CC7' }}
              fullWidth
              variant="outlined"
              onClick={() => alert('Sign up with Google')}
              startIcon={<GoogleIcon />}
            >
              Sign up with Google
            </Button>
            <Button
              className='signup_text'
              sx={{ color: '#455CC7', borderColor: '#455CC7' }}
              fullWidth
              variant="outlined"
              onClick={() => alert('Sign up with Facebook')}
              startIcon={<FacebookIcon />}
            >
              Sign up with Facebook
            </Button>
          </Box>*/}

            <Typography className='signup_text' sx={{ textAlign: 'center', mt:"20px" }}>
              Already have an account?{' '}
              <Link
                className='signup_text'
                color="#455CC7"
                component={RouterLink}
                to="/signin"
                variant="body2"
                sx={{ alignSelf: 'center' }}
              >
                Sign in
              </Link>
            </Typography> 
      </Stack>
  );
}