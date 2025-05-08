import { Card, CardContent, SxProps, Theme, Typography } from "@mui/material";
import { PieChart, PieValueType } from "@mui/x-charts";
import { MakeOptional } from "@mui/x-charts/internals";

function StatisticPieCard({title, sx, data}: {sx?: SxProps<Theme>, title: string, data: MakeOptional<PieValueType, "id">[]}){
    return (
        <Card sx={sx} variant="outlined">
            <CardContent sx={{display:"flex", flexDirection:"column", gap:"15px", alignItems: "center", '&:last-child': { pb: "16px" }}}>
                <Typography variant="h6" fontFamily="var(--primary-font)" fontWeight={"500"}>{title}</Typography>
                <PieChart  
                    margin={{ bottom: 100, left: 100, right:100 }}
                    
                    series={[
                        {
                            data,
                            innerRadius: 90,
                        }
                    ]}
                    slotProps={{
                        legend: {
                            labelStyle: {
                                fontSize: 16,
                                fontFamily: 'var(--primary-font)',
                                fontWeight: 500
                            },
                            position: { vertical: 'bottom', horizontal: 'middle' },
                            padding: 0,
                            itemMarkWidth: 8,
                            itemMarkHeight: 8,
                            markGap: 8,
                            itemGap: 16,
                        },
                      }}
                    width={500}
                    height={350}
                />
            </CardContent>
        </Card>
    );
}

export default StatisticPieCard