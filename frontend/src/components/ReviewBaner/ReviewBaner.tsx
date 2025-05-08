import { Card, Rating, SxProps, Theme, Typography } from "@mui/material"
import ReviewBanerProps from "../../types/ReviewBanerProps"

function ReviewBaner(props: {item: ReviewBanerProps, sx: SxProps<Theme> | undefined}) {

    return (
        <Card sx={{...props.sx, display: "flex", flexDirection: "column", gap: "5px", bgcolor: "#F8F8F8", borderRadius: "10px", padding: "15px", boxShadow: "none", fontFamily: "var(--primary-font)"}}>
            <Typography variant="body1" fontWeight={"bold"} fontFamily={"inherit"}>{props.item.user}</Typography>
            <Rating  defaultValue={props.item.rate} precision={0.1} readOnly />
            <Typography fontSize="15px" fontWeight="500" fontFamily={"inherit"}>{props.item.comment}</Typography>
        </Card>
    )
  }
  
  export default ReviewBaner
  