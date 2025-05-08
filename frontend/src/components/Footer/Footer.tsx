import { NavLink as RouterLink } from "react-router"
import { Link, Typography } from "@mui/material";

import FacebookIcon from '@mui/icons-material/Facebook';
import InstagramIcon from '@mui/icons-material/Instagram';
import XIcon from '@mui/icons-material/X';
import LinkedInIcon from '@mui/icons-material/LinkedIn';

import NavigationLinkType from "../../types/NavigationLinkType"
import './Footer.css'


function Footer({menuItems}: {menuItems: NavigationLinkType[]}) {
    return (
     <footer>
        <div className="footer__content">
            <div className="footer__main">
                <Typography className="footer__logo">
                    Shooting<span>Academy</span>
                </Typography>
                <ul className="footer__links">
                    {menuItems.map((item) => (
                    <li key={item.to}>
                        <Link className="footer__link navigation__link" component={RouterLink} to={item.to}>{item.label}</Link>
                    </li>
                    ))}
                </ul>
            </div>
            <div className="footer__footnote">
                <div className="footer__copyright">
                    © 2024 ShootingAcademy. All rights reserved.
                </div>
                <div className="footer__social footer__links">
                    <div className="footer__social__item">
                        <Link href="https://www.facebook.com/">
                            <FacebookIcon className="footer__social__item__icon"></FacebookIcon>
                        </Link>
                    </div>
                    <div className="footer__social__item">
                        <Link href="https://x.com/">
                            <XIcon className="footer__social__item__icon"></XIcon>
                        </Link>
                    </div>
                    <div className="footer__social__item">
                        <Link href="https://www.instagram.com/">
                            <InstagramIcon className="footer__social__item__icon"></InstagramIcon>
                        </Link>
                    </div>
                    <div className="footer__social__item">
                        <Link href="https://www.linkedin.com/">
                            <LinkedInIcon className="footer__social__item__icon"></LinkedInIcon>
                        </Link>
                    </div>
                </div>
            </div>
        </div>
     </footer>
    );
  }
  
  export default Footer