import { Avatar, Card, CardContent, Stack, SxProps, Theme, Typography } from "@mui/material"
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import TrendingDownIcon from '@mui/icons-material/TrendingDown';
import TrendingFlatIcon from '@mui/icons-material/TrendingFlat';

function StatisticCard({title, value, fontFamily, bgIconColor, icon, last7DaysPercent, sx}: {title: string, value: string, last7DaysPercent: number, fontFamily?: string, bgIconColor?: string, icon?: JSX.Element, sx?: SxProps<Theme>}){ {
    const trandColor = (last7DaysPercent: number) => {
        if(last7DaysPercent > 0){
            return "green"
        }
        else if(last7DaysPercent < 0){
            return "red"
        }
        return "orange"
    }

    return (
        <Card sx={sx} variant="outlined">
             <CardContent sx={{display:"flex", flexDirection:"column", gap:"15px", '&:last-child': { pb: "16px" }}}>
                <Stack gap={"5px"} direction="row" alignItems={"center"}>
                    <Avatar sx={{ bgcolor: bgIconColor ? bgIconColor : "gray", width: "32px", height: "32px" }} variant="rounded">
                       {icon}
                    </Avatar>
                    <Typography color="text.secondary" fontFamily={fontFamily} variant="body2" fontWeight={"500"}>{title}</Typography>
                </Stack>
                <Stack justifyContent={"space-between"} direction="row" alignContent={"center"}>
                    <Typography color="black" fontFamily={fontFamily} variant="h4" fontWeight={"bold"}>{value}</Typography>
                    <Stack>
                        <Stack direction="row" color={trandColor(last7DaysPercent)}>
                            {last7DaysPercent > 0 && 
                                <TrendingUpIcon sx={{height: "16px", width: "16px"}}/>
                            }
                            {last7DaysPercent < 0 && 
                                <TrendingDownIcon sx={{height: "16px", width: "16px"}}/>
                            }
                            {last7DaysPercent == 0 && 
                                <TrendingFlatIcon sx={{height: "16px", width: "16px"}}/>
                            }
                            <Typography fontFamily={fontFamily} fontSize={"12px"} fontWeight={"600"}>{last7DaysPercent}%</Typography>
                        </Stack>
                        <Typography color="text.secondary" fontFamily={fontFamily} fontSize={"12px"} fontWeight={"600"}>vs last 7 days</Typography>
                    </Stack>
                </Stack>
             </CardContent>
        </Card>
    )
  }
}
  
export default StatisticCard