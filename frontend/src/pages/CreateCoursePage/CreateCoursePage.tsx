import { Button, Divider, Stack, TextField, Typography, Select, MenuItem, SelectChangeEvent } from '@mui/material';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import CourseModule from '../../components/CourseModule/CourseModule';
import { useForm } from 'react-hook-form';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { useEffect, useState } from 'react';
import { tagGroups } from '../../constants/TagGroups';
import { CreateCourseType } from '../../types/CourseTypes';
import { useNavigate } from 'react-router';
import { useSnackbar } from 'notistack';
import { TEXTS } from '../../constants/texts';

function CreateCoursePage() {
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();

    const [course, setCourse] = useState<CreateCourseType>({
        id: '',
        title: '',
        description: '',
        duration: '',
        level: '',
        category: '',
        rate: 0,
        modules: [{ id: '', title: '', lessons: [] }],
    });

    const [moduleIdCount, setModuleIdCount] = useState(1);

    const { execute: executeCreateCourse, statusCode: receivedStatusCode } = useApi<null, CreateCourseType>(
        async (body) => {
            return axios.post('/api/course/create', body);
        }
    );

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<CreateCourseType>({
        mode: 'onBlur',
    });

    const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setCourse((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const onChangeSelect = (e: SelectChangeEvent<string>) => {
        const { name, value } = e.target;
        setCourse((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const IsNullParamsInModules = () => {
        let hasErrors = false;

        course.modules.forEach((module) => {
            if (!module.title || module.title == '') {
                enqueueSnackbar(TEXTS.MODULE_TITLE_EMPTY + module.id, { variant: 'error' })
                hasErrors = true;
            }
            if (!module.lessons || module.lessons.length === 0) {
                enqueueSnackbar(TEXTS.LESSON_EMPTY + module.id, { variant: 'error' })
                hasErrors = true;
            }
            module.lessons.forEach((lesson) => {
                if (!lesson.title || lesson.title == '') {
                    enqueueSnackbar(TEXTS.LESSON_TITLE_EMPTY + lesson.id, { variant: 'error' })
                    hasErrors = true;
                }
                if (!lesson.description || lesson.description == '') {
                    enqueueSnackbar(TEXTS.LESSON_DESCRIPTION_EMPTY + lesson.id, { variant: 'error' })
                    hasErrors = true;
                }
                if (!lesson.videoId || lesson.videoId == '') {
                    enqueueSnackbar(TEXTS.VIDEO_NOT_UPLOADED + lesson.id, { variant: 'error' })
                    hasErrors = true;
                }
            });
        });

        return hasErrors;
    };

    const onSubmit = async () => {
        if (IsNullParamsInModules()) return;
        executeCreateCourse(course);
        console.log(course);
    };

    const addNewModule = () => {
        setCourse((prev) => {
            return {
                ...prev,
                modules: [...prev.modules, { id: String(moduleIdCount), title: '', lessons: [] }],
            };
        });
        setModuleIdCount(moduleIdCount + 1);
    };

    const deleteModule = (index: number) => {
        setCourse((prev) => {
            return {
                ...prev,
                modules: prev.modules.filter((_, i) => i !== index),
            };
        });
    };

    useEffect(() => {
        if (!receivedStatusCode) return;
        console.log('Received status code:', receivedStatusCode);
        if (receivedStatusCode === 200 || receivedStatusCode === 201) {
            enqueueSnackbar(TEXTS.COURSE_CREATED, { variant: 'success' });
            navigate('/app/moderatecourses');
        } else {
            enqueueSnackbar(TEXTS.COURSE_CREATE_ERROR, { variant: 'error' });
        }
    }, [receivedStatusCode, navigate, enqueueSnackbar]);

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <Stack gap={2}>
                <Typography
                    variant="h4"
                    fontWeight={700}
                    fontFamily={'inherit'}
                >
                    Create new course
                </Typography>

                <Stack gap={2}>
                    <Divider></Divider>
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
                                {TEXTS.COURSE_TITLE}
                            </Typography>
                            <TextField
                                variant="outlined"
                                placeholder=""
                                size="small"
                                {...register('title', { required: 'Title is required' })}
                                onChange={onChange}
                                sx={{ width: 400 }}
                            />
                            {errors.title && <Typography color="error">{errors.title.message}</Typography>}
                        </Stack>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COURSE_DESCRIPTION}
                            </Typography>
                            <TextField
                                variant="outlined"
                                placeholder=""
                                size="small"
                                {...register('description', { required: 'Description is required' })}
                                onChange={onChange}
                                sx={{ width: 400 }}
                            />
                            {errors.description && <Typography color="error">{errors.description.message}</Typography>}
                        </Stack>
                    </Stack>

                    <Stack flexDirection={'row'} gap={2}>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COURSE_DURATION}
                            </Typography>

                            <Select
                                labelId="label"
                                id="select"
                                size="small"
                                defaultValue=""
                                {...register('duration', { required: 'Duration is required' })}
                                onChange={onChangeSelect}
                                sx={{ width: 400 }}
                            >
                                {tagGroups
                                    .find((tag) => tag.tagGroupTitle === 'Duration')
                                    ?.tags.map((item, i) => (
                                        <MenuItem key={i} value={item}>{item}</MenuItem>
                                    ))}
                            </Select>
                            {errors.duration && <Typography color="error">{errors.duration.message}</Typography>}
                        </Stack>

                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COURSE_LEVEL}
                            </Typography>
                            <Select
                                labelId="label"
                                id="select"
                                size="small"
                                defaultValue=""
                                {...register('level', { required: 'Level is required' })}
                                onChange={onChangeSelect}
                                sx={{ width: 400 }}
                            >
                                {tagGroups
                                    .find((tag) => tag.tagGroupTitle === 'Level')
                                    ?.tags.map((item, i) => (
                                        <MenuItem key={i} value={item}>{item}</MenuItem>
                                    ))}
                            </Select>
                            {errors.level && <Typography color="error">{errors.level.message}</Typography>}
                        </Stack>
                    </Stack>

                    <Stack flexDirection={'row'} gap={2}>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COURSE_CATEGORY}
                            </Typography>
                            <TextField
                                variant="outlined"
                                placeholder=""
                                size="small"
                                {...register('category', { required: 'Category is required' })}
                                onChange={onChange}
                                sx={{ width: 400 }}
                            />
                            {errors.category && <Typography color="error">{errors.category.message}</Typography>}
                        </Stack>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                {TEXTS.COURSE_RATE}
                            </Typography>
                            <TextField
                                type="number"
                                variant="outlined"
                                placeholder="Rate (1-5)"
                                size="small"
                                {...register('rate', {
                                    required: 'Rate is required',
                                    min: { value: 1, message: 'Rate must be at least 1' },
                                    max: { value: 5, message: 'Rate must be at most 5' },
                                })}
                                onChange={onChange}
                                sx={{ width: 400 }}
                            />
                            {errors.rate && <Typography color="error">{errors.rate.message}</Typography>}
                        </Stack>
                    </Stack>
                </Stack>
                <Divider></Divider>

                <Stack>
                    <Stack flexDirection={'row'} alignItems={'center'}>
                        <Typography variant="h6" fontWeight={600} fontFamily={'inherit'}>
                            Modules
                        </Typography>

                        <Stack flexDirection={'row'} justifyContent={'flex-start'}>
                            <Button onClick={addNewModule}>
                                <AddCircleOutlineIcon sx={{ color: 'var(--accent-color)' }} />
                            </Button>
                        </Stack>
                    </Stack>

                    <Stack flexDirection={'row'} gap={4} flexWrap={'wrap'}>
                        {course.modules.map((item, index) => {
                            return (
                                <Stack key={index} gap={2} sx={{ border: 1 }} padding={2} width={500}>
                                    <Stack flexDirection={'row'} justifyContent={'flex-end'}>
                                        <Button
                                            onClick={() => {
                                                deleteModule(index);
                                            }}
                                        >
                                            <CancelOutlinedIcon
                                                sx={{
                                                    color: 'var(--accent-color)',
                                                }}
                                            />
                                        </Button>
                                    </Stack>
                                    <CourseModule
                                        moduleIdCount={item.id}
                                        course={course}
                                        setCourse={setCourse}
                                        key={index}
                                    />
                                    
                                </Stack>
                            );
                        })}
                    </Stack>
                </Stack>
                <Divider></Divider>

                <Stack gap={2}>
                    <Stack flexDirection={'row'} gap={1} justifyContent={'flex-end'}>
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

export default CreateCoursePage;
