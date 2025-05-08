import { Avatar, Button, Card, CardContent, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Stack, Typography } from "@mui/material"
import GroupType from "../../types/GroupType";
import { DataGrid, GridActionsCellItem, GridColDef, GridRowId } from "@mui/x-data-grid";
import React, { useEffect, useMemo, useState } from "react";
import { useAtomValue } from "jotai";
import { userAtom } from "../../jotai/atoms";
import axios from "axios";
import { useApi } from "../../hooks/useApi";
import { FullUserModel } from "../../types/UserProfileData";
import { useParams } from "react-router";
import DeleteIcon from '@mui/icons-material/Delete';
import AddCircleIcon from '@mui/icons-material/AddCircle';


function MyGroupPage() {
    const params = useParams();
    const groupId = params.id;
    const userData : FullUserModel = useAtomValue(userAtom);

    const [openDelete, setOpenDelete] = React.useState(false);
    const [openAdd, setOpenAdd] = React.useState(false);

    const [selectedId, setSelectedId] = React.useState<GridRowId | null>(null);
    const [selectedAddId, setSelectedAddId] = React.useState<GridRowId | null>(null);

    const [group, setGroup] = useState<GroupType>({
        id: "00000000-0000-0000-0000-000000000000",
        organisationName: "",
        coach: {
            id: "00000000-0000-0000-0000-000000000000",
            firstName: "",
            secoundName: "",
            patronymicName: "",
            age: 0,
            grade: "",
            country: "",
            city: "",
            address: "",
            email: "",
            role: "",
        },
        members: []
    });
    const [users, setUsers] = useState<FullUserModel[]>([]);

    const {resData: RecivedUserGroups, execute: executeUserGetGroup} = useApi<GroupType>(async ()=>{
        return axios.get('/api/group/user/fullData', {params: {groupId: groupId}});
    })

    const {resData: RecivedCoachGroups, execute: executeCoachGetGroup} = useApi<GroupType>(async ()=>{
        return axios.get('/api/group/coach/fullData', {params: {groupId: groupId}});
    })

    const {resData: RecivedAthletes, execute: executeGetUnsubscribedAthletes} = useApi<FullUserModel[]>(async ()=>{
        return axios.get('/api/group/unsubscribedathletes', {params: {groupId: groupId}});
    })

    const {execute: executeAddAthlete, statusCode: RecivedAddCode, setStatusCode} = useApi(async ()=>{
        return axios.post('/api/group/addmember', null, {params: {groupId: groupId, userId: selectedAddId}});
    })

    const {execute: executeDeleteMember, statusCode: RecivedDeleteCode} = useApi(async ()=>{
        return axios.post('/api/group/kickmember', null, {params: {groupId: groupId, userId: selectedId}});
    })

    const getData = () =>{
        if(userData.role === "coach") executeCoachGetGroup();
        else executeUserGetGroup()
    }

    useEffect(()=>{
        if(!userData.id) return;
        getData();
        
    }, [userData])
   
    useEffect(()=>{
        if(RecivedUserGroups) setGroup(RecivedUserGroups);
        if(RecivedCoachGroups) setGroup(RecivedCoachGroups);
    }, [RecivedUserGroups, RecivedCoachGroups])

    useEffect(()=>{
        if(RecivedAthletes){ 
            setUsers(RecivedAthletes);
            setSelectedAddId(null);
        }
    }, [RecivedAthletes])

    useEffect(()=>{
        if(RecivedAddCode == 201){
            getData();
            executeGetUnsubscribedAthletes();
            setStatusCode(null);
        }
    }, [RecivedAddCode])

    useEffect(()=>{
        if(RecivedAddCode == 201){ 
            executeGetUnsubscribedAthletes();
            setSelectedAddId(null);
            setStatusCode(null);
        }
    }, [RecivedAthletes])

    useEffect(() => {
        if (selectedAddId && groupId) {
            executeAddAthlete();
            setSelectedAddId(null);
        }
    }, [selectedAddId, groupId]);

    useEffect(() => {
        if(RecivedDeleteCode === 200){
            getData();
            setStatusCode(null);
        }
        
    }, [RecivedDeleteCode]);


    const handleOpenDeleteDialog = (id: GridRowId) => {
        setSelectedId(id);
        setOpenDelete(true);
    };

    const handleCloseDeleteDialog = () => {
        setOpenDelete(false);
        setSelectedId(null);
    };

    const handleConfirmDelete = () => {
        executeDeleteMember().then(() => {
            getData();
        });
        handleCloseDeleteDialog();
        setStatusCode(null);
    };

    const handleOpenAddDialog = () => {
        setOpenAdd(true);
        executeGetUnsubscribedAthletes();
    };

    const handleCloseAddDialog = () => {
        setOpenAdd(false);
        setSelectedAddId(null);
    };

    const handleAdd = (id: GridRowId) => {
        setSelectedAddId(id);
    };

    const usersColumns: GridColDef[] = [
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

    const columns = useMemo(() => {
        const baseColumns: GridColDef[] = [
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
        ];

        if (userData.id && userData.role == "coach") {
            baseColumns.push({
                field: 'actions',
                type: 'actions',
                headerName: 'Actions',
                width: 100,
                cellClassName: 'actions',
                getActions: ({ id }) => [
                    <GridActionsCellItem
                        icon={<DeleteIcon />}
                        label="Delete"
                        onClick={() => handleOpenDeleteDialog(id)}
                        color="inherit"
                    />,
                ],
            });
        }
    
        return baseColumns;
    }, [userData.role]); 

    return (
        <Stack>
            { 
                group != undefined ? 
                <Stack gap={"20px"}>
                <Typography fontFamily={"var(--primary-font)"} variant="h4">{group?.organisationName}</Typography>
                <Card variant="outlined">
                    <CardContent sx={{display:"flex", flexDirection:"column", gap:"15px", '&:last-child': { pb: "16px" }}}>
                        <Stack flexDirection={"row"} gap={"20px"} alignItems={"center"}>
                            <Avatar
                                alt="Coach"
                                sx={{ width: 200, height: 200 }}
                            />

                            <Stack gap={"5px"}>
                                <Typography fontFamily={"var(--primary-font)"} variant="body1"><span style={{fontWeight: "bold"}}>Coach name: </span>{group?.coach?.firstName} {group?.coach?.secoundName} {group?.coach?.patronymicName}</Typography>
                                <Typography fontFamily={"var(--primary-font)"} variant="body1"><span style={{fontWeight: "bold"}}>Email: </span>{group?.coach?.email}</Typography>
                                <Typography fontFamily={"var(--primary-font)"} variant="body1"><span style={{fontWeight: "bold"}}>Grade: </span>{group?.coach?.grade}</Typography>
                                <Typography fontFamily={"var(--primary-font)"} variant="body1"><span style={{fontWeight: "bold"}}>Age: </span>{group?.coach?.age}</Typography>
                                <Typography fontFamily={"var(--primary-font)"} variant="body1"><span style={{fontWeight: "bold"}}>Country: </span>{group?.coach?.country}</Typography>
                            </Stack>
                        </Stack>
                    </CardContent>
                </Card> 
                
                <Stack flexDirection={"row"} justifyContent={"space-between"}>
                    <Typography fontFamily={"var(--primary-font)"} variant="h6">Members</Typography>
                    {userData.id && userData.role == "coach" && <Button sx={{bgcolor: "#455CC7"}} variant="contained" onClick={handleOpenAddDialog}>Add user</Button>}
                </Stack>

                <Card variant="outlined">
                <DataGrid
                    rows={group.members}
                    columns={columns}
                    hideFooter
                    sx={{ border: 0 }}
                />
                </Card>

                </Stack>
                : <Typography color="text.secondary" textAlign={"center"} fontFamily={"var(--primary-font)"} variant="h4">No groups</Typography>
            }   

            <Dialog  open={openDelete} onClose={handleCloseDeleteDialog}>
                <DialogTitle fontFamily={"var(--primary-font)"}>Kick user?</DialogTitle>
                <DialogContent>
                    <DialogContentText fontFamily={"var(--primary-font)"}>
                        Are you sure you want to delete this user? This action cannot be undone.
                    </DialogContentText>
                </DialogContent>
               <DialogActions>
                    <Button sx={{borderColor: "#455CC7", color: "#455CC7"}} variant="outlined" onClick={handleCloseDeleteDialog}>Cancel</Button>
                    <Button  variant="contained" onClick={handleConfirmDelete} color="error">Delete</Button>
                </DialogActions>
            </Dialog>

            <Dialog maxWidth="lg" 
            fullWidth open={openAdd} onClose={handleCloseAddDialog}>
                <DialogTitle fontFamily={"var(--primary-font)"}>Add user</DialogTitle>
                <DialogContent>
                    <DataGrid
                        rows={users}
                        columns={usersColumns}
                        sx={{ border: 0 }}
                    />
                </DialogContent>
               <DialogActions>
                    <Button sx={{borderColor: "#455CC7", color: "#455CC7"}} variant="outlined" onClick={handleCloseAddDialog}>Cancel</Button>
                </DialogActions>
            </Dialog>
        </Stack>
    )
  }

export default MyGroupPage;