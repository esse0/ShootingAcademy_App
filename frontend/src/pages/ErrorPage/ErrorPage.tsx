import { Box, Button, Container, Typography } from "@mui/material";
import { Link as RouterLink } from "react-router";
import errorImage from '../../assets/images/illustration-404.svg';

export default function ErrorPage(){
    return (
        <Container sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center',height: '80vh', justifyContent: 'center'}}>
            <Typography variant="h3" sx={{ mb: 2 }}>
            Sorry, page not found!
            </Typography>

            <Typography sx={{ color: 'text.secondary' }}>
            Sorry, we couldn’t find the page you’re looking for. Perhaps you’ve mistyped the URL? Be
            sure to check your spelling.
            </Typography>

            <Box
            component="img"
            src={errorImage}
            sx={{
                width: 320,
                height: 'auto',
                my: { xs: 5, sm: 10 },
            }}
            />

            <Button component={RouterLink} to="/app" size="large" variant="contained" sx={{ bgcolor: '#455CC7' }}>
            Go to home
            </Button>
      </Container>
    );
}