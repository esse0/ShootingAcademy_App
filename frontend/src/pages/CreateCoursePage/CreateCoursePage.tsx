import { Button, Divider, Stack, TextField, Typography, Select, MenuItem, SelectChangeEvent } from '@mui/material';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import CourseModule from '../../components/CourseModule/CourseModule';
import { useForm } from 'react-hook-form';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { useEffect, useState } from 'react';
import { tagGroups } from '../../consts/TagGroups';
import { CreateCourseType } from '../../types/CourseTypes';
import { useNavigate } from 'react-router';

function CreateCoursePage() {
    const navigate = useNavigate();

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

    const { execute: executeCourseData, statusCode: RecivedStatusCode } = useApi<null, CreateCourseType>(
        async (body) => {
            return axios.post('/api/course/create', body);
        },
    );

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<CreateCourseType>({
        mode: 'onBlur', // Проверять ошибки при потере фокуса
    });

    const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setCourse((prev) => ({
            ...prev,
            [name]: value, // Обновляем соответствующее поле в состоянии
        }));
    };

    const onChangeSelect = (e: SelectChangeEvent<string>) => {
        const { name, value } = e.target;
        setCourse((prev) => ({
            ...prev,
            [name]: value, // Обновляем соответствующее поле в состоянии
        }));
    };

    const IsNullParamsInModules = () => {
        let hasErrors = false;

        course.modules.forEach((modul) => {
            if (!modul.title || modul.title == '') {
                alert('Вы не заполнили title в ModulId:' + modul.id + ' модуле');
                hasErrors = true;
            }
            if (!modul.lessons || modul.lessons.length === 0) {
                alert('Вы не добавили ни одного lesson в ModulId:' + modul.id + ' модуле');
                hasErrors = true;
            }
            modul.lessons.forEach((lesson) => {
                if (!lesson.title || lesson.title == '') {
                    alert('Вы не заполнили title в LessonId:' + lesson.id + ' уроке');
                    hasErrors = true;
                }
                if (!lesson.description || lesson.description == '') {
                    alert('Вы не заполнили description в LessonId:' + lesson.id + ' уроке');
                    hasErrors = true;
                }
                if (!lesson.videoLink || lesson.videoLink == '') {
                    alert('Вы не заполнили videoLink в LessonId:' + lesson.id + ' урока');
                    hasErrors = true;
                }
            });
        });

        return hasErrors;
    };

    const onSubmit = async () => {
        if (IsNullParamsInModules()) return;
        executeCourseData(course);
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
        if (!RecivedStatusCode) return;
        if (RecivedStatusCode === 201) {
            navigate('/app/moderatecourses');
        }
    }, [RecivedStatusCode]);

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <Stack gap={2}>
                <Typography variant="h4" fontWeight={700} fontFamily={'inherit'}>
                    Create new course
                </Typography>

                <Stack gap={2}>
                    <Divider></Divider>
                    <Typography variant="h6" fontWeight={600} fontFamily={'inherit'}>
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
                                {...register('title', { required: 'Title is required' })}
                                onChange={onChange}
                                sx={{ width: 400 }}
                            />
                            {errors.title && <Typography color="error">{errors.title.message}</Typography>}
                        </Stack>
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                Description
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
                                Duration
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
                                Level
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
                                Rate
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
                        <Stack>
                            <Typography variant="body2" fontFamily={'inherit'}>
                                Category
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
                                <Stack key={index} gap={2} sx={{ border: 1 }} padding={2} width={440}>
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

export default CreateCoursePage;
