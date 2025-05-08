import { useNavigate } from 'react-router';
import { useEffect, useState } from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Stack, Typography } from '@mui/material';
import { DataGrid, GridActionsCellItem, GridColDef, GridRowId } from '@mui/x-data-grid';
import DeleteIcon from '@mui/icons-material/Delete';
import React from 'react';
import { CompetitionType } from '../../types/CompetitionBanerType';
import axios from 'axios';
import { useApi } from '../../hooks/useApi';

function CompetitionModeratePage(){
    const navigate = useNavigate();

    const [competitions, setCompetitions] = useState<CompetitionType[]>([]);
    const [open, setOpen] = React.useState(false);
    const [selectedId, setSelectedId] = React.useState<GridRowId | null>(null);

    const {resData: RecivedCompetitions, execute: executeGetCompetitions} = useApi<CompetitionType[]>(async ()=>{
        return axios.get('/api/competition/organisator');
    })

    const {execute: executeDelete, statusCode: RecivedDeleteCode, setStatusCode} = useApi(async ()=>{
        return axios.delete('/api/competition/delete', {params: {competitionId: selectedId}});
    })

    useEffect(()=>{
        executeGetCompetitions();
    }, [])

    useEffect(() => {
        if(!RecivedCompetitions) return;
        setCompetitions(RecivedCompetitions);
    }, [RecivedCompetitions]);

    useEffect(() => {
        if (RecivedDeleteCode === 200) {
            executeGetCompetitions();
            setStatusCode(null);
        }
    }, [RecivedDeleteCode]);
  

    const handleOpenDialog = (id: GridRowId) => {
        setSelectedId(id);
        setOpen(true);
    };

    const handleConfirmDelete = () => {
        executeDelete().then(() => {
            executeGetCompetitions();
        });

        handleCloseDialog();
        setStatusCode(null);
    };
    
    
    const handleCloseDialog = () => {
        setOpen(false);
        setSelectedId(null);
    };
   
    const columns: GridColDef[] = [
        { disableColumnMenu: true, field: 'id', headerName: 'ID', width: 70 },
        { field: 'title', headerName: 'Title', width: 130 },
        {
            disableColumnMenu: true,
            field: 'date',
            headerName: 'Date',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'time',
            headerName: 'Time',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'memberCount',
            headerName: 'Member Count',
            type: 'number',
        },
        {
            disableColumnMenu: true,
            field: 'maxMemberCount',
            headerName: 'Max member count',
            type: 'string',
        },
        {
            field: 'organiser',
            headerName: 'Organiser',
            type: 'string',
        },
        {
            field: 'actions',
            type: 'actions',
            headerName: 'Actions',
            width: 100,
            cellClassName: 'actions',
            getActions: ({ id }) => [
                <GridActionsCellItem
                  icon={<DeleteIcon />}
                  label="Delete"
                  onClick={() => handleOpenDialog(id)}
                  color="inherit"
                />,
              ],
        }
    ];

    return(
        <Stack>
            <Stack direction="row" justifyContent="space-between" alignItems={"center"}>
                <Typography variant="h4" fontWeight={"bold"} fontFamily="var(--primary-font)">My Competitions</Typography>
                <Button sx={{bgcolor: "#455CC7", color: "white"}} variant="contained" onClick={() => navigate("create")}>Create competition</Button>
            </Stack>
            <DataGrid
                rows={competitions}
                columns={columns}
                hideFooter
                sx={{ border: 0 }}
            />
            <Dialog open={open} onClose={handleCloseDialog}>
                <DialogTitle fontFamily={"var(--primary-font)"}>Delete competition?</DialogTitle>
                <DialogContent>
                    <DialogContentText fontFamily={"var(--primary-font)"}>
                        Are you sure you want to delete this competition? This action cannot be undone.
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button sx={{borderColor: "#455CC7", color: "#455CC7"}} variant="outlined" onClick={handleCloseDialog}>Cancel</Button>
                    <Button  variant="contained" onClick={handleConfirmDelete} color="error">Delete</Button>
                </DialogActions>
            </Dialog>
        </Stack>
    )
}

export default CompetitionModeratePage