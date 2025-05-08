import * as React from 'react';
import { styled, useTheme, Theme, CSSObject } from '@mui/material/styles';
import Box from '@mui/material/Box';
import MuiDrawer from '@mui/material/Drawer';
import MuiAppBar, { AppBarProps as MuiAppBarProps } from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import List from '@mui/material/List';
import CssBaseline from '@mui/material/CssBaseline';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import { Outlet, NavLink as RouterLink, useNavigate } from 'react-router';
import DashboardIcon from '@mui/icons-material/Dashboard';
import GroupIcon from '@mui/icons-material/Group';
import { Avatar, Container, Menu, MenuItem, Stack } from '@mui/material';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import SchoolIcon from '@mui/icons-material/School';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';
import { useAtomValue } from 'jotai';
import { userAtom } from '../../jotai/atoms';
import { FullUserModel } from '../../types/UserProfileData';
import ViewListIcon from '@mui/icons-material/ViewList';
import { useTestAuth } from '../../hooks/useTestAuth';
import { useEffect } from 'react';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import ExtensionIcon from '@mui/icons-material/Extension';

const drawerWidth = 300;

const openedMixin = (theme: Theme): CSSObject => ({
    width: drawerWidth,
    transition: theme.transitions.create('width', {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.enteringScreen,
    }),
    overflowX: 'hidden',
});

const closedMixin = (theme: Theme): CSSObject => ({
    transition: theme.transitions.create('width', {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
    }),
    overflowX: 'hidden',
    width: `calc(${theme.spacing(7)} + 1px)`,
    [theme.breakpoints.up('sm')]: {
        width: `calc(${theme.spacing(8)} + 1px)`,
    },
});

const DrawerHeader = styled('div')(({ theme }) => ({
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
    padding: theme.spacing(0, 1),

    ...theme.mixins.toolbar,
}));

interface AppBarProps extends MuiAppBarProps {
    open?: boolean;
}

const AppBar = styled(MuiAppBar, {
    shouldForwardProp: (prop) => prop !== 'open',
})<AppBarProps>(({ theme }) => ({
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
    }),
    variants: [
        {
            props: ({ open }) => open,
            style: {
                marginLeft: drawerWidth,
                width: `calc(100% - ${drawerWidth}px)`,
                transition: theme.transitions.create(['width', 'margin'], {
                    easing: theme.transitions.easing.sharp,
                    duration: theme.transitions.duration.enteringScreen,
                }),
            },
        },
    ],
}));

const Drawer = styled(MuiDrawer, {
    shouldForwardProp: (prop) => prop !== 'open',
})(({ theme }) => ({
    width: drawerWidth,
    flexShrink: 0,
    whiteSpace: 'nowrap',
    boxSizing: 'border-box',
    variants: [
        {
            props: ({ open }) => open,
            style: {
                ...openedMixin(theme),
                '& .MuiDrawer-paper': openedMixin(theme),
            },
        },
        {
            props: ({ open }) => !open,
            style: {
                ...closedMixin(theme),
                '& .MuiDrawer-paper': closedMixin(theme),
            },
        },
    ],
}));

