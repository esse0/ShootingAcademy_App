import React, { useEffect } from "react";
import "./SignInPage.css";
import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Link,
  TextField,
  Typography,
} from "@mui/material";
import { Link as RouterLink, useNavigate } from "react-router";
import { LoginType } from "../../types/LoginType";
import { useApi } from "../../hooks/useApi";
import axios from "axios";
import { userAtom } from "../../jotai/atoms";
import { useSetAtom } from "jotai";
import { UserModelWithProfilePhoto } from "../../types/UserModelWithProfilePhoto";

export default function SignInPage() {
  const [emailError, setEmailError] = React.useState(false);
  const [emailErrorMessage, setEmailErrorMessage] = React.useState("");
  const [passwordError, setPasswordError] = React.useState(false);
  const [passwordErrorMessage, setPasswordErrorMessage] = React.useState("");
  // const [open, setOpen] = React.useState(false);

  const setUserFieldsToAtom = useSetAtom(userAtom);
  const navigate = useNavigate();

    const {resData: RecivedLogin, execute: executeLogin} = useApi<UserModelWithProfilePhoto, LoginType>(async (body)=>{
      return axios.post('/api/auth/signin', body);
    });

  // const handleClickOpen = () => {
  //   setOpen(true);
  // };

  // const handleClose = () => {
  //   setOpen(false);
  // };

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (emailError || passwordError) {
      return;
    }
    const formData = new FormData(event.currentTarget);
    const data = {
      email: (formData.get("email") as string) ?? "",
      password: (formData.get("password") as string) ?? "",
    };

    executeLogin(data);
  };

  const validateInputs = () => {
    const email = document.getElementById("email") as HTMLInputElement;
    const password = document.getElementById("password") as HTMLInputElement;

    let isValid = true;

    if (!email.value || !/\S+@\S+\.\S+/.test(email.value)) {
      setEmailError(true);
      setEmailErrorMessage("Please enter a valid email address.");
      isValid = false;
    } else {
      setEmailError(false);
      setEmailErrorMessage("");
    }

    if (!password.value || password.value.length < 6) {
      setPasswordError(true);
      setPasswordErrorMessage("Password must be at least 6 characters long.");
      isValid = false;
    } else {
      setPasswordError(false);
      setPasswordErrorMessage("");
    }

    return isValid;
  };

  useEffect(() => {
    if (!RecivedLogin) return;

    setUserFieldsToAtom(RecivedLogin);

    navigate("/app");
  }, [RecivedLogin]);

  return (
    <div className="signin_container">
      <Box
        component="form"
        onSubmit={handleSubmit}
        noValidate
        sx={{
          display: "flex",
          flexDirection: "column",
          width: "100%",
          gap: 2,
        }}
      >
        <FormControl>
          <FormLabel className="signin_label" htmlFor="email">
            Email
          </FormLabel>
          <TextField
            error={emailError}
            helperText={emailErrorMessage}
            id="email"
            type="email"
            name="email"
            placeholder="your@email.com"
            autoComplete="email"
            autoFocus
            required
            fullWidth
            variant="outlined"
            color={emailError ? "error" : "primary"}
          />
        </FormControl>
        <FormControl>
          <FormLabel className="signin_label" htmlFor="password">
            Password
          </FormLabel>
          <TextField
            error={passwordError}
            helperText={passwordErrorMessage}
            name="password"
            placeholder="••••••"
            type="password"
            id="password"
            autoComplete="current-password"
            autoFocus
            required
            fullWidth
            variant="outlined"
            color={passwordError ? "error" : "primary"}
          />
        </FormControl>
        {/* <FormControlLabel
          control={
            <Checkbox
              className="signin_label"
              value="remember"
              color="primary"
            />
          }
          label="Remember me"
        /> 
        <ForgotPassword open={open} handleClose={handleClose} />
        */}
        <Button
          className="signin_label"
          type="submit"
          fullWidth
          variant="contained"
          onClick={validateInputs}
          sx={{ bgcolor: '#455CC7'}}
        >
          Sign in
        </Button>
        {/* <Link
          className="signin_label"
          component="button"
          type="button"
          onClick={handleClickOpen}
          sx={{ alignSelf: "center" }}
        >
          Forgot your password?
        </Link>
      </Box>
      <Divider className="signin_label">or</Divider>
      <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <Button
          className="signin_label"
          fullWidth
          variant="outlined"
          onClick={() => alert("Sign in with Google")}
          startIcon={<GoogleIcon />}
        >
          Sign in with Google
        </Button>
        <Button
          className="signin_label"
          fullWidth
          variant="outlined"
          onClick={() => alert("Sign in with Facebook")}
          startIcon={<FacebookIcon />}
        >
          Sign in with Facebook
        </Button>
         */}
      </Box>
      <Typography className="signin_label" sx={{ textAlign: "center" }}>
          Don&apos;t have an account?{" "}
          <Link
            className="signin_label"
            component={RouterLink}
            to="/signup"
            sx={{ alignSelf: "center" }}
          >
            Sign up
          </Link>
        </Typography>
    </div>
  );
}
