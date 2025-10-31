import { Button, Card, Dialog, DialogActions, DialogContent, DialogTitle, Stack, Typography } from "@mui/material"
import { DataGrid, GridActionsCellItem, GridColDef, GridRowId, GridColType } from "@mui/x-data-grid";
import { useEffect, useState } from "react";
import axios from "axios";
import { useApi } from "../../hooks/useApi";
import { useAtomValue } from "jotai";
import { userAtom } from "../../jotai/atoms";

import { OrganizationType } from "../../types/OrganizationType";
import AddCircleIcon from '@mui/icons-material/AddCircle';
import { useSnackbar } from "notistack";
import { TEXTS } from '../../constants/texts';
import { useWebSocketContext } from "../../hooks/WebSocketContext";
import CancelIcon from '@mui/icons-material/Cancel';
import DeleteIcon from '@mui/icons-material/Delete';
import { UserWithStatus } from "../../types/UserWithStatusType";

function OrganizationMembersPage() {
  const { enqueueSnackbar } = useSnackbar();
  const [openAdd, setOpenAdd] = useState(false);
  const [organization, setOrganization] = useState<OrganizationType | null>(null);
  const [users, setUsers] = useState<UserWithStatus[]>([]);
  const ws = useWebSocketContext();
  const userData = useAtomValue(userAtom);

  const { resData: fetchedOrganization, execute: executeGetOrganization } = useApi<OrganizationType>(async () => {
    return axios.get('/api/user/organization');
  });

  const { resData: fetchedUsers, execute: executeGetUninvitedUsers } = useApi<UserWithStatus[]>(async () => {
    return axios.get('/api/organization/uninvitedusers');
  });

  const { execute: executeRejectCoach } = useApi(async (params?: { organizationId: string, userId: string }) => {
    if (!params) throw new Error('Нет параметров для удаления');
    return axios.put('/api/organization/member/reject', null, {
      params
    });
  });

  useEffect(() => {
    executeGetOrganization();
  }, []);

  useEffect(() => {
    if (fetchedOrganization) {
      setOrganization(fetchedOrganization);
    }
  }, [fetchedOrganization]);

  useEffect(() => {
    if (fetchedUsers) {
      setUsers(fetchedUsers);
    }
  }, [fetchedUsers]);

  useEffect(() => {
    if (ws?.setOnInviteChanged) {
      ws.setOnInviteChanged(() => {
        executeGetOrganization();
        executeGetUninvitedUsers();
      });
    }
  }, [ws, executeGetOrganization, executeGetUninvitedUsers]);

  const handleOpenAddDialog = () => {
    setOpenAdd(true);
    executeGetUninvitedUsers();
  };

  const handleCloseAddDialog = () => {
    setOpenAdd(false);
  };

  const handleInvite = (id: GridRowId) => {
    if (organization?.id && ws?.inviteToOrganization) {
      ws.inviteToOrganization(organization.id, id.toString(), TEXTS.ORGANIZATION_INVITATION);
    }
  };

  const handleCancelInvite = (id: GridRowId) => {
    if (organization?.id && ws?.cancelOrganizationInvitation) {
      ws.cancelOrganizationInvitation(organization.id, id.toString());
    }
  };

  const handleDelete = async (id: GridRowId) => {
    if (organization?.id) {
      try {
        await executeRejectCoach({ organizationId: organization.id, userId: id.toString() });
        enqueueSnackbar(TEXTS.USER_REMOVED, { variant: 'success' });
        executeGetOrganization();
        executeGetUninvitedUsers();
      } catch (error) {
        enqueueSnackbar(TEXTS.ERROR_REMOVING_USER, { variant: 'error' });
      }
    }
  };

  const transformedUsers = users.map(user => ({
    id: user.user.id,
    firstName: user.user.firstName,
    secoundName: user.user.secoundName,
    email: user.user.email,
    grade: user.user.grade,
    status: user.status,
    user: user.user // для действий
  }));

  const usersColumns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'firstName', headerName: TEXTS.FIRST_NAME, width: 130 },
    { field: 'secoundName', headerName: TEXTS.LAST_NAME, width: 130 },
    { field: 'email', headerName: TEXTS.EMAIL, width: 200 },
    { field: 'grade', headerName: TEXTS.GRADE, width: 130 },
    ...(userData.role !== "coach" ? [{
      field: 'actions',
      type: 'actions' as GridColType,
      headerName: TEXTS.ACTIONS,
      width: 150,
      getActions: ({ row }: any) => [
        row.status === 'Pending' ? (
          <GridActionsCellItem
            icon={<CancelIcon />}
            label={TEXTS.CANCEL}
            onClick={() => handleCancelInvite(row.id)}
            color="error"
          />
        ) : (
          <GridActionsCellItem
            icon={<AddCircleIcon />}
            label={TEXTS.INVITE}
            onClick={() => handleInvite(row.id)}
            color="inherit"
          />
        ),
      ],
    }] : [])
  ];

  const columns: GridColDef[] = [
    { field: 'userId', headerName: 'ID', width: 70 },
    { field: 'userName', headerName: TEXTS.USERNAME, width: 200 },
    { field: 'role', headerName: TEXTS.ROLE, width: 130 },
    { field: 'joinedAt', headerName: TEXTS.JOIN_DATE, width: 180 },
    ...(userData.role !== "coach" ? [{
      field: 'actions',
      type: 'actions' as GridColType,
      headerName: TEXTS.ACTIONS,
      width: 100,
      getActions: ({ id, row }: any) =>
        row.role === 'owner'
          ? []
          : [
              <GridActionsCellItem
                icon={<DeleteIcon />}
                label={TEXTS.DELETE}
                onClick={() => handleDelete(id)}
                color="error"
              />
            ],
    }] : [])
  ];

  return (
    <Stack gap={2}>
      <Stack flexDirection="row" justifyContent="space-between" alignItems="center">
        <Typography variant="h4" fontFamily="var(--primary-font)">
          {TEXTS.ORGANIZATION_MEMBERS}
        </Typography>
        {
          userData.role !== "coach" && (<Button
            variant="contained"
            onClick={handleOpenAddDialog}
            sx={{ bgcolor: "var(--accent-color)" }}
          >
            {TEXTS.INVITE_USERS}
          </Button>)
        }
      </Stack>

      <Card variant="outlined">
        <DataGrid
          rows={(organization?.members || []).filter(m => m.status === 'Approved')}
          columns={columns}
          getRowId={(row) => row.userId}
          hideFooter
          sx={{ border: 0 }}
        />
      </Card>

      <Dialog
        maxWidth="lg"
        fullWidth
        open={openAdd}
        onClose={handleCloseAddDialog}
      >
        <DialogTitle fontFamily="var(--primary-font)">
          {TEXTS.INVITE_USERS}
        </DialogTitle>
        <DialogContent>
          <DataGrid
            rows={transformedUsers}
            columns={usersColumns}
            getRowId={(row) => row.id}
            sx={{ border: 0 }}
          />
        </DialogContent>
        <DialogActions>
          <Button
            variant="outlined"
            onClick={handleCloseAddDialog}
            sx={{ borderColor: "var(--accent-color)", color: "var(--accent-color)" }}
          >
            {TEXTS.CLOSE}
          </Button>
        </DialogActions>
      </Dialog>
    </Stack>
  );
}

export default OrganizationMembersPage;