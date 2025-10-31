import { Button, Divider, Stack, TextField, Typography, } from '@mui/material';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import { CreateCourseType } from '../../types/CourseTypes';
import { useState } from 'react';
import { CreateCourleModuleLesson } from '../CourseModuleLesson/CourseModuleLesson';

interface Props {
    moduleIdCount: string;
    course: CreateCourseType;
    setCourse: React.Dispatch<React.SetStateAction<CreateCourseType>>;
}
function CourseModule(props: Props) {
    const [lessonId, setLessonId] = useState(1);

    const updateTitle = (e: React.ChangeEvent<HTMLInputElement>) => {
        props.setCourse((prev) => {
            return {
                ...prev,
                modules: prev.modules.map((module) => {
                    if (module.id == props.moduleIdCount) {
                        return { ...module, title: e.target.value };
                    } else {
                        return module;
                    }
                }),
            };
        });
    };

    const addNewLesson = () => {
        props.setCourse((prev) => {
            return {
                ...prev,
                modules: prev.modules.map((modul) => {
                    if (modul.id == props.moduleIdCount) {
                        return {
                            ...modul,
                            lessons: [
                                ...modul.lessons,
                                { id: String(lessonId), title: '', description: '', videoId: '' },
                            ],
                        };
                    } 
                    return modul;
                }),
            };
        });
        setLessonId(lessonId + 1);
    };

    const deleteLesson = (index: number) => {
        props.setCourse((prev) => {
            return {
                ...prev,
                modules: prev.modules.map(modul=>{
                    if (modul.id === props.moduleIdCount){
                        return {
                            ...modul,
                            lessons: modul.lessons.filter((_, i) => i !== index)
                        }
                    }
                    return modul;
                })
            };
        });
    };

    return (
        <>
            <Stack>
                <Typography variant="body2" fontFamily={'inherit'}>
                    Title
                </Typography>
                <TextField variant="outlined" placeholder="" size="small" onChange={updateTitle} sx={{ width: 400 }} />
            </Stack>
            <Divider />
            <Stack flexDirection={'row'} alignItems={'center'}>
                <Typography variant="h6" fontWeight={600} fontFamily={'inherit'}>
                    Lessons
                </Typography>
                <Stack flexDirection={'row'} justifyContent={'flex-start'}>
                    <Button onClick={addNewLesson}>
                        <AddCircleOutlineIcon sx={{ color: 'var(--accent-color)' }} />
                    </Button>
                </Stack>
            </Stack>

            <Stack flexWrap={'wrap'} gap={2} sx={{ maxHeight: '400px', overflow: 'auto' }}>
                {props.course.modules
                    .find((modul) => modul.id == props.moduleIdCount)
                    ?.lessons.map((item, index) => {
                        return (
                            <Stack sx={{ border: 1 }} padding={2}>
                                <Stack flexDirection={'row'} justifyContent={'flex-end'}>
                                    <Button
                                    onClick={()=>{deleteLesson(index)}}
                                    >
                                        <CancelOutlinedIcon sx={{ color: 'var(--accent-color)' }} />
                                    </Button>
                                </Stack>
                                <CreateCourleModuleLesson
                                    moduleIdCount={props.moduleIdCount}
                                    LessonIdCount={item.id}
                                    course={props.course}
                                    setCourse={props.setCourse}
                                    key={index}
                                />
                            </Stack>
                        );
                    })}
            </Stack>
        </>
    );
}

export default CourseModule;
