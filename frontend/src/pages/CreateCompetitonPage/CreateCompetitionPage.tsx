import {
    Button,
    Divider,
    MenuItem,
    Select,
    Stack,
    TextField,
    Typography,
} from '@mui/material';
import {
    LocalizationProvider,
    DatePicker,
    TimePicker,
} from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { useForm } from 'react-hook-form';
import { CompetitionType } from '../../types/CompetitionBanerType';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { useEffect } from 'react';
import { useNavigate } from 'react-router';
import dayjs from 'dayjs';
import { useSnackbar } from 'notistack';
import { TEXTS } from '../../constants/texts';

function CreateCompetitonPage() {
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();
    const { execute: executeCreateCompetition, statusCode: RecivedStatusCode } =
        useApi<null, CompetitionType>(async (body) => {
            return axios.post('/api/Competition/create', body);
        });

    const {
        register,
        setValue,
        handleSubmit,
        formState: { errors },
    } = useForm<CompetitionType>();

    const onSubmit = (data: CompetitionType) => {
        const formattedData = {
            ...data,
            memberCount: Number(data.memberCount),
            maxMemberCount: Number(data.maxMemberCount),
        };
        
        executeCreateCompetition(formattedData);
    };

    useEffect(() => {
        if (!RecivedStatusCode) return;
        console.log('Received status code:', RecivedStatusCode);
        if (RecivedStatusCode === 200 || RecivedStatusCode === 201) {
            enqueueSnackbar(TEXTS.COMPETITION_CREATED, { variant: 'success' });
            navigate('/app/moderatecompetitions');
        } else {
            enqueueSnackbar(TEXTS.COMPETITION_CREATE_ERROR, { variant: 'error' });
        }
    }, [RecivedStatusCode, navigate, enqueueSnackbar]);

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <Stack gap={2}>
                <Typography
                    variant="h4"
                    fontWeight={700}
                    fontFamily={'inherit'}
                >
                    Create new competition
                </Typography>

                <Divider></Divider>
                <Stack gap={2}>
                    <Typography
                        variant="h6"
                        fontWeight={600}
                        fontFamily={'inherit'}
                    >
                        Basic details
                    </Typography>
                    <Stack flexDirection={'row'} gap={2}>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COMPETITION_TITLE}
                            </Typography>
                            <TextField
                                variant="outlined"
                                placeholder=""
                                size="small"
                                {...register('title')}
                                sx={{ width: 400 }}
                            />
                        </Stack>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COMPETITION_DESCRIPTION}
                            </Typography>
                            <TextField
                                variant="outlined"
                                placeholder=""
                                size="small"
                                {...register('description')}
                                sx={{ width: 400 }}
                            />
                        </Stack>
                    </Stack>
                    <Stack flexDirection={'row'} gap={2}>
                        <LocalizationProvider dateAdapter={AdapterDayjs}>
                            <Stack>
                                <Typography
                                    variant="body2"
                                    fontFamily={'inherit'}
                                >
                                    {TEXTS.COMPETITION_DATE}
                                </Typography>
                                <DatePicker
                                    label="Basic date picker"
                                    onChange={(newValue) =>
                                        setValue(
                                            'date',
                                            newValue
                                                ? dayjs(newValue).format('YYYY-MM-DD') : '',
                                        )
                                    }
                                    sx={{ width: 400 }}
                                />
                            </Stack>
                            <Stack>
                                <Typography
                                    variant="body2"
                                    fontFamily={'inherit'}
                                >
                                    {TEXTS.COMPETITION_TIME}
                                </Typography>
                                <TimePicker
                                    label="Basic time picker"
                                    onChange={(newValue) =>
                                        setValue(
                                            'time',
                                            newValue ? dayjs(newValue).format('HH:mm:ss') : ''
                                        )
                                    }
                                    sx={{ width: 400 }}
                                />
                            </Stack>
                        </LocalizationProvider>
                    </Stack>
                    <Stack flexDirection={'row'} gap={2}>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COMPETITION_MAX_MEMBERS}
                            </Typography>
                            <TextField
                                type="number"
                                variant="outlined"
                                placeholder="100"
                                size="small"
                                {...register('maxMemberCount', {
                                    required: TEXTS.REQUIRED_FIELD,
                                    validate: (value) =>
                                        value > 0 ||
                                        TEXTS.INVALID_NUMBER,
                                })}
                                sx={{ width: 400 }}
                            />
                            {errors.maxMemberCount && (
                                <Typography color="error">
                                    {errors.maxMemberCount.message}
                                </Typography>
                            )}
                        </Stack>
                    </Stack>
                </Stack>

                <Stack>
                    <Typography variant="body2" fontFamily={'inherit'}>
                        {TEXTS.COMPETITION_EXERCISE}
                    </Typography>
                    <Select
                        labelId="label"
                        id="select"
                        size="small"
                        {...register('exercise')}
                        sx={{ width: 820 }}
                    >
                        <MenuItem value={TEXTS.EXERCISES.RIFLE_10M_20}>
                            {TEXTS.EXERCISES.RIFLE_10M_20}
                        </MenuItem>
                        <MenuItem value={TEXTS.EXERCISES.RIFLE_10M_40}>
                            {TEXTS.EXERCISES.RIFLE_10M_40}
                        </MenuItem>
                        <MenuItem value={TEXTS.EXERCISES.PISTOL_10M_60}>
                            {TEXTS.EXERCISES.PISTOL_10M_60}
                        </MenuItem>
                        <MenuItem value={TEXTS.EXERCISES.PISTOL_25M_60}>
                            {TEXTS.EXERCISES.PISTOL_25M_60}
                        </MenuItem>
                    </Select>
                </Stack>

                <Divider></Divider>
                <Stack gap={2}>
                    <Typography
                        variant="h6"
                        fontWeight={600}
                        fontFamily={'inherit'}
                    >
                        {TEXTS.COMPETITION_LOCATION}
                    </Typography>
                    <Stack flexDirection={'row'} gap={2}>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COMPETITION_COUNTRY}
                            </Typography>
                            <TextField
                                variant="outlined"
                                placeholder="Belarus"
                                size="small"
                                {...register('country')}
                                sx={{ width: 400 }}
                            />
                        </Stack>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COMPETITION_CITY}
                            </Typography>
                            <TextField
                                variant="outlined"
                                placeholder="Minsk"
                                size="small"
                                {...register('city')}
                                sx={{ width: 400 }}
                            />
                        </Stack>
                    </Stack>

                    <Stack>
                        <Typography variant="body2" fontFamily={'inherit'}>
                            {TEXTS.COMPETITION_VENUE}
                        </Typography>
                        <TextField
                            variant="outlined"
                            placeholder="St. Sverdlova 13a"
                            size="small"
                            {...register('venue')}
                            sx={{ width: 400 }}
                        />
                    </Stack>
                </Stack>
                <Divider></Divider>
                <Stack gap={2}>
                    <Stack
                        flexDirection={'row'}
                        gap={1}
                        justifyContent={'flex-end'}
                    >
                        <Button
                            variant="outlined"
                            sx={{
                                borderColor: 'var(--accent-color)',
                                color: 'var(--accent-color)',
                            }}
                            onClick={() => navigate(-1)}
                        >
                            {TEXTS.CANCEL}
                        </Button>
                        <Button
                            sx={{ bgcolor: 'var(--accent-color)' }}
                            variant="contained"
                            color="primary"
                            type="submit"
                        >
                            {TEXTS.SAVE_CHANGES}
                        </Button>
                    </Stack>
                </Stack>
            </Stack>
        </form>
    );
}

export default CreateCompetitonPage;
