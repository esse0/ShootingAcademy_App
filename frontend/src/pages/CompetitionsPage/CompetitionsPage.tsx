import { List, ListItem, TextField, Typography } from "@mui/material"
import { useEffect, useState } from "react";
import { CompetitionType } from "../../types/CompetitionBanerType";
import { CompetitionBaner } from "../../components/CompetitionBaner/CompetitionBaner";
import axios from "axios";
import { useApi } from "../../hooks/useApi";

export default function CompetitionsPage() {

   const [search, setSearch] = useState("");
   const [competitions, setCompetitions] = useState<CompetitionType[]>([]);

   const {resData: RecivedCompetitions, execute: executeGetCompetitions} = useApi<CompetitionType[]>(async ()=>{
    return axios.get('/api/competition');
   })

   useEffect(() => {
     if(!RecivedCompetitions) return;
    
     if(search){
      setCompetitions(RecivedCompetitions.filter((competition) => competition.title.toLowerCase().includes(search.toLowerCase())));
     }else{
      setCompetitions(RecivedCompetitions);
     }    
   }, [RecivedCompetitions, search]);
  

   useEffect(()=>{
    executeGetCompetitions();
   }, [])

   return (
      <>
        <TextField
            sx={{ width: '100%', marginBottom: '16px'}}
            label="Search Competitions"
            variant="outlined"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />

        { competitions.length > 0 ? <List sx={{ width: '100%'}}>
          {competitions?.map((competition, index) => (
            <ListItem key={index} sx={{ padding: '0px'}}>
               <CompetitionBaner {...competition}/>
            </ListItem>
         ))}
        </List> :
          <Typography variant="h4" color="text.secondary" textAlign="center" mt="30vh">No competitions</Typography>
        }
      </>
    )
}