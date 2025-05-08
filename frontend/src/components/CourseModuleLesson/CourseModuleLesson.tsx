import { Stack, TextField, Typography} from '@mui/material';
import { CreateCourseType } from '../../types/CourseTypes';
import { ChangeEvent } from 'react';


interface Props {
    moduleIdCount: string;
    LessonIdCount: string;
    course: CreateCourseType;
    setCourse: React.Dispatch<React.SetStateAction<CreateCourseType>>;
}
export function CreateCourleModuleLesson(props: Props) {

    const onChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;

        props.setCourse((prev) => {
            return {
                ...prev,
                modules: prev.modules.map((modul) => {
                    if (modul.id === props.moduleIdCount) {
                        return {
                            ...modul,
                            lessons: modul.lessons.map((lesson) => {
                                if (lesson.id === props.LessonIdCount) {
                                    return {
                                        ...lesson,
                                        [name]: value,
                                    };
                                }
                                return lesson;
                            }),
                        };
                    }
                    return modul;
                }),
            };
        });
    };

    return (
        <Stack>
            <Stack>
                <Typography variant="body2" fontFamily={'inherit'}>
                    Title
                </Typography>
                <TextField name='title' variant="outlined" placeholder="" size="small" onChange={onChange} sx={{ width: 300 }} />
            </Stack>
            <Stack>
                <Typography variant="body2" fontFamily={'inherit'}>
                    Description
                </Typography>
                <TextField name='description' variant="outlined" placeholder="" size="small" onChange={onChange} sx={{ width: 300 }} />
            </Stack>
            <Stack>
                <Typography variant="body2" fontFamily={'inherit'}>
                    VideoLink
                </Typography>
                <TextField name='videoLink' variant="outlined" placeholder="" size="small" onChange={onChange} sx={{ width: 300 }} />
            </Stack>
        </Stack>
    );
}
