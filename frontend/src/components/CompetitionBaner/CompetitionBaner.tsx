import { Button,  Chip,  Paper, Stack, Typography } from "@mui/material";
import { CompetitionType } from "../../types/CompetitionBanerType";
import { Link } from "react-router";

export function CompetitionBaner(props: CompetitionType){
    const {
        id,
        title,
        date,
        time,
        organiser,
        memberCount,
        maxMemberCount,
        venue,
        country,
        city,
        exercise,
        status
      } = props;

    return(
      <Paper
          elevation={0}
          sx={{
          bgcolor: "#F9F9F9",
          width: "100%",
          padding: "20px",
          borderRadius: "0.5em",
          marginBottom: "20px",
          fontFamily: "var(--primary-font)",
        }}
      >
        <Stack flexDirection="row">
          <Stack width="100%" gap="8px">
            
            <Stack flexDirection="row" justifyContent="space-between">
              <Typography fontFamily="inherit" variant="h5" fontWeight="bold">
                  {title}
              </Typography>

              <Stack flexDirection="row" alignItems={"center"} gap={"10px"}>
                <Typography fontFamily="inherit" variant="body1" fontWeight="bold">
                    Status:
                </Typography>
                {status === "Active" && <Chip sx={{width:"100px", fontFamily: "var(--primary-font)"}} label="Active" color="success" />}
                {status === "Pending" && <Chip sx={{width:"100px", fontFamily: "var(--primary-font)"}} label="didn't start" color="warning" />}
                {status === "Ended" && <Chip sx={{width:"100px", fontFamily: "var(--primary-font)"}} label="Ended" color="error" />}
              </Stack>
            </Stack>
            

            <Typography fontFamily="inherit" variant="body2" color="textSecondary">
              {date} at {time}
            </Typography>

            <Typography fontFamily="inherit" variant="body2">
              Member count: {memberCount} / {maxMemberCount}
            </Typography>

            <Typography fontFamily="inherit" variant="body2">
              Organiser: {organiser}
            </Typography>

            <Typography fontFamily="inherit" variant="body2">
              Venue: {venue}, {city}, {country}
            </Typography>

            <Stack flexDirection="row" justifyContent="space-between">
              <Stack>
                <Typography fontFamily="inherit" variant="body2">Exercises:</Typography>
                <ul>
                    <li ><Typography fontFamily="inherit" variant="body2">{exercise}</Typography></li>
                </ul>
              </Stack>
              
              <Button  variant="contained" to={`/app/competitions/${id}`} component={Link} sx={{ borderBlockColor: '#455CC7', background: '#455CC7', fontFamily: "inherit", height: '40px', width: '100px', alignSelf: 'flex-end'}}>
                  View
              </Button>
            </Stack>
          </Stack>
        </Stack>
      </Paper>
    )
}