export default function AppMenu() {
    const theme = useTheme();
    const [open, setOpen] = React.useState(false);

    useTestAuth();
    const navigate = useNavigate();

    const menuItems = [
        { icon: <DashboardIcon />, label: 'My activity', to: 'myactivity' },
        { icon: <SchoolIcon />, label: 'Courses', to: 'courses' },
        {
            icon: <EmojiEventsIcon />,
            label: 'Competitions',
            to: 'competitions',
        },
        { icon: <GroupIcon />, label: 'My groups', to: 'mygroup' },
    ];

    const userData: FullUserModel = useAtomValue(userAtom);

    if (userData && userData.role == 'organisator')
        menuItems.push({
            icon: <ViewListIcon />,
            label: 'Competition moderation',
            to: 'moderatecompetitions',
        });

    if (userData && userData.role == 'moderator')
        menuItems.push({
            icon: <ViewListIcon />,
            label: 'Courses moderation',
            to: 'moderatecourses',
        });

    if (userData && userData.role == 'admin')
        menuItems.push({
            icon: <ExtensionIcon />,
            label: 'Admin panel',
            to: 'adminPanel',
        });

    const handleDrawerOpen = () => {
        setOpen(true);
    };

    const handleDrawerClose = () => {
        setOpen(false);
    };

    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const isMenuOpen = Boolean(anchorEl);

    // Открытие меню
    const handleOpenMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    // Закрытие меню
    const handleCloseMenu = () => {
        setAnchorEl(null);
    };

    function NavigateToSettings(): void {
        navigate('/app/settings');
        handleCloseMenu();
    }

    const { execute: executeLogout, statusCode: RecivedStatus } = useApi(async () => {
        return axios.post('/api/auth/signout');
    });

    function Logout(): void {
        executeLogout();
    }

    useEffect(() => {
        if (RecivedStatus == 200) {
            navigate('/');

            handleCloseMenu();
        }
    }, [RecivedStatus]);

    return (
        <Box sx={{ display: 'flex' }}>
            <CssBaseline />
            <AppBar
                position="fixed"
                open={open}
                sx={{
                    bgcolor: 'white',
                    color: '#424242',
                    fontFamily: 'var(--primary-font)',
                }}
            >
                <Toolbar>
                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        onClick={handleDrawerOpen}
                        edge="start"
                        sx={[
                            {
                                marginRight: 5,
                            },
                            open && { display: 'none' },
                        ]}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography variant="h6" noWrap component="div" sx={{ fontFamily: 'inherit', fontWeight: 800 }}>
                        <span style={{ color: '#455CC7' }}>Shooting</span>
                        Academy
                    </Typography>
                </Toolbar>
            </AppBar>
            <Drawer variant="permanent" open={open}>
                <DrawerHeader>
                    <IconButton onClick={handleDrawerClose}>
                        {theme.direction === 'rtl' ? <ChevronRightIcon /> : <ChevronLeftIcon />}
                    </IconButton>
                </DrawerHeader>
                <Divider />
                <Container
                    disableGutters
                    sx={{
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'space-between',
                        height: '100%',
                    }}
                >
                    <List>
                        {menuItems.map((element, index) => (
                            <ListItem
                                className="navigation__link"
                                key={index}
                                component={RouterLink}
                                to={element.to}
                                disablePadding
                                sx={{ display: 'block', color: '#424242' }}
                            >
                                <ListItemButton
                                    sx={[
                                        {
                                            minHeight: 48,
                                            px: 2.5,
                                        },
                                        open
                                            ? {
                                                  justifyContent: 'initial',
                                              }
                                            : {
                                                  justifyContent: 'center',
                                              },
                                    ]}
                                >
                                    <ListItemIcon
                                        sx={[
                                            {
                                                minWidth: 0,
                                                justifyContent: 'center',
                                                color: 'inherit',
                                            },
                                            open
                                                ? {
                                                      mr: 3,
                                                  }
                                                : {
                                                      mr: 'auto',
                                                  },
                                        ]}
                                    >
                                        {element.icon}
                                    </ListItemIcon>
                                    <ListItemText
                                        primaryTypographyProps={{
                                            fontFamily: 'inherit',
                                            fontWeight: 500,
                                        }}
                                        primary={element.label}
                                        sx={[
                                            open
                                                ? {
                                                      opacity: 1,
                                                  }
                                                : {
                                                      opacity: 0,
                                                  },
                                        ]}
                                    />
                                </ListItemButton>
                            </ListItem>
                        ))}
                    </List>

                    <Container disableGutters sx={{ display: 'flex', flexDirection: 'column' }}>
                        <Divider />
                        <Stack
                            direction="row"
                            gap={'5px'}
                            p="10px"
                            alignItems="center"
                            justifyContent={open ? 'flex-start' : 'center'}
                        >
                            <Stack direction="row" alignItems="center" spacing={2}>
                                {!open ? (
                                    <IconButton onClick={handleOpenMenu} sx={{ p: 0 }}>
                                        <Avatar sx={{ width: 48, height: 48 }} />
                                    </IconButton>
                                ) : (
                                    <Avatar sx={{ width: 48, height: 48 }} />
                                )}

                                {open && (
                                    <Stack>
                                        <Typography
                                            variant="body2"
                                            sx={{
                                                fontWeight: 'bolder',
                                                fontFamily: 'inherit',
                                            }}
                                        >
                                            {userData.firstName} {userData.secoundName}
                                        </Typography>
                                        <Typography variant="caption" sx={{ fontFamily: 'inherit' }}>
                                            {userData.email}
                                        </Typography>
                                    </Stack>
                                )}
                            </Stack>
                            {open && (
                                <IconButton
                                    size="small"
                                    onClick={handleOpenMenu}
                                    sx={{
                                        borderRadius: '8px',
                                        border: '1px solid hsl(220, 20%, 88%)',
                                        width: 36,
                                        height: 36,
                                        ml: 'auto',
                                    }}
                                >
                                    <MoreVertIcon htmlColor="black" fontSize="small" sx={{ width: 16, height: 16 }} />
                                </IconButton>
                            )}
                        </Stack>

                        <Menu anchorEl={anchorEl} open={isMenuOpen} onClose={handleCloseMenu}>
                            <MenuItem onClick={NavigateToSettings}>Settings</MenuItem>
                            <MenuItem onClick={Logout}>Logout</MenuItem>
                        </Menu>
                    </Container>
                </Container>
            </Drawer>
            <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
                <DrawerHeader />
                <Outlet />
            </Box>
        </Box>
    );
}
