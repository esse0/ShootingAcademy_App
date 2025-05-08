import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Paper, Stack, Typography } from "@mui/material"
import GroupType from "../../types/GroupType";
import { Link } from "react-router";
import { useAtomValue } from "jotai";
import { userAtom } from "../../jotai/atoms";
import { FullUserModel } from "../../types/UserProfileData";
import axios from "axios";
import { useApi } from "../../hooks/useApi";
import React, { useEffect } from "react";


function GroupBanner({group, listUpdate}: {group: GroupType, listUpdate: () => void}){
    const [open, setOpen] = React.useState(false);

    const userData : FullUserModel = useAtomValue(userAtom);

    const {execute: executeDeleteGroup, statusCode: RecivedDeleteCode, setStatusCode} = useApi(
        async ()=>{
            return axios.delete('/api/Group/delete', {params: {groupId: group.id}});
        }
    );

    const handleOpenDialog = () => {
        setOpen(true);
    };
    
    const handleCloseDialog = () => {
        setOpen(false);
    };
    
    const handleConfirmDelete = () => {  
        executeDeleteGroup();
        handleCloseDialog();
    };

    useEffect(() => {
        if (RecivedDeleteCode === 200) {
           listUpdate();
           setStatusCode(null);
        }
    },[RecivedDeleteCode])

    return(
        <Paper elevation={0} sx={{bgcolor:"#F9F9F9", fontFamily: "var(--primary-font)", width:"100%", height:"170px", borderRadius: '0.5em'}}>
            <Stack height={"100%"} p={'25px'} justifyContent={"space-between"}>
                <Typography variant="h5" fontFamily={"inherit"} fontWeight={"bold"}>{group.organisationName}</Typography>
                <Stack flexDirection={"row"} gap={"5px"}>  
                    <Typography variant="body1" fontFamily={"inherit"} fontWeight={"bold"}>Coach: </Typography>
                    <Typography variant="body1" fontFamily={"inherit"}>{group.coach?.firstName} {group.coach?.secoundName} {group.coach?.patronymicName} </Typography>
                </Stack>
                <Stack flexDirection={"row"} gap={"10px"} justifyContent={"flex-end"}>
                        <Button sx={{bgcolor: "#455CC7"}} variant="contained" to={`/app/mygroup/${group.id}`} component={Link}>See Group</Button>
                        {userData.role === "coach" && <Button variant="outlined" color="error" onClick={handleOpenDialog}>Delete group</Button>}
                </Stack>
            </Stack>
            <Dialog open={open} onClose={handleCloseDialog}>
                <DialogTitle fontFamily={"var(--primary-font)"}>Delete group</DialogTitle>
                <DialogContent>
                    <DialogContentText fontFamily={"var(--primary-font)"}>
                        Are you sure you want to delete this group? This action cannot be undone.
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button sx={{borderColor: "#455CC7", color: "#455CC7"}} variant="outlined" onClick={handleCloseDialog}>Cancel</Button>
                    <Button  variant="contained" onClick={handleConfirmDelete} color="error">Delete</Button>
                </DialogActions>
            </Dialog>
        </Paper>
    )
}

export default GroupBanner