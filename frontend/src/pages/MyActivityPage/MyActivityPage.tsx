import { Box, Tab, Tabs } from "@mui/material";
import React from "react";
import a11yProps from "../../types/A11yProps";
import CustomTabPanel from "../../components/TabPanel/TabPanel";
import AnalyticsPage from "../AnalyticsPage/AnalyticsPage";
import MyCoursesPage from "../MyCoursesPage/MyCoursesPage";
import MyCompetitionsPage from "../MyCompetitionsPage/MyCompetitionsPage";
import HistoryCompetitionPage from "../HistoryCompititionPage/HistoryCompetitionPage";
import HistoryCoursesPage from "../HistoryCoursesPage/HistoryCoursesPage";

function MyActivityPage() {
    const [value, setValue] = React.useState(0);
    const handleChange = (_event: React.SyntheticEvent, newValue: number) => {
      setValue(newValue);
    };

    return (
      <>
        <Box sx={{ width: '100%' }}>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs TabIndicatorProps={{style: {backgroundColor: "#455CC7"}}} sx={{"&& .Mui-selected": {color: "#455CC7"}}} value={value} onChange={handleChange}>
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}} label="Analytics" {...a11yProps(0)} />
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}} label="My courses" {...a11yProps(1)} />
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}} label="My competitions" {...a11yProps(2)} />
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}}  label="History Courses" {...a11yProps(3)} />
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}}  label="History Competitions" {...a11yProps(4)} />
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}}  label="Schedule" {...a11yProps(5)} />
            </Tabs>
          </Box>
          <CustomTabPanel value={value} index={0}>
            <AnalyticsPage />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={1}>
            <MyCoursesPage />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={2}>
            <MyCompetitionsPage />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={3}>
            <HistoryCoursesPage />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={4}>
            <HistoryCompetitionPage />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={5}>
            <></>
          </CustomTabPanel>
        </Box>
      </>
    )
  }
  
  export default MyActivityPage
  