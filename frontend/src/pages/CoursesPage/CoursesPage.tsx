import {
    Button,
    Checkbox,
    checkboxClasses,
    FormControlLabel,
    FormGroup,
    List,
    ListItem,
    Menu,
    Stack,
    SxProps,
    TextField,
    Theme,
    Typography,
} from '@mui/material';
import Course from '../../components/Course/Course';
import CourseBannerType from '../../types/CourseBannerType';
import SearchIcon from '@mui/icons-material/Search';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import React, { useEffect, useState } from 'react';
import { CategoryButton } from '../../components/CategoryButton/CategoryButton';
import axios from 'axios';
import { useApi } from '../../hooks/useApi';
import { tagGroups } from '../../constants/TagGroups';

function CoursesPage({
    sx,
    courseLink,
}: {
    sx?: SxProps<Theme>;
    courseLink: string;
}) {
    const [searchText, setSearchText] = useState('');
    const [selectedFilters, setSelectedFilters] = useState<
        Record<string, Set<string>>
    >({});
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const [selectedCategory, setSelectedCategory] = React.useState<
        string | null
    >(null);
    const [categories, setCategories] = useState<string[]>();

    const { resData: RecivedCourses, execute } = useApi<CourseBannerType[]>(
        async () => {
            return axios.get('/api/course');
        },
    );

    useEffect(() => {
        execute();
    }, []);

    const [courses, setCourses] = useState<CourseBannerType[]>([]);

    const filterMenuOpen = Boolean(anchorEl);

    const handleOpenFilterMenu = (
        event: React.MouseEvent<HTMLButtonElement>,
    ) => {
        setAnchorEl(event.currentTarget);
    };

    const handleCloseFilterMenu = () => {
        setAnchorEl(null);
    };

    const handleCategoryClick = (category: string) => {
        setSelectedCategory((prev) => (prev === category ? null : category));
    };

    const handleCheckboxChange = (group: string, tag: string) => {
        setSelectedFilters((prev) => {
            const updated = new Map(Object.entries(prev));
            const groupSet = new Set(updated.get(group) || []);

            if (groupSet.has(tag)) {
                groupSet.delete(tag);
            } else {
                groupSet.add(tag);
            }

            if (groupSet.size === 0) {
                updated.delete(group);
            } else {
                updated.set(group, groupSet);
            }

            return Object.fromEntries(updated);
        });
    };

    useEffect(() => {
        if (!RecivedCourses) return;

        if (RecivedCourses.length > 0) {
            setCategories([
                ...new Set(RecivedCourses.map((course) => course.category)),
            ]);
        }

        const filteredCourses = RecivedCourses.filter(
            (course) =>
                // Фильтрация по поисковому запросу
                !searchText ||
                course.title.toLowerCase().includes(searchText.toLowerCase()),
        )
            .filter(
                (course) =>
                    // Фильтрация по категории
                    !selectedCategory ||
                    (categories?.includes(selectedCategory) &&
                        course.category === selectedCategory),
            )
            .filter((course) =>
                // Фильтрация по выбранным тегам
                Object.entries(selectedFilters).every(([group, tags]) => {
                    if (group === 'Level') return tags.has(course.level);
                    if (group === 'Duration') return tags.has(course.duration);
                    return true;
                }),
            );

        setCourses(filteredCourses);
    }, [searchText, selectedCategory, selectedFilters, RecivedCourses]);

    return (
        <Stack sx={sx} className="main">
            <Stack gap="20px">
                <Typography
                    variant="h4"
                    fontFamily={'inherit'}
                    fontWeight="bold"
                >
                    Explore Our Courses
                </Typography>
                <Typography
                    variant="body1"
                    fontFamily={'inherit'}
                    fontWeight="500"
                >
                    Search for a course
                </Typography>
            </Stack>

            <Stack direction={'row'} gap={'10px'} mt={'30px'} mb={'30px'}>
                <div style={{ position: 'relative', width: '50%' }}>
                    <SearchIcon
                        style={{
                            position: 'absolute',
                            left: 10,
                            top: 10,
                            width: 20,
                            height: 20,
                        }}
                    />
                    <TextField
                        fullWidth
                        inputMode="search"
                        placeholder="Search"
                        size="small"
                        onChange={(e) => setSearchText(e.target.value)}
                        sx={{
                            '& .MuiInputBase-root:hover .MuiOutlinedInput-notchedOutline':
                                {
                                    borderColor: 'black !important',
                                },
                            '& .MuiInputBase-root.Mui-focused .MuiOutlinedInput-notchedOutline':
                                {
                                    borderColor: '#455CC7 !important',
                                },
                        }}
                        slotProps={{
                            input: {
                                sx: {
                                    pl: '25px',
                                },
                            },
                        }}
                    />
                </div>
                <Button
                    aria-controls={filterMenuOpen ? 'basic-menu' : undefined}
                    aria-haspopup="true"
                    aria-expanded={filterMenuOpen ? 'true' : undefined}
                    onClick={handleOpenFilterMenu}
                    variant="contained"
                    sx={{
                        bgcolor: '#455CC7',
                        fontFamily: 'inherit',
                        fontSize: '12px',
                        width: '120px',
                        textDecoration: 'none',
                    }}
                >
                    <Stack direction={'row'} gap={'10px'}>
                        <FilterAltIcon />
                        <Typography fontFamily={'inherit'} variant="body1">
                            Filter
                        </Typography>
                    </Stack>
                </Button>
                <Menu
                    sx={{ fontFamily: 'var(--primary-font)' }}
                    id="basic-menu"
                    anchorEl={anchorEl}
                    open={filterMenuOpen}
                    onClose={handleCloseFilterMenu}
                >
                    {tagGroups.map((tagGroup, index) => (
                        <Stack key={index}>
                            <Typography
                                p={'2px 5px'}
                                fontFamily={'inherit'}
                                variant="body1"
                                fontWeight={'600'}
                                color="var(--secondary-color)"
                            >
                                {tagGroup.tagGroupTitle}
                            </Typography>
                            <FormGroup>
                                {tagGroup.tags.map((tag, index) => (
                                    <FormControlLabel
                                        key={index}
                                        sx={{
                                            m: 0,
                                            pr: '10px',
                                            width: '100%',
                                            userSelect: 'none',
                                        }}
                                        control={
                                            <Checkbox
                                                sx={{
                                                    [` &.${checkboxClasses.checked}`]:
                                                        {
                                                            color: 'var(--accent-color)',
                                                        },
                                                }}
                                                checked={
                                                    selectedFilters[
                                                        tagGroup.tagGroupTitle
                                                    ]?.has(tag) || false
                                                }
                                                onChange={() =>
                                                    handleCheckboxChange(
                                                        tagGroup.tagGroupTitle,
                                                        tag,
                                                    )
                                                }
                                            />
                                        }
                                        label={
                                            <Typography
                                                fontFamily={'inherit'}
                                                variant="body1"
                                                fontWeight={'400'}
                                            >
                                                {tag}
                                            </Typography>
                                        }
                                    />
                                ))}
                            </FormGroup>
                        </Stack>
                    ))}
                </Menu>
            </Stack>

            <Stack height={'100vh'} flexDirection={'row'}>
                <Stack flex={0.7} pr={'15px'} gap={'20px'}>
                    {courses.map((course, index) => (
                        <Course
                            courseLink={courseLink}
                            key={index}
                            course={course}
                        />
                    ))}
                </Stack>
                <Stack flex={0.3} sx={{ borderLeft: '1px solid #D8D8D9' }}>
                    <Typography
                        p={'0 15px 15px 15px'}
                        variant="h5"
                        fontFamily={'inherit'}
                        fontWeight={'500'}
                    >
                        Categories
                    </Typography>

                    <List>
                        {categories?.map((category, index) => (
                            <ListItem key={index}>
                                <CategoryButton
                                    sx={{
                                        width: '100%',
                                        fontFamily: 'inherit',
                                        fontWeight: '500',
                                    }}
                                    label={category}
                                    isSelected={selectedCategory === category}
                                    onClick={() =>
                                        handleCategoryClick(category)
                                    }
                                />
                            </ListItem>
                        ))}
                    </List>
                </Stack>
            </Stack>
        </Stack>
    );
}

export default CoursesPage;
