import {  Card, CardContent } from "@mui/material";
import { BarChart } from "@mui/x-charts";
import { StatisticHorizontalBarProps } from "../../types/StatisticTypes";

function StatisticTableCard(props: StatisticHorizontalBarProps){

    const chartSetting = {
        xAxis: [
          {
            label: 'Avg competition results',
          },
        ],
        width: 890,
        height: 400,
    };   


    return (
        <Card sx={props.sx}  variant="outlined">
            <CardContent sx={{display:"flex", flexDirection:"row", gap:"15px", alignContent:"center", '&:last-child': { pb: "16px" }}}>
                <BarChart
                        dataset={props.dataset}
                        yAxis={[{ scaleType: 'band', dataKey: 'month' }]}
                        series={[{ dataKey: 'avgresult', label: 'Avg result' }]}
                        layout="horizontal"
                        {...chartSetting}
                />
            </CardContent>
        </Card>
    );
}

export default StatisticTableCard;