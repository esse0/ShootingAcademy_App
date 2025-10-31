import { useEffect, useState } from "react";
import { Button, Card, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField, Typography, MenuItem, Select, FormControl, InputLabel } from "@mui/material";
import { DataGrid, GridActionsCellItem, GridColDef, GridRowId, GridColType } from "@mui/x-data-grid";
import { useAtomValue } from "jotai";
import { userAtom } from "../../jotai/atoms";
import axios from "axios";
import { useSnackbar } from "notistack";
import { useApi } from "../../hooks/useApi";
import { RangeDTO, CreateRangeDTO, UpdateRangeDTO } from "../../types/RangeType";
import { OrganizationType } from "../../types/OrganizationType";
import { TEXTS } from "../../constants/texts";
import { useForm, Controller } from "react-hook-form";

const RANGE_TYPES = [
  { value: 'Indoor', label: 'Indoor' },
  { value: 'Outdoor', label: 'Outdoor' },
  { value: 'Virtual', label: 'Virtual' },
  { value: 'Other', label: 'Other' }
] as const;

function OrganizationRangesPage() {
  const userData = useAtomValue(userAtom);
  const { enqueueSnackbar } = useSnackbar();
  const { register, handleSubmit, reset, formState: { errors }, control } = useForm<CreateRangeDTO>({
    mode: "onChange"
  });

  const [openAdd, setOpenAdd] = useState(false);
  const [openEdit, setOpenEdit] = useState(false);
  const [selectedRange, setSelectedRange] = useState<RangeDTO | null>(null);

  // Получение организации
  const { resData: organization, execute: executeGetOrganization } = useApi<OrganizationType>(() => {
    return axios.get('/api/user/organization');
  });

  // Получение помещений
  const { resData: ranges, execute: executeGetRanges } = useApi<RangeDTO[]>(() => {
    if (!organization?.id) throw new Error('Нет ID организации');
    return axios.get(`/api/range/organization/${organization.id}`);
  });

  // Добавление помещения
  const { execute: executeCreateRange } = useApi<RangeDTO, CreateRangeDTO>((data) => {
    return axios.post('/api/range', data);
  });

  // Обновление помещения
  const { execute: executeUpdateRange } = useApi<RangeDTO, { id: string, updateDto: UpdateRangeDTO }>((data) => {
    if (!data) throw new Error('No data for update');
    return axios.put(`/api/range/${data.id}`, data.updateDto);
  });

  // Удаление помещения
  const { execute: executeDeleteRange } = useApi<void, string>((id) => {
    if (!id) throw new Error('No ID for deletion');
    return axios.delete(`/api/range/${id}`);
  });

  useEffect(() => {
    executeGetOrganization();
  }, []);

  useEffect(() => {
    if (organization?.id) {
      executeGetRanges();
    }
  }, [organization]);

  const handleOpenAddDialog = () => {
    reset({
      location: undefined,
      description: undefined,
      capacity: undefined,
      type: undefined
    });
    setOpenAdd(true);
  };

  const handleCloseAddDialog = () => {
    setOpenAdd(false);
    reset();
  };

  const handleOpenEditDialog = (range: RangeDTO) => {
    setSelectedRange(range);
    reset({
      location: range.location,
      description: range.description,
      capacity: range.capacity,
      type: range.type,
    });
    setOpenEdit(true);
  };

  const handleCloseEditDialog = () => {
    setOpenEdit(false);
    setSelectedRange(null);
    reset();
  };

  const onSubmit = async (data: CreateRangeDTO) => {
    if (!organization?.id) return;
    
    try {
      await executeCreateRange({
        ...data,
        organizationId: organization.id
      });
      enqueueSnackbar(TEXTS.RANGE_ADDED, { variant: "success" });
      handleCloseAddDialog();
      executeGetRanges();
    } catch (error) {
      enqueueSnackbar(TEXTS.ERROR_ADDING_RANGE, { variant: "error" });
    }
  };

  const onEdit = async (data: CreateRangeDTO) => {
    if (!selectedRange) return;

    try {
      await executeUpdateRange({
        id: selectedRange.id,
        updateDto: {
          location: data.location,
          description: data.description,
          capacity: data.capacity,
          type: data.type
        }
      });
      enqueueSnackbar(TEXTS.RANGE_UPDATED, { variant: "success" });
      handleCloseEditDialog();
      executeGetRanges();
    } catch (error) {
      enqueueSnackbar(TEXTS.ERROR_UPDATING_RANGE, { variant: "error" });
    }
  };

  const handleDelete = async (id: GridRowId) => {
    try {
      await executeDeleteRange(id.toString());
      enqueueSnackbar(TEXTS.RANGE_DELETED, { variant: "success" });
      executeGetRanges();
    } catch (error) {
      enqueueSnackbar(TEXTS.ERROR_DELETING_RANGE, { variant: "error" });
    }
  };

  const columns: GridColDef[] = [
    { field: 'location', headerName: TEXTS.LOCATION, width: 200 },
    { field: 'description', headerName: TEXTS.DESCRIPTION, width: 250 },
    { field: 'capacity', headerName: TEXTS.CAPACITY, width: 120 },
    { field: 'type', headerName: TEXTS.TYPE, width: 120},
    ...(userData.role !== "coach" ? [{
      field: 'actions',
      type: 'actions' as GridColType,
      headerName: TEXTS.ACTIONS,
      width: 120,
      getActions: ({ row }: { row: RangeDTO }) => [
        <GridActionsCellItem
          icon={<span style={{ color: '#455CC7', fontWeight: 700 }}>✎</span>}
          label={TEXTS.EDIT}
          onClick={() => handleOpenEditDialog(row)}
        />,
        <GridActionsCellItem
          icon={<span style={{ color: 'red', fontWeight: 700 }}>✖</span>}
          label={TEXTS.DELETE}
          onClick={() => handleDelete(row.id)}
        />
      ]
    }] : [])
  ];

  return (
    <Stack gap={2}>
      <Stack flexDirection="row" justifyContent="space-between" alignItems="center">
        <Typography variant="h4" fontFamily="var(--primary-font)">
          {TEXTS.ORGANIZATION_RANGES}
        </Typography>
        {userData.role !== "coach" && (
          <Button 
            variant="contained" 
            onClick={handleOpenAddDialog} 
            sx={{ bgcolor: "var(--accent-color)" }}
          >
            {TEXTS.ADD_RANGE}
          </Button>
        )}
      </Stack>

      <Card variant="outlined">
        <DataGrid
          rows={ranges || []}
          columns={columns}
          getRowId={(row) => row.id}
          hideFooter
          sx={{ border: 0 }}
        />
      </Card>

      {/* Add Dialog */}
      <Dialog open={openAdd} onClose={handleCloseAddDialog}>
        <DialogTitle>{TEXTS.ADD_RANGE}</DialogTitle>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogContent>
            <Stack gap={2} sx={{ mt: 1 }}>
              <TextField 
                label={TEXTS.LOCATION}
                {...register('location', { required: TEXTS.REQUIRED_FIELD })}
                error={!!errors.location}
                helperText={errors.location?.message}
                fullWidth 
              />
              <TextField 
                label={TEXTS.DESCRIPTION}
                {...register('description', { required: TEXTS.REQUIRED_FIELD })}
                error={!!errors.description}
                helperText={errors.description?.message}
                fullWidth 
              />
              <TextField 
                label={TEXTS.CAPACITY}
                type="number"
                {...register('capacity', { 
                  required: TEXTS.REQUIRED_FIELD,
                  min: { value: 1, message: TEXTS.INVALID_NUMBER }
                })}
                error={!!errors.capacity}
                helperText={errors.capacity?.message}
                fullWidth 
              />
              <FormControl fullWidth error={!!errors.type}>
                <InputLabel>{TEXTS.TYPE}</InputLabel>
                <Controller
                  name="type"
                  control={control}
                  rules={{ required: TEXTS.REQUIRED_FIELD }}
                  render={({ field }) => (
                    <Select
                      label={TEXTS.TYPE}
                      {...field}
                    >
                      {RANGE_TYPES.map(type => (
                        <MenuItem key={type.value} value={type.value}>
                          {type.label}
                        </MenuItem>
                      ))}
                    </Select>
                  )}
                />
                {errors.type && (
                  <Typography color="error" variant="caption">
                    {errors.type.message}
                  </Typography>
                )}
              </FormControl>
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseAddDialog}>
              {TEXTS.CANCEL}
            </Button>
            <Button type="submit" variant="contained">
              {TEXTS.ADD}
            </Button>
          </DialogActions>
        </form>
      </Dialog>

      {/* Edit Dialog */}
      <Dialog open={openEdit} onClose={handleCloseEditDialog}>
        <DialogTitle>{TEXTS.EDIT_RANGE}</DialogTitle>
        <form onSubmit={handleSubmit(onEdit)}>
          <DialogContent>
            <Stack gap={2} sx={{ mt: 1 }}>
              <TextField 
                label={TEXTS.LOCATION}
                {...register('location', { required: TEXTS.REQUIRED_FIELD })}
                error={!!errors.location}
                helperText={errors.location?.message}
                fullWidth 
              />
              <TextField 
                label={TEXTS.DESCRIPTION}
                {...register('description', { required: TEXTS.REQUIRED_FIELD })}
                error={!!errors.description}
                helperText={errors.description?.message}
                fullWidth 
              />
              <TextField 
                label={TEXTS.CAPACITY}
                type="number"
                {...register('capacity', { 
                  required: TEXTS.REQUIRED_FIELD,
                  min: { value: 1, message: TEXTS.INVALID_NUMBER }
                })}
                error={!!errors.capacity}
                helperText={errors.capacity?.message}
                fullWidth 
              />
              <FormControl fullWidth error={!!errors.type}>
                <InputLabel>{TEXTS.TYPE}</InputLabel>
                <Controller
                  name="type"
                  control={control}
                  rules={{ required: TEXTS.REQUIRED_FIELD }}
                  render={({ field }) => (
                    <Select
                      label={TEXTS.TYPE}
                      {...field}
                    >
                      {RANGE_TYPES.map(type => (
                        <MenuItem key={type.value} value={type.value}>
                          {type.label}
                        </MenuItem>
                      ))}
                    </Select>
                  )}
                />
                {errors.type && (
                  <Typography color="error" variant="caption">
                    {errors.type.message}
                  </Typography>
                )}
              </FormControl>
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseEditDialog}>
              {TEXTS.CANCEL}
            </Button>
            <Button type="submit" variant="contained">
              {TEXTS.SAVE}
            </Button>
          </DialogActions>
        </form>
      </Dialog>
    </Stack>
  );
}

export default OrganizationRangesPage;