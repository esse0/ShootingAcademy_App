import { List, ListItem, TextField, Typography } from '@mui/material';
import MyCourseBannerType from '../../types/MyCourseBannerType';
import SearchIcon from '@mui/icons-material/Search';
import MyCourse from '../../components/MyCourse/MyCourse';
import { useEffect, useState } from 'react';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';

function HistoryCoursesPage() {
    const [searchText, setSearchText] = useState('');
    const [filteredMyCourses, setFilteredMyCourses] = useState<
        MyCourseBannerType[]
    >([]);

    const { resData: RecivedMyCourses, execute } = useApi<MyCourseBannerType[]>(
        async () => {
            return axios.get('/api/course/user', { params: { history: true } });
        },
    );

    useEffect(() => {
        if (!RecivedMyCourses) return;
        if (searchText) {
            setFilteredMyCourses(
                RecivedMyCourses.filter((RecivedMyCourses) =>
                    RecivedMyCourses.title
                        .toLowerCase()
                        .includes(searchText.toLowerCase()),
                ).filter((course) => course.is_closed == true),
            );
        } else {
            setFilteredMyCourses(
                RecivedMyCourses.filter((course) => course.is_closed == true),
            );
        }
    }, [searchText, RecivedMyCourses]);

    useEffect(() => {
        execute();
    }, []);

    return (
        <>
            <div
                style={{
                    position: 'relative',
                    display: 'inline-block',
                    margin: '0 0 10px 0',
                    width: '330px',
                }}
            >
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
                    onChange={(e) => setSearchText(e.target.value)}
                    size="small"
                    slotProps={{
                        input: {
                            sx: {
                                pl: '25px',
                            },
                        },
                    }}
                />
            </div>
            {filteredMyCourses.length > 0 ? (
                <List sx={{ width: '100%' }}>
                    {filteredMyCourses
                        .filter((mycourse) => mycourse.is_closed)
                        .map((mycourse, index) => (
                            <ListItem key={index} sx={{ pl: 0, pr: 0 }}>
                                <MyCourse {...mycourse} />
                            </ListItem>
                        ))}
                </List>
            ) : (
                <Typography
                    variant="h4"
                    color="text.secondary"
                    textAlign="center"
                    mt="30vh"
                >
                    No closed courses
                </Typography>
            )}
        </>
    );
}

export default HistoryCoursesPage;
