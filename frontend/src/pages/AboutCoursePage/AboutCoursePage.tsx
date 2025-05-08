import {
    Accordion,
    AccordionDetails,
    AccordionSummary,
    Box,
    Button,
    Rating,
    Stack,
    Step,
    StepButton,
    StepLabel,
    Stepper,
    SxProps,
    Theme,
    Typography,
} from '@mui/material';
import { Link as RouterLink, useNavigate, useParams } from 'react-router';
import QontoStepIcon from '../../components/StepperIcon/StepperIcon';
import React, { useEffect } from 'react';
import ReviewBaner from '../../components/ReviewBaner/ReviewBaner';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import './AboutCoursePage.css';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';
import { CourseType } from '../../types/CourseTypes';

function AboutCoursePage(props: { sx?: SxProps<Theme>; isApp: boolean }) {
    const params = useParams();
    const courseId = params.id;
    const navigate = useNavigate();
    const [activeStep, setActiveStep] = React.useState(0);
    const reviews = [
        {
            id: '1',
            user: 'User 1',
            rate: 3,
            comment: 'Good course',
        },
        {
            id: '2',
            user: 'User 2',
            rate: 4,
            comment: 'Amazing',
        },
        {
            id: '3',
            user: 'User 3',
            rate: 5,
            comment: '5 stars',
        },
    ];

    const { resData: course, execute: execute } = useApi<CourseType>(
        async () => {
            return axios.get('/api/course/fulldata', {
                params: { id: courseId },
            });
        },
    );

    const { execute: executeAddToCourse, statusCode: RecivedStatus } = useApi(
        async () => {
            return axios.post('/api/Course/subscribe', null, {
                params: { courseId: courseId },
            });
        },
    );

    useEffect(() => {
        execute();
    }, []);

    const startLearning = () => {
        executeAddToCourse();
    };

    const handleStep = (step: number) => () => {
        setActiveStep(step);
    };

    useEffect(() => {
        if (RecivedStatus == 200) navigate(`/app/course/${course?.id}`);

    }, [RecivedStatus]);

    return (
        <>
            {course && (
                <Stack sx={props.sx} fontFamily={'var(--primary-font)'}>
                    <Stack flexDirection="row" flexWrap="wrap" gap="40px">
                        <Stack gap={'40px'} flex={2}>
                            <Typography
                                variant="h4"
                                fontWeight="bold"
                                fontFamily="inherit"
                            >
                                {course.title}
                            </Typography>
                            <Typography
                                variant="body1"
                                fontWeight="500"
                                fontFamily="inherit"
                            >
                                {course.description}
                            </Typography>

                            <ul
                                style={{ listStyle: 'disc' }}
                                className="about-course-list"
                            >
                                <li>
                                    <Typography
                                        variant="body1"
                                        fontWeight="500"
                                        fontFamily="inherit"
                                    >
                                        <span
                                            style={{
                                                color: 'var(--accent-color)',
                                            }}
                                        >
                                            Duration:
                                        </span>{' '}
                                        {course.duration}
                                    </Typography>
                                </li>
                                <li>
                                    <Typography
                                        variant="body1"
                                        fontWeight="500"
                                        fontFamily="inherit"
                                    >
                                        <span
                                            style={{
                                                color: 'var(--accent-color)',
                                            }}
                                        >
                                            Level:
                                        </span>{' '}
                                        {course.level}
                                    </Typography>
                                </li>
                                <li>
                                    <Stack flexDirection="row" gap={'5px'}>
                                        <Typography
                                            variant="body1"
                                            fontWeight="500"
                                            fontFamily="inherit"
                                        >
                                            <span
                                                style={{
                                                    color: 'var(--accent-color)',
                                                }}
                                            >
                                                Rating:
                                            </span>
                                        </Typography>
                                        <Rating
                                            defaultValue={course.rate}
                                            precision={0.1}
                                            readOnly
                                        />
                                        <Typography
                                            variant="body1"
                                            fontWeight="500"
                                            fontFamily="inherit"
                                        >
                                            ({course.peopleRateCount} students )
                                        </Typography>
                                    </Stack>
                                </li>
                            </ul>

                            {props.isApp ? (
                                <Stack flexDirection="row" gap={'20px'}>
                                    <Button
                                        sx={{
                                            width: '200px',
                                            bgcolor: '#455CC7',
                                            color: 'white',
                                            fontFamily: 'inherit',
                                        }}
                                        variant="contained"
                                        onClick={() => startLearning()}
                                    >
                                        Start Learning
                                    </Button>
                                </Stack>
                            ) : (
                                <Button
                                    sx={{
                                        width: '200px',
                                        bgcolor: '#455CC7',
                                        color: 'white',
                                        fontFamily: 'inherit',
                                    }}
                                    variant="contained"
                                    component={RouterLink}
                                    to="/signup"
                                >
                                    Enroll Now
                                </Button>
                            )}
                        </Stack>

                        <Stack
                            position={'relative'}
                            border={'1px solid #455CC7'}
                            borderRadius={'10px'}
                            p={'20px'}
                            m={'80px 0 20px 0'}
                            gap="10px"
                        >
                            <img
                                style={{
                                    borderRadius: '15px',
                                    width: '160px',
                                    height: '140px',
                                    position: 'absolute',
                                    top: '-100px',
                                    right: '25%',
                                    objectFit: 'cover',
                                }}
                                src="https://static.vecteezy.com/system/resources/thumbnails/005/346/410/small_2x/close-up-portrait-of-smiling-handsome-young-caucasian-man-face-looking-at-camera-on-isolated-light-gray-studio-background-photo.jpg"
                                alt=""
                            />
                            <Typography
                                pt={'30px'}
                                fontFamily="inherit"
                                variant="body1"
                                fontWeight="bold"
                            >
                                About the Instructor:{' '}
                            </Typography>
                            <Typography
                                fontFamily="inherit"
                                variant="body1"
                                fontWeight="500"
                            >
                                {course?.instructor.firstName}{' '}
                                {course?.instructor.secoundName}{' '}
                                {course?.instructor.patronymicName} |{' '}
                                {course?.instructor.grade}
                            </Typography>
                            <Typography
                                fontFamily="inherit"
                                variant="body1"
                                fontWeight="500"
                            >
                                instructor description
                            </Typography>
                        </Stack>
                    </Stack>

                    <Stack gap={'60px'}>
                        <Typography
                            variant="h4"
                            fontWeight="bold"
                            fontFamily="inherit"
                            textAlign="center"
                        >
                            Course Curriculum
                        </Typography>

                        <Box sx={{ width: '100%' }}>
                            <Stepper
                                activeStep={activeStep}
                                nonLinear
                                alternativeLabel
                            >
                                {course.modules.map((module, index) => (
                                    <Step
                                        key={index}
                                        completed={index === activeStep}
                                    >
                                        <StepButton
                                            disableTouchRipple
                                            color="inherit"
                                            onClick={handleStep(index)}
                                        >
                                            <StepLabel
                                                slots={{
                                                    stepIcon: QontoStepIcon,
                                                }}
                                            >
                                                <Stack>
                                                    <Typography
                                                        fontWeight="bold"
                                                        fontFamily="var(--primary-font)"
                                                        sx={
                                                            index === activeStep
                                                                ? {
                                                                      color: '#455CC7',
                                                                  }
                                                                : {
                                                                      color: 'inherit',
                                                                  }
                                                        }
                                                    >
                                                        Module {index + 1}
                                                    </Typography>
                                                    <Typography fontFamily="var(--primary-font)">
                                                        {module.title}
                                                    </Typography>
                                                </Stack>
                                            </StepLabel>
                                        </StepButton>
                                    </Step>
                                ))}
                            </Stepper>
                        </Box>

                        <Stack>
                            <ul
                                style={{
                                    display: 'flex',
                                    gap: '10px',
                                    flexDirection: 'column',
                                }}
                            >
                                {course.modules[activeStep]?.lessons.map(
                                    (lesson, i) => (
                                        <li key={i}>
                                            <Typography
                                                variant="h6"
                                                fontWeight="500"
                                                fontFamily="inherit"
                                            >
                                                {lesson.title}
                                            </Typography>
                                        </li>
                                    ),
                                )}
                            </ul>
                        </Stack>
                    </Stack>

                    <Stack gap="40px">
                        <Typography
                            variant="h4"
                            fontWeight="bold"
                            fontFamily="inherit"
                        >
                            Course Features
                        </Typography>

                        <ul
                            className="list"
                            style={{
                                display: 'flex',
                                gap: '10px',
                                flexDirection: 'column',
                            }}
                        >
                            {course.features.map((feature, i) => (
                                <li
                                    key={i}
                                    style={{
                                        display: 'flex',
                                        flexDirection: 'column',
                                        gap: '5px',
                                    }}
                                >
                                    <Typography
                                        fontSize="20px"
                                        fontWeight="600"
                                        fontFamily="inherit"
                                        color="var(--accent-color)"
                                    >
                                        {i + 1}- {feature.title}:
                                    </Typography>

                                    <Typography
                                        fontSize="18px"
                                        fontWeight="500"
                                        fontFamily="inherit"
                                    >
                                        "{feature.description}."
                                    </Typography>
                                </li>
                            ))}
                        </ul>
                    </Stack>

                    <Stack gap="40px">
                        <Typography
                            variant="h4"
                            fontWeight="bold"
                            fontFamily="inherit"
                        >
                            Student Reviews
                        </Typography>

                        <Stack
                            flexDirection="row"
                            justifyContent="space-between"
                        >
                            {reviews.map((item, i) => (
                                <ReviewBaner
                                    key={i}
                                    item={item}
                                    sx={{ width: '400px', height: '150px' }}
                                />
                            ))}
                        </Stack>
                    </Stack>

                    <Stack gap={'40px'} mb={'140px'}>
                        <Typography
                            variant="h4"
                            fontWeight="bold"
                            fontFamily="inherit"
                        >
                            FAQs :
                        </Typography>
                        <Stack gap="20px">
                            {course.faqs.map((faq, i) => (
                                <Accordion
                                    key={i}
                                    sx={{
                                        boxShadow: 'none',
                                        border: '1px solid var(--accent-color)',
                                    }}
                                >
                                    <AccordionSummary
                                        expandIcon={
                                            <ExpandMoreIcon
                                                sx={{
                                                    color: 'var(--accent-color)',
                                                    border: '1px solid var(--accent-color)',
                                                    borderRadius: '10%',
                                                }}
                                            />
                                        }
                                        aria-controls="panel1-content"
                                        id="panel1-header"
                                    >
                                        <Typography
                                            fontSize={'18px'}
                                            fontWeight={'bold'}
                                            fontFamily={'inherit'}
                                            component="span"
                                        >
                                            {faq.question}
                                        </Typography>
                                    </AccordionSummary>
                                    <AccordionDetails>
                                        <Typography fontFamily={'inherit'}>
                                            {faq.answer}
                                        </Typography>
                                    </AccordionDetails>
                                </Accordion>
                            ))}
                        </Stack>
                    </Stack>
                </Stack>
            )}
        </>
    );
}

export default AboutCoursePage;
