import { Button, Paper, Rating, Stack, Typography } from "@mui/material";
import CourseBannerType from "../../types/CourseBannerType";
import { Link } from "react-router";
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import GradeIcon from '@mui/icons-material/Grade';

function Course({course, courseLink}: {course: CourseBannerType, courseLink: string}){
    return(
        <Paper elevation={0} sx={{bgcolor:"#F9F9F9", fontFamily: "var(--primary-font)", width:"100%", borderRadius: '0.5em'}}>
            <Stack p={'25px'} gap={'12px'}>
                <Stack flexDirection={"row"} justifyContent={"space-between"} width={"100%"}>
                    <Typography variant="h5" fontFamily={"inherit"} fontWeight={"bold"}>{course.title}</Typography>
                    <Rating name="half-rating-read" defaultValue={course.rate} precision={0.1} readOnly />
                </Stack>
                
                <Stack flexDirection={"row"} gap='5px'>
                    <GradeIcon sx={{ color: '#455CC7' }}></GradeIcon>
                    <Typography variant="body1" fontWeight={"600"} fontFamily={"inherit"}>{course.level}</Typography>
                </Stack>

                <Stack flexDirection={"row"} gap='5px'>
                    <AccessTimeIcon sx={{ color: '#455CC7' }}></AccessTimeIcon>
                    <Typography variant="body1" fontWeight={"600"} fontFamily={"inherit"}>{course.duration}</Typography>
                </Stack>
                <p>category: {course.category}</p>
                <Stack flexDirection={"row"} justifyContent={"space-between"} width={"100%"}>
                    <Typography variant="body1" fontFamily={"inherit"}>{course.description}</Typography>
                    <Button to={`${courseLink}/${course.id}`} component={Link} variant="outlined" sx={{ alignSelf: 'flex-end', borderBlockColor: '#455CC7', color: '#455CC7', fontFamily: "inherit", fontSize: '14px', maxHeight: '40px' }}>See Course</Button>
                </Stack>
            </Stack>
        </Paper>
    );
}

export default Course