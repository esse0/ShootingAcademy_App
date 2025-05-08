import Box from '@mui/material/Box';
import TabPanelProps from '../../types/TabPanelPropType';


export default function CustomTabPanel(props: TabPanelProps) {

  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ pt:"20px"}}>{children}</Box>}
    </div>
  );
}

