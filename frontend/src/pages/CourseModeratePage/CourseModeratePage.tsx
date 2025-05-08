import { useNavigate } from 'react-router';
import { useEffect, useState } from 'react';
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Stack, Typography } from '@mui/material';
import { DataGrid, GridActionsCellItem, GridColDef, GridRowId } from '@mui/x-data-grid';
import CourseBannerType from '../../types/CourseBannerType';
import DeleteIcon from '@mui/icons-material/Delete';
import React from 'react';
import axios from 'axios';
import { useApi } from '../../hooks/useApi';

function CourseModeratePage(){
     const navigate = useNavigate();

    const [courses, setCourses] = useState<CourseBannerType[]>([]);
    const [open, setOpen] = React.useState(false);
    const [selectedId, setSelectedId] = React.useState<GridRowId | null>(null);

    const {resData: RecivedCourses, execute: executeGetCourses, setStatusCode} = useApi<CourseBannerType[]>(async ()=>{
        return axios.get('/api/course/admin');
    })

    const {execute: executeDelete, statusCode: RecivedDeleteCode} = useApi(async ()=>{
        return axios.delete('/api/course/delete', {params: {courseId: selectedId}});
    })

    useEffect(()=>{
        executeGetCourses();
    }, [])

    useEffect(() => {
        if(!RecivedCourses) return;
        setCourses(RecivedCourses);
    }, [RecivedCourses]);

    useEffect(() => {
        if (RecivedDeleteCode === 200) {
            executeGetCourses();
            setStatusCode(null);
        }
    }, [RecivedDeleteCode]);

    const handleOpenDialog = (id: GridRowId) => {
        setSelectedId(id);
        setOpen(true);
    };
    
    const handleConfirmDelete = () => {
        executeDelete().then(() => {
            executeGetCourses();
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
            field: 'duration',
            headerName: 'Duration',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'level',
            headerName: 'Level',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'rate',
            headerName: 'Rate',
            type: 'number',
        },
        {
            disableColumnMenu: true,
            field: 'category',
            headerName: 'Category',
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
                <Typography variant="h4" fontWeight={"bold"} fontFamily="var(--primary-font)">My Courses</Typography>
                <Button sx={{bgcolor: "#455CC7", color: "white"}} variant="contained" onClick={() => navigate("/app/moderatecourses/create")}>Create course</Button>
            </Stack>
            <DataGrid
                rows={courses}
                columns={columns}
                hideFooter
                sx={{ border: 0 }}
            />
            <Dialog open={open} onClose={handleCloseDialog}>
                <DialogTitle fontFamily={"var(--primary-font)"}>Delete course?</DialogTitle>
                <DialogContent>
                    <DialogContentText fontFamily={"var(--primary-font)"}>
                        Are you sure you want to delete this course? This action cannot be undone.
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

export default CourseModeratePage