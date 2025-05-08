import { Stack, Typography } from "@mui/material"
import StatisticCard from "../../components/StatisticCard/StatisticCard"
import EmojiObjectsIcon from '@mui/icons-material/EmojiObjects';
import WhatshotIcon from '@mui/icons-material/Whatshot';
import EmojiEventsIcon from '@mui/icons-material/EmojiEvents';
import StatisticPieCard from "../../components/StatisticPieCard/StatisticPieCard";
import StatisticHorizontalBar from "../../components/StatisticHorizontalBar/StatisticHorizontalBar";
import { AnalyticsData } from "../../types/StatisticTypes";
import axios from "axios";
import { useApi } from "../../hooks/useApi";
import { useEffect, useState } from "react";


function AnalyticsPage() {

   const [analytics, setAnalytics] = useState<AnalyticsData>();

   const {resData: RecivedAnalytics, execute: executeGetAnalytics} = useApi<AnalyticsData>(async ()=>{
      return axios.get('/api/analytic');
    })

   useEffect(()=>{
      executeGetAnalytics();
   }, [])
    
   useEffect(()=>{
      if(RecivedAnalytics){
         setAnalytics(RecivedAnalytics);
      }
   }, [RecivedAnalytics])
   
   return (
       <Stack gap={"20px"}>
         <Typography variant="h4" fontFamily="var(--primary-font)" fontWeight={"bold"}>Overview</Typography>

         <Stack flexWrap={"wrap"} direction="row" gap={"20px"}>
            <StatisticCard sx={{ minWidth: "300px", flex: 1}} icon={<WhatshotIcon sx={{color: "#8C57FF"}}/>} last7DaysPercent={1} bgIconColor="#E3DCFB" fontFamily="var(--primary-font)" title="Courses ended" value={`${analytics?.completedCoursesCount ?? 0}`}/>
            <StatisticCard sx={{ minWidth: "300px", flex: 1}} icon={<EmojiObjectsIcon sx={{color: "#16B1FF"}}/>} last7DaysPercent={1} bgIconColor="#D0EBFB" fontFamily="var(--primary-font)" title="Lessons passed" value={`${analytics?.completedLessonsCount ?? 0}`}/>
            <StatisticCard sx={{ minWidth: "300px", flex: 1}} icon={<EmojiEventsIcon sx={{color: "#FFB400"}}/>} last7DaysPercent={1} bgIconColor="#F6EBD2" fontFamily="var(--primary-font)" title="Competitions completed" value={`${analytics?.completedCompetitions ?? 0}`}/>
         </Stack>

         <Stack flexWrap={"wrap"} direction="row" gap={"20px"} justifyContent={"space-between"}>
            <StatisticHorizontalBar dataset={analytics?.scoreByMonth?.dataset ?? []} sx={{flex: 3}}/>
            <StatisticPieCard sx={{flex: 2}} title="Courses completed by category" data={analytics?.coursesByCategory ?? []}/>
         </Stack>
       </Stack>
    )
  }
  
  export default AnalyticsPage
  