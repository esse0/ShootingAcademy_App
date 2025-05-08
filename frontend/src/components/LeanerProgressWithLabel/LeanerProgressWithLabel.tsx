import LinearProgress, { LinearProgressProps } from '@mui/material/LinearProgress';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';

export default function LinearProgressWithLabel(props: LinearProgressProps & { value: number }) {
  return (
    
    <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
       <Box sx={{ minWidth: 130 }}>
        <Typography
          variant="body2"
          fontFamily={'inherit'}
          sx={{ color: 'black' }}
        >{`${Math.round(props.value)}%`} completed</Typography>
      </Box>
      <Box sx={{ width: '100%', ml: 1 }}>
        <LinearProgress variant="determinate" {...props} />
      </Box>
    </Box>
  );
}