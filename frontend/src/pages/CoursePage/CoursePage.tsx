import {
    Accordion,
    AccordionDetails,
    AccordionSummary,
    Button,
    Card,
    CardContent,
    Checkbox,
    List,
    ListItemButton,
    Stack,
    Typography,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ReactPlayer from 'react-player';
import React, { useEffect, useState } from 'react';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { useNavigate, useParams } from 'react-router';
import ModulesTypeWithVideo, { CourseTypeWithVideo } from '../../types/CourseWithVideo';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import { useSnackbar } from 'notistack';

function CoursePage() {
    const navigate = useNavigate();
    const params = useParams();
    const courseId = params.id;
    const { enqueueSnackbar } = useSnackbar();

    const [modules, setModules] = React.useState(Array<ModulesTypeWithVideo>);
    const [selectedLessonId, setSelectedLessonId] = useState('');
    const [lessonData, setLessonData] = useState({
        id: '',
        title: '',
        description: '',
        videoLink: '',
        isCompleted: false,
        isLast: false,
    });

    const { resData: course, execute: execute } = useApi<CourseTypeWithVideo>(async () => {
        return axios.get('/api/course/fulldata', {
            params: { id: courseId },
        });
    });

    const {
        execute: executeFinishCourse,
        statusCode: finishCourseStatusCode,
        error: finishCourseError
    } = useApi(async () => {
        return axios.put('/api/Course/leaveCourse', null, { params: { courseId: courseId } });
    });

    const {
        execute: executeMarkLesson,
        error: markLessonError
    } = useApi(async () => {
        return axios.post(`/api/CourseProgress/${courseId}/lessons/${lessonData.id}/complete`);
    });

    const {
        execute: executeUnmarkLesson,
        error: unmarkLessonError
    } = useApi(async () => {
        return axios.delete(`/api/CourseProgress/${courseId}/lessons/${lessonData.id}/complete`);
    });

    useEffect(() => {
        if (!course) return;
        console.log('Course data:', course);
        console.log('Course is_closed:', course.is_closed);
        setModules(course.modules);
    }, [course]);

    useEffect(() => {
        execute();
    }, []);

    useEffect(() => {
        if (finishCourseStatusCode === 200) {
            enqueueSnackbar('Курс успешно завершен', { variant: 'success' });
            navigate('/app/myactivity');
        }
    }, [finishCourseStatusCode]);

    useEffect(() => {
        if (finishCourseError) {
            const errorMessage = finishCourseError.response?.data?.message || 'Ошибка завершения курса';
            enqueueSnackbar(errorMessage, { variant: 'error' });
        }
    }, [finishCourseError, enqueueSnackbar]);

    useEffect(() => {
        if (markLessonError) {
            const errorMessage = markLessonError.response?.data?.message || 'Ошибка при отметке урока';
            enqueueSnackbar(errorMessage, { variant: 'error' });
        }
    }, [markLessonError, enqueueSnackbar]);

    useEffect(() => {
        if (unmarkLessonError) {
            const errorMessage = unmarkLessonError.response?.data?.message || 'Ошибка при снятии отметки с урока';
            enqueueSnackbar(errorMessage, { variant: 'error' });
        }
    }, [unmarkLessonError, enqueueSnackbar]);

    const handleChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
        const isChecked = event.target.checked;
        
        try {
            if (isChecked) {
                await executeMarkLesson();
            } else {
                await executeUnmarkLesson();
            }
            
            // Обновляем состояние урока
            setLessonData(prev => ({
                ...prev,
                isCompleted: isChecked
            }));

            // Обновляем состояние в списке уроков
            setModules(prevModules => 
                prevModules.map(module => ({
                    ...module,
                    lessons: module.lessons.map(lesson => 
                        lesson.id === lessonData.id 
                            ? { ...lesson, isCompleted: isChecked }
                            : lesson
                    )
                }))
            );

            // Перезагружаем данные курса для обновления прогресса
            const response = await execute();
            if (response?.data) {
                setModules(response.data.modules);
            }
        } catch (error) {
            console.error('Ошибка при обновлении статуса урока:', error);
        }
    };

    const handleFinishCourse = async () => {
        try {
            await executeFinishCourse();
        } catch (error) {
            console.error('Ошибка при завершении курса:', error);
        }
    };

    return (
        <Stack direction={'row'} gap={'40px'}>
            <Stack flex={'0.5'}>
                {modules.map((module, i) => (
                    <Accordion defaultExpanded={i === 0} key={module.id} variant="outlined" disableGutters>
                        <AccordionSummary
                            expandIcon={<ExpandMoreIcon />}
                            aria-controls="panel1-content"
                            id="panel1-header"
                        >
                            <Typography fontFamily={'var(--primary-font)'} fontWeight={'500'}>
                                {module.title}
                            </Typography>
                        </AccordionSummary>
                        <AccordionDetails>
                            <List>
                                {module.lessons.map((lesson) => (
                                    <ListItemButton
                                        key={lesson.id}
                                        selected={selectedLessonId === lesson.id}
                                        onClick={() => {
                                            setLessonData({
                                                id: lesson.id,
                                                title: lesson.title,
                                                description: lesson.description,
                                                videoLink: lesson.videoUri ?? '',
                                                isCompleted: lesson.isCompleted,
                                                isLast:
                                                    lesson.id === module.lessons[module.lessons.length - 1].id &&
                                                    module.id === modules[modules.length - 1].id,
                                            });
                                            setSelectedLessonId(lesson.id);
                                        }}
                                        sx={{
                                            borderRadius: '8px',
                                            '&.Mui-selected': {
                                                backgroundColor: 'rgba(25, 118, 210, 0.2)',
                                                fontWeight: '600',
                                                '&:hover': {
                                                    backgroundColor: 'rgba(25, 118, 210, 0.3)',
                                                },
                                            },
                                        }}
                                    >
                                        <Stack flexDirection="row" justifyContent="space-between" width="100%">
                                            <Typography fontFamily={'var(--primary-font)'} fontWeight={'inherit'}>
                                                {lesson.title}
                                            </Typography>
                                            {lesson.isCompleted ? <CheckCircleOutlineIcon color='success'/> : <></>}
                                        </Stack>
                                    </ListItemButton>
                                ))}
                            </List>
                        </AccordionDetails>
                    </Accordion>
                ))}
            </Stack>
            <Stack />

            <Card variant="outlined" sx={{ flex: '1', height: '100%' }}>
                <CardContent sx={{ display: 'flex', gap: '25px', flexDirection: 'column' }}>
                    {lessonData.id != '' ? (
                        <>
                            <Typography variant="h4" fontWeight={'500'} fontFamily={'var(--primary-font)'}>
                                {lessonData.title}
                            </Typography>
                            {lessonData.videoLink != '' && (
                                <ReactPlayer minWidth={'900px'} width={'100%'} controls url={lessonData.videoLink} />
                            )}
                            <Stack>
                                <Typography variant="body1" fontFamily={'var(--primary-font)'}>
                                    {lessonData.description}
                                </Typography>
                                <Stack flexDirection="row" gap="5px" alignItems="center">
                                   <Checkbox
                                        disabled={course?.is_closed}
                                        checked={lessonData.isCompleted}
                                        onChange={handleChange}
                                        inputProps={{ 'aria-label': 'controlled' }}
                                    />
                                    Mark as complete
                                </Stack>
                            </Stack>
                            {lessonData.isLast && course && !course.is_closed && (
                                <Button
                                    onClick={handleFinishCourse}
                                    variant="contained"
                                    color="error"
                                    sx={{ ml: 'auto', maxWidth: '200px', maxHeight: '40px' }}
                                >
                                    End course
                                </Button>
                            )}
                        </>
                    ) : (
                        <Typography
                            mt={'2%'}
                            variant="h4"
                            textAlign={'center'}
                            fontWeight={'500'}
                            fontFamily={'var(--primary-font)'}
                            color={'text.secondary'}
                        >
                            Выберите урок
                        </Typography>
                    )}
                </CardContent>
            </Card>
        </Stack>
    );
}

export default CoursePage;
