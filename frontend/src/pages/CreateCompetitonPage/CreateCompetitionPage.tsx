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

function CreateCompetitonPage() {
    const navigate = useNavigate();
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
        if (RecivedStatusCode === 201) {
            navigate('/app/moderatecompetitions');
        } else {
            alert('Не удалось создать competition');
        }
    }, [RecivedStatusCode]);

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
                                Title
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
                                Description
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
                                    Date
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
                                    Time
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
                                Max members count
                            </Typography>
                            <TextField
                                type="number"
                                variant="outlined"
                                placeholder="100"
                                size="small"
                                {...register('maxMemberCount', {
                                    required: 'MaxMemberCount is required',
                                    validate: (value) =>
                                        value > 0 ||
                                        'MaxMemberCount must be a positive number',
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
                        Exercises
                    </Typography>
                    <Select
                        labelId="label"
                        id="select"
                        size="small"
                        {...register('exercise')}
                        sx={{ width: 820 }}
                    >
                        <MenuItem value="PN rifle, 10m, 20 shots">
                            PN rifle, 10m, 20 shots
                        </MenuItem>
                        <MenuItem value="PN rifle, 10m, 40 shots">
                            PN rifle, 10m, 40 shots
                        </MenuItem>
                        <MenuItem value="Pistol, 10 meters, 60 shots">
                            Pistol, 10 meters, 60 shots
                        </MenuItem>
                        <MenuItem value="Pistol, 25 meters, 60 shots">
                            Pistol, 25 meters, 60 shots
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
                        Location
                    </Typography>
                    <Stack flexDirection={'row'} gap={2}>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                Country
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
                                City
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
                            Venue
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
                            Cancel
                        </Button>
                        <Button
                            sx={{ bgcolor: 'var(--accent-color)' }}
                            variant="contained"
                            color="primary"
                            type="submit"
                        >
                            Save changes
                        </Button>
                    </Stack>
                </Stack>
            </Stack>
        </form>
    );
}

export default CreateCompetitonPage;
