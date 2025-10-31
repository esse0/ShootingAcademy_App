import { useEffect, useState, useMemo } from "react";
import {
  Box, Stack, Paper, Typography, Button, Dialog, DialogTitle, DialogContent, DialogActions, Select, MenuItem
} from "@mui/material";
import { DataGrid, GridActionsCellItem, GridColDef } from "@mui/x-data-grid";
import { DateCalendar } from '@mui/x-date-pickers/DateCalendar';
import { PickersDay, PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import { useForm, Controller } from "react-hook-form";
import AddIcon from "@mui/icons-material/Add";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import dayjs, { Dayjs } from "dayjs";
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { TrainingSessionDTO, CreateTrainingSessionDTO } from "../../types/TrainingSessionType";
import { useGetCoachGroups, useGetAvailableRanges, useCreateTrainingSession, useUpdateTrainingSession, useDeleteTrainingSession, useGetSchedule, useGetUserSchedule } from "../../api/trainingSession";
import { useSnackbar } from 'notistack';
import { useAtomValue } from 'jotai';
import { userAtom } from '../../jotai/atoms';

const STATUS_OPTIONS = [
  { value: "Planned", label: "Planned" },
  { value: "Ongoing", label: "Ongoing" },
  { value: "Completed", label: "Completed" },
  { value: "Cancelled", label: "Cancelled" }
];

export default function ModerateTraningPage({ isModerate = true }: { isModerate?: boolean }) {
  const [selectedDate, setSelectedDate] = useState<Dayjs | null>(dayjs());
  const [sessions, setSessions] = useState<TrainingSessionDTO[]>([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [editing, setEditing] = useState<TrainingSessionDTO | null>(null);
  const { enqueueSnackbar } = useSnackbar();
  const { control, handleSubmit, reset, formState: { errors }, watch, setError, clearErrors } = useForm<CreateTrainingSessionDTO>();
  const user = useAtomValue(userAtom);
  const isCoach = user?.role === 'coach';

  // API хуки
  const { execute: getGroups, resData: groups = [] } = useGetCoachGroups();
  const { execute: getRanges, resData: ranges = [] } = useGetAvailableRanges();
  const { execute: createSession } = useCreateTrainingSession();
  const { execute: updateSession } = useUpdateTrainingSession();
  const { execute: deleteSession } = useDeleteTrainingSession();
  const { execute: getSchedule } = useGetSchedule();
  const { execute: getUserSchedule } = useGetUserSchedule();

  // Загрузка данных при монтировании
  useEffect(() => {
    if (isCoach) {
      getGroups();
      getRanges();
      loadSessions();
    } else {
      loadUserSessions();
    }
  }, []);

  const loadSessions = async () => {
    const result = await getSchedule({});
    if (result?.data) {
      setSessions(result.data);
    }
  };

  const loadUserSessions = async () => {
    const result = await getUserSchedule();
    if (result?.data) {
      setSessions(result.data);
    }
  };

  // Для подсветки дней с тренировками
  const daysWithSessions = useMemo(() =>
    sessions.map((s: TrainingSessionDTO) => dayjs(s.startDate).format("YYYY-MM-DD")),
    [sessions]
  );

  function CustomDay(props: PickersDayProps<Dayjs>) {
    const isSelected = daysWithSessions.includes(props.day.format("YYYY-MM-DD"));
    return (
      <Box sx={{ position: "relative" }}>
        <PickersDay {...props} />
        {isSelected && <Box sx={{ position: "absolute", top: 2, right: 2, width: 8, height: 8, bgcolor: "primary.main", borderRadius: "50%" }} />}
      </Box>
    );
  }

  // Фильтрация тренировок на выбранный день
  const daySessions: TrainingSessionDTO[] = useMemo(() => {
    if (!selectedDate) return [];
    return sessions.filter(
      (s: any) => s && s.startDate && s.endDate && dayjs(s.startDate).isSame(selectedDate, "day")
    );
  }, [sessions, selectedDate]);

  const handleOpenAdd = () => {
    setEditing(null);
    reset({ 
      groupId: "", 
      rangeId: "", 
      status: "Planned", 
      startDate: selectedDate?.startOf("day").toISOString() || "", 
      endDate: selectedDate?.endOf("day").toISOString() || "" 
    });
    setOpenDialog(true);
  };

  const handleOpenEdit = (row: TrainingSessionDTO) => {
    setEditing(row);
    reset({
      groupId: row.groupId,
      rangeId: row.rangeId,
      status: row.status,
      startDate: row.startDate,
      endDate: row.endDate,
    });
    setOpenDialog(true);
  };

  const handleDelete = async (id: string) => {
    const result = await deleteSession(id);
    if (result) {
      await loadSessions();
      enqueueSnackbar("Session deleted", { variant: "success" });
    }
  };

  const onSubmit = async (data: CreateTrainingSessionDTO) => {
    let result;
    if (editing) {
      result = await updateSession({ id: editing.id, data });
    } else {
      result = await createSession(data);
    }
    
    if (result) {
      await loadSessions();
      setOpenDialog(false);
      enqueueSnackbar(editing ? "Session updated" : "Session created", { variant: "success" });
    }
  };

  // Кросс-валидация: старт < конец
  const startDate = watch('startDate');
  const endDate = watch('endDate');
  // Проверка и установка ошибки
  useEffect(() => {
    if (startDate && endDate) {
      if (dayjs(startDate).isAfter(dayjs(endDate)) || dayjs(startDate).isSame(dayjs(endDate))) {
        setError('startDate', { type: 'validate', message: 'Start time must be before end time' });
      } else {
        clearErrors('startDate');
      }
    }
  }, [startDate, endDate, setError, clearErrors]);

  const columns: GridColDef<TrainingSessionDTO>[] = [
    { field: "startDate", headerName: "Start", width: 140, renderCell: (params) => params.row && params.row.startDate ? new Date(params.row.startDate).toLocaleString() : "—" },
    { field: "endDate", headerName: "End", width: 140, renderCell: (params) => params.row && params.row.endDate ? new Date(params.row.endDate).toLocaleString() : "—" },
    { field: "groupName", headerName: "Group", width: 120 },
    { field: "trainerName", headerName: "Coach", width: 120 },
    { field: "rangeName", headerName: "Room", width: 120 },
    { field: "rangeLocation", headerName: "Address", width: 150 },
    { field: "status", headerName: "Status", width: 110, renderCell: (params) => params.row && params.row.status ? (STATUS_OPTIONS.find(opt => opt.value === params.row.status)?.label || params.row.status) : "—" },
    ...(isCoach && isModerate ? [{
      field: "actions", type: 'actions' as const, width: 90, getActions: ({ row }: { row: TrainingSessionDTO }) => [
        <GridActionsCellItem icon={<EditIcon />} label="Edit" onClick={() => handleOpenEdit(row)} />,
        <GridActionsCellItem icon={<DeleteIcon />} label="Delete" onClick={() => handleDelete(row.id)} />
      ]
    }] : [])
  ];

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Stack direction="row" spacing={2} sx={{ height: "100%", minHeight: 600 }}>
        {/* Левая панель: Календарь */}
        <Paper sx={{ width: "45%", p: 2, minWidth: 340 }}>
          <DateCalendar
            value={selectedDate}
            onChange={setSelectedDate}
            slots={{ day: CustomDay }}
          />
        </Paper>
        {/* Правая панель: Список и детали */}
        <Box sx={{ flex: 1, p: 2 }}>
          <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
            <Typography variant="h5">Sessions for {selectedDate?.format("DD MMM YYYY")}</Typography>
            {isCoach && isModerate && (
              <Button startIcon={<AddIcon />} variant="contained" onClick={handleOpenAdd}>Add session</Button>
            )}
          </Stack>
          <DataGrid<TrainingSessionDTO>
            rows={daySessions.filter(Boolean)}
            columns={columns}
            getRowId={(row) => row.id}
            autoHeight
            hideFooter
          />
        </Box>
        {/* Диалог создания/редактирования */}
        <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="xs" fullWidth>
          <DialogTitle>{editing ? "Edit session" : "Add session"}</DialogTitle>
          <form onSubmit={handleSubmit(onSubmit)}>
            <DialogContent>
              <Stack gap={2}>
                <Controller
                  name="groupId"
                  control={control}
                  rules={{ required: "Group is required" }}
                  render={({ field }) => (
                    <>
                      <Select {...field} label="Group" fullWidth displayEmpty error={!!errors.groupId}>
                        <MenuItem value="" disabled>Select group</MenuItem>
                        {(groups || []).map(g => <MenuItem key={g.id} value={g.id}>{g.name}</MenuItem>)}
                      </Select>
                      {errors.groupId && <Typography color="error" variant="caption">{errors.groupId.message}</Typography>}
                    </>
                  )}
                />
                <Controller
                  name="rangeId"
                  control={control}
                  rules={{ required: "Room is required" }}
                  render={({ field }) => (
                    <>
                      <Select {...field} label="Room" fullWidth displayEmpty error={!!errors.rangeId}>
                        <MenuItem value="" disabled>Select room</MenuItem>
                        {(ranges || []).map(r => <MenuItem key={r.id} value={r.id}>{r.name}</MenuItem>)}
                      </Select>
                      {errors.rangeId && <Typography color="error" variant="caption">{errors.rangeId.message}</Typography>}
                    </>
                  )}
                />
                <Controller
                  name="status"
                  control={control}
                  rules={{ required: "Status is required" }}
                  render={({ field }) => (
                    <>
                      <Select {...field} label="Status" fullWidth error={!!errors.status}>
                        {STATUS_OPTIONS.map(s => <MenuItem key={s.value} value={s.value}>{s.label}</MenuItem>)}
                      </Select>
                      {errors.status && <Typography color="error" variant="caption">{errors.status.message}</Typography>}
                    </>
                  )}
                />
                <Controller
                  name="startDate"
                  control={control}
                  rules={{ required: "Start time is required" }}
                  render={({ field }) => (
                    <>
                      <DateTimePicker label="Start" {...field} value={field.value ? dayjs(field.value) : null} onChange={(val: Dayjs | null) => field.onChange(val?.toISOString())} />
                      {errors.startDate && <Typography color="error" variant="caption">{errors.startDate.message}</Typography>}
                    </>
                  )}
                />
                <Controller
                  name="endDate"
                  control={control}
                  rules={{ required: "End time is required" }}
                  render={({ field }) => (
                    <>
                      <DateTimePicker label="End" {...field} value={field.value ? dayjs(field.value) : null} onChange={(val: Dayjs | null) => field.onChange(val?.toISOString())} />
                      {errors.endDate && <Typography color="error" variant="caption">{errors.endDate.message}</Typography>}
                    </>
                  )}
                />
              </Stack>
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
              <Button type="submit" variant="contained">{editing ? "Save" : "Add"}</Button>
            </DialogActions>
          </form>
        </Dialog>
      </Stack>
    </LocalizationProvider>
  );
}
