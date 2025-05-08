import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
    Stack,
    Typography,
} from '@mui/material';
import { DataGrid, GridActionsCellItem, GridColDef, GridRowId } from '@mui/x-data-grid';
import React, { useEffect, useState } from 'react';
import { FullUserModel } from '../../types/UserProfileData';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { ChangeRoleType } from '../../types/ChangeRoleType';
import AccountCircleTwoToneIcon from '@mui/icons-material/AccountCircleTwoTone';

export function AdminPage() {
    const [open, setOpen] = useState(false);
    const [selectedId, setSelectedId] = React.useState<GridRowId>('');

    const { resData, execute } = useApi<FullUserModel[]>(async () => {
        return axios.get('/api/User/get');
    });

    const { statusCode: statusCodeChangeRole, execute: executeChangeRole } = useApi<null, ChangeRoleType>(
        async (body) => {
            return axios.post('/api/User/changerole', null, { params: body });
        },
    );

    const handleOpenDialog = (id: GridRowId) => {
        setSelectedId(id);
        setOpen(true);
    };

    const handleChangeRole = (newRole: string) => {
        executeChangeRole({ userId: selectedId.toString(), newRole });
        handleCloseDialog();
    };

    const handleCloseDialog = () => {
        setOpen(false);
        setSelectedId('');
    };

    useEffect(() => {
        if (statusCodeChangeRole == 200) {
            alert('Роль обновленна');
            window.location.reload();
        }
    }, [statusCodeChangeRole]);

    useEffect(() => {
        execute();
    }, []);

    useEffect(() => {
        console.log(resData);
    }, [resData]);

    const columns: GridColDef[] = [
        { disableColumnMenu: true, field: 'id', headerName: 'ID', width: 70 },
        {
            disableColumnMenu: true,
            field: 'firstName',
            headerName: 'Name',
            type: 'string',
            width: 200,
        },
        {
            disableColumnMenu: true,
            field: 'secoundName',
            headerName: 'Secound Name',
            type: 'string',
            width: 200,
        },
        {
            disableColumnMenu: true,
            field: 'age',
            headerName: 'Age',
            type: 'number',
            width: 80,
        },
        {
            disableColumnMenu: true,
            field: 'grade',
            headerName: 'Grade',
            type: 'string',
            width: 200,
        },
        {
            disableColumnMenu: true,
            field: 'email',
            headerName: 'Email',
            type: 'string',
            width: 200,
        },
        {
            disableColumnMenu: true,
            field: 'role',
            headerName: 'Role',
            type: 'string',
            width: 150,
        },
        {
            field: 'actions',
            type: 'actions',
            headerName: 'Actions',
            width: 100,
            cellClassName: 'actions',
            getActions: ({ id }) => [
                <GridActionsCellItem
                    icon={<AccountCircleTwoToneIcon />}
                    label="ChangeRole"
                    onClick={() => handleOpenDialog(id)}
                    color="inherit"
                />,
            ],
        },
    ];

    return (
        <Stack>
            <Typography variant="h4" fontWeight={'bold'} fontFamily="var(--primary-font)">
                Admin panel
            </Typography>

            <DataGrid rows={resData || []} columns={columns} hideFooter sx={{ border: 0 }} />
            <Dialog open={open} onClose={handleCloseDialog}>
                <DialogTitle fontFamily={'var(--primary-font)'}>Change user role?</DialogTitle>
                <DialogContent>
                    <DialogContentText fontFamily={'var(--primary-font)'}>
                        You can set one of the following roles of your choice
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button
                        sx={{ borderColor: '#455CC7', color: '#455CC7' }}
                        variant="outlined"
                        onClick={() => {
                            handleChangeRole('athlete');
                        }}
                    >
                        Set athlet
                    </Button>
                    <Button
                        variant="outlined"
                        onClick={() => {
                            handleChangeRole('coach');
                        }}
                        sx={{ borderColor: '#455CC7', color: '#455CC7' }}
                    >
                        Set coach
                    </Button>
                    <Button
                        variant="outlined"
                        onClick={() => {
                            handleChangeRole('moderator');
                        }}
                        sx={{ borderColor: '#455CC7', color: '#455CC7' }}
                    >
                        Set moderator
                    </Button>
                    <Button
                        variant="outlined"
                        onClick={() => {
                            handleChangeRole('organisator');
                        }}
                        sx={{ borderColor: '#455CC7', color: '#455CC7' }}
                    >
                        Set organisator
                    </Button>
                    <Button
                        sx={{ borderColor: '#455CC7', color: 'white' }}
                        variant="contained"
                        onClick={handleCloseDialog}
                    >
                        Cancel window
                    </Button>
                </DialogActions>
            </Dialog>
        </Stack>
    );
}
