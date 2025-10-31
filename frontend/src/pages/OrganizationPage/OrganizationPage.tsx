import { Box, Tab, Tabs } from "@mui/material";
import CustomTabPanel from "../../components/TabPanel/TabPanel";
import a11yProps from "../../types/A11yProps";
import OrganizationInfoPage from "./OrganizationInfoPage";
import OrganizationMembersPage from "./OrganizationMembersPage";
import OrganizationRangesPage from "./OrganizationRangesPage";
import React from "react";

function OrganizationPage(){
    const [value, setValue] = React.useState(0);
    const handleChange = (_event: React.SyntheticEvent, newValue: number) => {
      setValue(newValue);
    };
  
  return (
    <>
      <Box sx={{ width: '100%' }}>
          <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
            <Tabs TabIndicatorProps={{style: {backgroundColor: "#455CC7"}}} sx={{"&& .Mui-selected": {color: "#455CC7"}}} value={value} onChange={handleChange}>
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}} label="Info" {...a11yProps(0)} />
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}} label="Members" {...a11yProps(1)} />
              <Tab sx={{fontFamily: 'var(--primary-font)', textTransform: 'none', fontSize:"1em"}} label="Ranges" {...a11yProps(2)} />
            </Tabs>
          </Box>
          <CustomTabPanel value={value} index={0}>
            <OrganizationInfoPage/>
          </CustomTabPanel>
          <CustomTabPanel value={value} index={1}>
            <OrganizationMembersPage />
          </CustomTabPanel>
          <CustomTabPanel value={value} index={2}>
            <OrganizationRangesPage />
          </CustomTabPanel>
        </Box>
    </>
  );
}

export default OrganizationPage;