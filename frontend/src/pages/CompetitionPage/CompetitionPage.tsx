import { Button, Card, Chip, Dialog, DialogActions, DialogContent, DialogTitle, IconButton, MenuItem, Select, SelectChangeEvent, Stack, Typography } from "@mui/material"
import { CompetitionType } from "../../types/CompetitionBanerType"
import { Gauge, gaugeClasses } from "@mui/x-charts";
import { DataGrid, GridActionsCellItem, GridColDef, GridRowId } from '@mui/x-data-grid';
import { useApi } from "../../hooks/useApi";
import { useEffect, useRef, useState } from "react";
import axios from "axios";
import { useParams } from "react-router";
import React from "react";
import { FullUserModel } from "../../types/UserProfileData";
import DeleteIcon from '@mui/icons-material/Delete';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import { useAtomValue } from "jotai";
import { userAtom } from "../../jotai/atoms";
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import CloudDownloadIcon from '@mui/icons-material/CloudDownload';

function CompetitionPage(){
    const params = useParams();
    const competitionId = params.id;

    const userData : FullUserModel = useAtomValue(userAtom);
    const fileInput = useRef<HTMLInputElement>(null);

    const [selectedStatus, setSelectedStatus] = React.useState('');
    const [openAddDialog, setOpenAddDialog] = React.useState(false);
    const [openDeleteDialog, setOpenDeleteDialog] = React.useState(false);
    const [selectedAddId, setSelectedAddId] = React.useState<GridRowId | null>(null);
    const [selectedDeleteId, setSelectedDeleteId] = React.useState<GridRowId | null>(null);

    const [users, setUsers] = useState<FullUserModel[]>([]);

    const {resData: RecivedCompetition, execute: executeCompetition} = useApi<CompetitionType>(async ()=>{ // get competition
      return axios.get('/api/competition/fulldata', {params: {competitionId: competitionId}});
    })

    const {resData: RecivedAddUsers, execute: executeGetAddUsers} = useApi<FullUserModel[]>(async ()=>{ // get add users
        return axios.get('/api/competition/myathletesoutofcompetition', {params: {competitionId: competitionId}});
    })

    const {resData: RecivedDeleteUsers, execute: executeGetDeleteUsers} = useApi<FullUserModel[]>(async ()=>{ // get delete users
        return axios.get('/api/competition/myathletesinthecompetition', {params: {competitionId: competitionId}});
    })

    const {execute: executeAddAthlete, statusCode: RecivedAddCode, setStatusCode} = useApi(async ()=>{ // add athlete
        return axios.post('/api/competition/addmember', null, {params: {competitionId: competitionId, userId: selectedAddId}});
    })

    const {execute: executeDeleteAthlete, statusCode: RecivedDeleteCode, setStatusCode: setStatusCodeDelete} = useApi(async ()=>{ // add athlete
        return axios.delete('/api/competition/deletemember', {params: {competitionId: competitionId, userId: selectedDeleteId}});
    })

    const {execute: executeUploadCompetitionResults, statusCode: RecivedUploadCode, setStatusCode: setStatusCodeUpload} = useApi<null, string>(async (body)=>{ 
        return axios.post('/api/competition/import', body, {params: {competitionId: competitionId}, headers:{"Content-Type":"application/json"}});
    })

    const {execute: executeStatusSet, statusCode: RecivedStatusCode, setStatusCode: setStatusCodeStatus} = useApi<null, string>(async (status)=>{ 
        return axios.put('/api/competition/status', null, {params: {competitionId: competitionId, newStatus: status}});
    })

    const {execute: executeDownloadResults} = useApi<Blob>(async ()=>{ 
        return axios.get('/api/competition/export',{params: {competitionId: competitionId}, responseType: 'blob', headers:{"Content-Type":"application/json"}});
    })

    const [competition, setCompetition] = useState<CompetitionType>();


    

    useEffect(()=>{
        executeCompetition();
    }, [])


    useEffect(()=>{
        if(RecivedStatusCode === 200){
            executeCompetition();
            setStatusCodeStatus(null);
        }
    }, [RecivedStatusCode])

    useEffect(()=>{
        if(RecivedUploadCode === 200){
            executeCompetition();
            setStatusCodeUpload(null);
        }
    }, [RecivedUploadCode])

    useEffect(()=>{
        if(RecivedCompetition){
            setCompetition(RecivedCompetition);
            setSelectedStatus(RecivedCompetition.status);
        }
    }, [RecivedCompetition])

    const columns: GridColDef[] = [
        { disableColumnMenu: true, field: 'id', headerName: 'ID', width: 70 },
        { field: 'fullName', headerName: 'Full name', width: 130 },
        {
            disableColumnMenu: true,
            field: 'age',
            headerName: 'Age',
            type: 'number',
        },
        {
            disableColumnMenu: true,
            field: 'country',
            headerName: 'Country',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'grade',
            headerName: 'Grade',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'result',
            headerName: 'Result',
            type: 'string',
            width: 110,
        },
    ];

    useEffect(() => {
        if (RecivedAddUsers) {
            setUsers(RecivedAddUsers);
            setSelectedAddId(null);
        }
    }, [RecivedAddUsers]);
    
    // Срабатывает при удалении пользователей
    useEffect(() => {
        if (RecivedDeleteUsers) { 
            setUsers(RecivedDeleteUsers); // Обновляем список пользователей
            setSelectedAddId(null); // Сбрасываем выбранный ID
        }
    }, [RecivedDeleteUsers]);
    
    // Вызывается при добавлении атлета в соревнование
    useEffect(() => {
        if (selectedAddId && competitionId) {
            executeAddAthlete();
            setSelectedAddId(null); // Сбрасываем ID после выполнения
        }
    }, [selectedAddId, competitionId]);
    
    // Вызывается при удалении атлета из соревнования
    useEffect(() => {
        if (selectedDeleteId && competitionId) {
            executeDeleteAthlete();
            setSelectedDeleteId(null); // Сбрасываем ID после выполнения
        }
    }, [selectedDeleteId, competitionId]);
    
    // После успешного добавления, обновляем данные
    useEffect(() => {
        if (RecivedAddCode === 200) {
            executeCompetition();
            executeGetAddUsers(); // Обновление пользователей, добавленных в соревнование
            setStatusCode(null);
        }
    }, [RecivedAddCode]);
    
    // После успешного удаления, обновляем данные
    useEffect(() => {
        if (RecivedDeleteCode === 200) {
            executeCompetition();
            executeGetDeleteUsers(); // Обновление пользователей, удаленных из соревнования
            setStatusCodeDelete(null);
        }
    }, [RecivedDeleteCode]);


    const handleOpenDeleteDialog = () => { // delete
        setOpenDeleteDialog(true);
        executeGetDeleteUsers();
    };

    const handleCloseDeleteDialog = () => {
        setOpenDeleteDialog(false);
        setSelectedDeleteId(null);
    };

    

    const handleOpenAddDialog = () => { // add
        setOpenAddDialog(true);
        executeGetAddUsers();
    };
    
    const handleCloseAddDialog = () => {
        setOpenAddDialog(false);
        setSelectedAddId(null);
    };
    
    const handleAdd = (id: GridRowId) => {
        setSelectedAddId(id);
    };

    const handleDelete = (id: GridRowId) => {
        setSelectedDeleteId(id);
    };

     const usersDeleteColumns: GridColDef[] = [
        { disableColumnMenu: true, field: 'id', headerName: 'ID', width: 70 },
        { field: 'firstName', headerName: 'First name', width: 130 },
        { field: 'secoundName', headerName: 'Last name', width: 130 },
        {
            disableColumnMenu: true,
            field: 'age',
            headerName: 'Age',
            type: 'number',
        },
        {
            disableColumnMenu: true,
            field: 'country',
            headerName: 'Country',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'grade',
            headerName: 'Grade',
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
                    onClick={() => handleDelete(id)}
                    color="inherit"
                />,
            ],
        }
    ];

     const usersAddColumns: GridColDef[] = [
        { disableColumnMenu: true, field: 'id', headerName: 'ID', width: 70 },
        { field: 'firstName', headerName: 'First name', width: 130 },
        { field: 'secoundName', headerName: 'Last name', width: 130 },
        {
            disableColumnMenu: true,
            field: 'age',
            headerName: 'Age',
            type: 'number',
        },
        {
            disableColumnMenu: true,
            field: 'country',
            headerName: 'Country',
            type: 'string',
        },
        {
            disableColumnMenu: true,
            field: 'grade',
            headerName: 'Grade',
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
                    icon={<AddCircleIcon />}
                    label="Add"
                    onClick={() => handleAdd(id)}
                    color="inherit"
                />,
            ],
        }
    ];

    

    function handleDownload(): void {
    executeDownloadResults()
        .then((response) => {
            if (!response) return;

            const blob = response.data;
            const url = window.URL.createObjectURL(blob);

            const a = document.createElement('a');
            a.href = url;
            a.download = 'competition_members.json';
            document.body.appendChild(a);
            a.click();

            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        })
        .catch((error) => {
            console.error('Download failed', error);
        });
    }

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            file.text().then((text) => {
                executeUploadCompetitionResults(text);
                if (fileInput.current) {
                    fileInput.current.value = '';
                }
            })
            
        }
    };

    function handleStatusChange(event: SelectChangeEvent<string>): void {
        setSelectedStatus(event.target.value);
        executeStatusSet(event.target.value);
    }

    return (
        <Stack gap={"12px"} fontFamily={"var(--primary-font)"}>
            <Stack flexDirection={"row"} justifyContent={"space-between"} alignItems={"center"}>
                <Typography variant="h4" fontWeight="bold" fontFamily="inherit">{competition?.title}</Typography>
                <Stack alignItems={"center"}>
                    <Gauge
                        width={100} height={70}
                        value={competition?.memberCount}
                        valueMax={competition?.maxMemberCount}
                        startAngle={-110}
                        endAngle={110}
                        sx={{
                            [`& .${gaugeClasses.valueText}`]: {
                            fontFamily: 'var(--primary-font)',
                            fontSize: 15,
                            fontWeight: 'bold',
                            transform: 'translate(0px, 0px)',
                            },
                        }}
                        text={
                            ({ value, valueMax }) => `${value} / ${valueMax}`
                        }
                    />
                    <Typography variant="body2" fontFamily="inherit" fontWeight={"500"}>Member count</Typography>
                </Stack>
            </Stack>
            <Stack flexDirection={"row"} alignItems={"center"} gap={"15px"}>
                <Typography variant="body1" fontFamily="inherit" fontWeight={"bold"}>Status: </Typography>
                {competition?.status === "Active" && <Chip sx={{width:"100px", fontFamily: "var(--primary-font)"}} label="Active" color="success" />}
                {competition?.status === "Pending" && <Chip sx={{width:"100px", fontFamily: "var(--primary-font)"}} label="didn't start" color="warning" />}
                {competition?.status === "Ended" && <Chip sx={{width:"100px", fontFamily: "var(--primary-font)"}} label="Ended" color="error" />}
            </Stack>
            
            <Typography variant="body1" fontFamily="inherit"><span style={{ fontWeight: "bold" }}>Date:</span> {competition?.date}</Typography>
            
            <Typography variant="body1" fontFamily="inherit"><span style={{ fontWeight: "bold" }}>Time:</span> {competition?.time}</Typography>

            <Typography fontFamily="inherit" variant="body1">
                <span style={{ fontWeight: "bold" }}>Organiser:</span> {competition?.organiser}
            </Typography>

            <Typography fontFamily="inherit" variant="body1"><span style={{ fontWeight: "bold" }}>Address:</span> {competition?.venue}, {competition?.city}, {competition?.country}</Typography>

            <Typography fontFamily="inherit" variant="body1"><span style={{ fontWeight: "bold" }}>Exercises:</span> {competition?.exercise}</Typography>

            <Stack gap={"12px"} mt={"20px"}>
                <Stack flexDirection={"row"} justifyContent={"space-between"}>
                    <Typography variant="h5" fontWeight={"bold"} fontFamily="inherit">Members</Typography>
                    <Stack flexDirection={"row"}>                    
                        {
                            userData.role === "coach" &&
                            <Stack flexDirection={"row"} gap={"12px"}>
                                <Button sx={{bgcolor: "#455CC7", fontFamily: "var(--primary-font)", width: "200px"}} variant="contained" onClick={handleOpenAddDialog} >Add users</Button>
                                <Button sx={{fontFamily: "var(--primary-font)", width: "200px"}} variant="contained"  color="error" onClick={handleOpenDeleteDialog}>Delete users</Button>
                            </Stack>
                        }
                        {
                            userData.role === "organization" &&
                            <Stack>
                                <Stack>
                                    <Typography variant="body1" fontFamily="inherit" fontWeight={"bold"}>Status:</Typography>
                                    <Select
                                    labelId="label"
                                    id="select"
                                    size="small"
                                    onChange={handleStatusChange}
                                    value={selectedStatus}
                                    sx={{ width: 820 }}
                                >
                                    <MenuItem value="Pending">
                                        Didn't start
                                    </MenuItem>
                                    <MenuItem value="Active">
                                        Active
                                    </MenuItem>
                                    <MenuItem value="Ended">
                                        Ended
                                    </MenuItem>
                                </Select>
                                </Stack>

                                <Stack flexDirection={"row"} gap={"12px"}>
                                <input onChange={handleFileChange} style={{ display: "none" }} accept="application/json" type="file" ref={fileInput}></input>
                                <IconButton onClick={()=> {fileInput.current?.click()}}>
                                    <CloudUploadIcon/>
                                </IconButton>
                            </Stack>
                            </Stack>
                        }
                        <IconButton onClick={handleDownload}>
                            <CloudDownloadIcon/>
                        </IconButton>
                        
                    </Stack>
                </Stack>
                <Card variant="outlined" sx={{ width: '100%' }}>
                    <DataGrid
                        rows={competition?.members}
                        columns={columns}
                        rowSelection={false}
                        hideFooter
                        disableColumnSelector
                        sx={{ border: 0 }}
                    />
                </Card>
            </Stack>

            <Dialog maxWidth="lg" 
             fullWidth open={openAddDialog} onClose={handleCloseAddDialog}>
                <DialogTitle fontFamily={"var(--primary-font)"}>Add user to course</DialogTitle>
                <DialogContent>
                    <DataGrid
                        rows={users}
                        columns={usersAddColumns}
                        sx={{ border: 0 }}
                    />
                </DialogContent>
               <DialogActions>
                    <Button sx={{borderColor: "#455CC7", color: "#455CC7"}} variant="outlined" onClick={handleCloseAddDialog}>Cancel</Button>
                </DialogActions>
            </Dialog>


            <Dialog maxWidth="lg" 
             fullWidth open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
                <DialogTitle fontFamily={"var(--primary-font)"}>Delete user from course</DialogTitle>
                <DialogContent>
                    <DataGrid
                        rows={users}
                        columns={usersDeleteColumns}
                        sx={{ border: 0 }}
                    />
                </DialogContent>
               <DialogActions>
                    <Button sx={{borderColor: "#455CC7", color: "#455CC7"}} variant="outlined" onClick={handleCloseDeleteDialog}>Cancel</Button>
                </DialogActions>
            </Dialog>
        </Stack>
    )
}

export default CompetitionPage