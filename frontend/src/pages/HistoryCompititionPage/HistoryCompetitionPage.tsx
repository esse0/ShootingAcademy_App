import { List, ListItem, TextField, Typography } from '@mui/material';
import { CompetitionBaner } from '../../components/CompetitionBaner/CompetitionBaner';
import SearchIcon from '@mui/icons-material/Search';
import { CompetitionType } from '../../types/CompetitionBanerType';
import { useEffect, useState } from 'react';
import { useApi } from '../../hooks/useApi';
import axios from 'axios';

function HistoryCompetitionPage() {
    const [searchText, setSearchText] = useState('');
    const [filteredMyCompetitions, setFilteredMyCompetitions] = useState<
        CompetitionType[]
    >([]);

    const { resData: RecivedMyCompetitions, execute } = useApi<
        CompetitionType[]
    >(async () => {
        return axios.get('/api/competition/user', {
            params: { history: true },
        });
    });

    useEffect(() => {
        if (!RecivedMyCompetitions) return;

        if (searchText) {
            setFilteredMyCompetitions(
                RecivedMyCompetitions.filter((RecivedMyCompetitions) =>
                    RecivedMyCompetitions.title
                        .toLowerCase()
                        .includes(searchText.toLowerCase()),
                ),
            );
        } else {
            setFilteredMyCompetitions(RecivedMyCompetitions);
        }
    }, [searchText, RecivedMyCompetitions]);
    
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
            {filteredMyCompetitions.length > 0 ? (
                <List sx={{ width: '100%' }}>
                    {filteredMyCompetitions.map((mycourse, index) => (
                        <ListItem key={index} sx={{ pl: 0, pr: 0 }}>
                            <CompetitionBaner {...mycourse} />
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
                    No competitions
                </Typography>
            )}
        </>
    );
}

export default HistoryCompetitionPage;
