import { Button, Paper, Stack, Typography } from '@mui/material';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import GradeIcon from '@mui/icons-material/Grade';
import LinearProgressWithLabel from '../LeanerProgressWithLabel/LeanerProgressWithLabel';
import MyCourseBannerType from '../../types/MyCourseBannerType';
import { useNavigate } from 'react-router';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { useEffect } from 'react';

function MyCourse(props: MyCourseBannerType) {
    const navigation = useNavigate();

    const continueLearning = () => {
        navigation(`/app/course/${props.id}`);
    };

    const { statusCode, execute } = useApi(async () => {
        return axios.put('/api/course/leaveCourse', null, {
            params: { courseId: props.id },
        });
    });

    const leaveCourse = () => {
        execute();
    };
    const viewCourse = () => {
        navigation(`/app/course/${props.id}`);
    };

    useEffect(() => {
        if (statusCode == 200) window.location.reload();
    }, [statusCode]);

    return (
        <Paper
            elevation={0}
            sx={{
                bgcolor: '#F9F9F9',
                fontFamily: 'var(--primary-font)',
                width: '100%',
                height: '170px',
                borderRadius: '0.5em',
            }}
        >
            <Stack flexDirection={'row'} p={'25px'}>
                <img></img>
                <Stack width={'100%'} gap={'12px'}>
                    <Stack
                        flexDirection={'row'}
                        justifyContent={'space-between'}
                        width={'100%'}
                    >
                        <Typography
                            variant="h5"
                            fontWeight={'bold'}
                            fontFamily="inherit"
                        >
                            {props.title}
                        </Typography>
                        <span>
                            <Typography
                                variant="body2"
                                fontFamily="inherit"
                                color="text.secondary"
                            >
                                Start at {props.started_at}
                            </Typography>
                            {props.is_closed && (
                                <Typography
                                    variant="body2"
                                    fontFamily="inherit"
                                    color="text.secondary"
                                >
                                    End at {props.finished_at}
                                </Typography>
                            )}
                        </span>
                    </Stack>
                    <Stack flexDirection={'row'} gap="12px">
                        <Typography variant="body2" fontFamily="inherit">
                            Self-paced course
                        </Typography>

                        <Stack
                            flexDirection={'row'}
                            gap="6px"
                            alignContent={'center'}
                        >
                            <AccessTimeIcon sx={{ color: '#455CC7' }} />
                            <Typography variant="body2" fontFamily="inherit">
                                {props.duration}
                            </Typography>
                        </Stack>

                        <Stack
                            flexDirection={'row'}
                            gap="6px"
                            alignContent={'center'}
                        >
                            <GradeIcon sx={{ color: '#455CC7' }} />
                            <Typography variant="body2" fontFamily="inherit">
                                {props.level}
                            </Typography>
                        </Stack>
                    </Stack>
                    <Stack
                        flexDirection={'row'}
                        flexGrow={1}
                        width={'100%'}
                        height={'40px'}
                    >
                        <LinearProgressWithLabel
                            value={props.completed_percent}
                        />
                        <Stack flexDirection={'row'} gap="12px" ml={'50px'}>
                            {props.completed_percent != 100 &&
                            !props.is_closed ? (
                                <>
                                    <Button
                                        variant="outlined"
                                        sx={{
                                            borderColor: '#455CC7',
                                            color: '#455CC7',
                                            borderRadius: '0.3em',
                                            fontFamily: 'inherit',
                                            fontSize: '12px',
                                            width: '110px',
                                        }}
                                        onClick={leaveCourse}
                                    >
                                        Leave Course
                                    </Button>
                                    <Button
                                        variant="contained"
                                        sx={{
                                            bgcolor: '#455CC7',
                                            borderRadius: '0.3em',
                                            fontFamily: 'inherit',
                                            fontSize: '12px',
                                            width: '120px',
                                        }}
                                        onClick={continueLearning}
                                    >
                                        Continue learning
                                    </Button>
                                </>
                            ) : props.completed_percent == 100 ? (
                                <Button
                                    variant="contained"
                                    sx={{
                                        bgcolor: '#455CC7',
                                        borderRadius: '0.3em',
                                        fontFamily: 'inherit',
                                        fontSize: '12px',
                                        width: '120px',
                                    }}
                                    onClick={() => viewCourse()}
                                >
                                    View
                                </Button>
                            ) : (
                                <></>
                            )}
                        </Stack>
                    </Stack>
                </Stack>
            </Stack>
        </Paper>
    );
}

export default MyCourse;
