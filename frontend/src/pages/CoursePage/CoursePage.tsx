import {
    Accordion,
    AccordionDetails,
    AccordionSummary,
    Button,
    Card,
    CardContent,
    List,
    ListItemButton,
    Stack,
    Typography,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ReactPlayer from 'react-player';
import React, { useEffect, useState } from 'react';
import ModulesType, { CourseType } from '../../types/CourseTypes';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { useNavigate, useParams } from 'react-router';

function CoursePage() {
    const navigate = useNavigate();
    const params = useParams();
    const courseId = params.id;

    const [modules, setModules] = React.useState(Array<ModulesType>);

    const [selectedLessonId, setSelectedLessonId] = useState('');

    const [lessonData, setLessonData] = useState({
        id: '',
        title: '',
        description: '',
        videoLink: '',
        isLast: false,
    });

    const { resData: course, execute: execute } = useApi<CourseType>(async () => {
        return axios.get('/api/course/fulldata', {
            params: { id: courseId },
        });
    });

    const {
        execute: executeFinishCourse,
        statusCode: finishCourseStatusCode,
    } = useApi(async () => {
        return axios.put('/api/Course/leaveCourse', null, { params: { courseId: courseId } });
    });

    useEffect(() => {
        if (!course) return;
       
        setModules(course.modules);
    }, [course]);

    useEffect(() => {
        execute();
    }, []);

    useEffect(() => {
        if (finishCourseStatusCode == 200) navigate('/app/myactivity');
    }, [finishCourseStatusCode]);

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
                                                videoLink: lesson.videoLink ?? '',
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
                                        <Typography fontFamily={'var(--primary-font)'} fontWeight={'inherit'}>
                                            {lesson.title}
                                        </Typography>
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
                            <Typography variant="body1" fontFamily={'var(--primary-font)'}>
                                {lessonData.description}
                            </Typography>
                            {lessonData.isLast && (
                                <Button
                                    onClick={() => {
                                        executeFinishCourse();
                                    }}
                                    variant="contained"
                                    color="error"
                                    sx={{ ml: 'auto', maxWidth: '200px', maxHeight: '40px' }}
                                >
                                    End course
                                </Button>
                            )}{' '}
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
                            Select a lesson
                        </Typography>
                    )}
                </CardContent>
            </Card>
        </Stack>
    );
}

export default CoursePage;
