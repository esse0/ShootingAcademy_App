import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField, Typography } from '@mui/material';
import { useAtomValue } from 'jotai';
import { userAtom } from '../../jotai/atoms';
import { useEffect, useState } from 'react';
import GroupType from '../../types/GroupType';
import GroupBanner from '../../components/GroupBanner/GroupBanner';
import { FullUserModel } from '../../types/UserProfileData';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';

function GroupsPage() {
    const userData: FullUserModel = useAtomValue(userAtom);

    const [groups, setGroups] = useState<GroupType[]>([]);
    const [open, setOpen] = useState(false);
    const [groupName, setGroupName] = useState("");

    const { resData: RecivedGroups, execute: executeUserGetGroups } = useApi<GroupType[]>(async () => {
        return axios.get('/api/Group/user');
    });

    const { resData: RecivedCoachGroups, execute: executeCoachGetGroups } = useApi<GroupType[]>(async () => {
        return axios.get('/api/Group/coach');
    });

    const {execute: executeCreateGroup, statusCode: RecivedCreateCode, setStatusCode} = useApi<null, GroupType>(
        async (body)=>{
            return axios.post('/api/Group/create', body);
        }
    );

    const getData = () => {
        if (userData.role === 'coach') executeCoachGetGroups();
        else executeUserGetGroups();
    };

    useEffect(() => {
        if (RecivedGroups) setGroups(RecivedGroups);
        if (RecivedCoachGroups) setGroups(RecivedCoachGroups);
    }, [RecivedGroups, RecivedCoachGroups]);

    useEffect(()=>{
        if(userData.id) getData();
    }, [userData])

    useEffect(()=>{
        if(RecivedCreateCode === 201) {
            getData();
            setStatusCode(null);
        }
    }, [RecivedCreateCode])

    const handleAddOpen = () => setOpen(true);

    const handleAddClose = () => {
        setOpen(false);
        setGroupName('');
    };

    const handleAddSubmit = () => {
        executeCreateGroup({
            organisationName: groupName,
        });

        handleAddClose();
    };

    return (
       <Stack gap={"20px"}>
            <Stack flexDirection={"row"}>
                <Typography variant="h4" fontFamily={"inherit"} fontWeight={"bold"}>Groups</Typography> 
            
                {
                    userData.role === "coach" &&
                    <>
                        <Button sx={{ml: "auto", width:"200px", bgcolor: "var(--accent-color)"}} variant="contained" color="primary" onClick={handleAddOpen}>
                            Create group
                        </Button>

                        <Dialog open={open} onClose={handleAddClose}>
                            <DialogTitle>Create new group</DialogTitle>
                            <DialogContent>
                                <Stack spacing={2} sx={{ mt: 1 }}>
                                    <TextField
                                        label="Organisation Name"
                                        variant="outlined"
                                        fullWidth
                                        value={groupName}
                                        onChange={(e) => setGroupName(e.target.value)}
                                    />
                                </Stack>
                            </DialogContent>
                            <DialogActions>
                                <Button sx={{borderColor: "var(--accent-color)", color: "var(--accent-color)"}} onClick={handleAddClose} color="primary">Cancel</Button>
                                <Button sx={{borderColor: "var(--accent-color)", color: "var(--accent-color)"}} onClick={handleAddSubmit} color="primary" disabled={!groupName}>Create</Button>
                            </DialogActions>
                        </Dialog>
                    </>
                }
            </Stack>
            {
                groups.length === 0  && <Typography textAlign={"center"} variant="h5" fontFamily={"inherit"} fontWeight={"bold"} color="text.secondary">There are no groups</Typography>
            }
            {
                groups.map((group: GroupType, index) => {
                    return (
                        <GroupBanner key={index} group={group} listUpdate={getData}/>
                    );
                })
            }
       </Stack>
    );
}

export default GroupsPage;
