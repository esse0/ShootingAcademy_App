import { AppBar, Button, Drawer, IconButton, Link, List, ListItem, Toolbar, Typography, useMediaQuery } from "@mui/material"
import { NavLink as RouterLink } from "react-router"
import { useState } from "react";
import MenuIcon from '@mui/icons-material/Menu';
import NavigationLinkType from "../../types/NavigationLinkType";

import './Header.css'



function Header({menuItems}: {menuItems: NavigationLinkType[]}) {
  const [burgerState, setBurgerState] = useState(false);

  const isMobile = useMediaQuery('(max-width:960px)');

  const toggleBurger = (open: boolean) => () => {
    setBurgerState(open);
  };

  return (
    <AppBar className="header" position="static">
      <Toolbar className="header__body" variant="dense">
        <Typography className="header__logo">Shooting<span>Academy</span></Typography>

        {!isMobile ? (
          <>
            <nav className="navigation">
              <ul className="navigation__list">
                {menuItems.map(item => (
                  <li key={item.to}>
                    <Link className="navigation__link" component={RouterLink} to={item.to}>{item.label}</Link>
                  </li>
                ))}
              </ul>
            </nav>
            <div className="auth_links">
              <Button className="auth_links__button" variant="outlined" sx={{ borderBlockColor: '#455CC7', color: '#455CC7' }}  component={RouterLink} to='/signin'>Sign in</Button>
              <Button className="auth_links__button" variant="contained" sx={{ bgcolor: '#455CC7' }} component={RouterLink} to='/signup'>Sign up</Button>
            </div>
          </>
        ) : (
          <IconButton className="burger__menu" edge="end" aria-label="menu" onClick={toggleBurger(true)}>
            <MenuIcon className="burger__icon"/>
          </IconButton>
        )}

        <Drawer anchor="right" open={burgerState} onClose={toggleBurger(false)}>
          <List>
            {menuItems.map(item => (
              <ListItem  key={item.label} onClick={toggleBurger(false)}>
                <Link className="navigation__link" component={RouterLink} to={item.to}>{item.label}</Link>
              </ListItem>
            ))}
            <ListItem onClick={toggleBurger(false)}>
              <Link className="navigation__link" component={RouterLink} to="/signin">SignIn</Link>
            </ListItem>
            <ListItem onClick={toggleBurger(false)}>
              <Link className="navigation__link" component={RouterLink} to="/signup">SignUp</Link>
            </ListItem>
          </List>
        </Drawer>
      </Toolbar>
    </AppBar>
  );
}

export default Header