import { SxProps, Theme } from "@mui/material";

export type StatisticByWeaphonType = {
    month: string;
    avgresult: number;
}

export type StatisticHorizontalBarProps = {
    sx?: SxProps<Theme>;
    dataset: Array<StatisticByWeaphonType>
}

export type CoursesByCategoryData = {
    id: string;
    label: string;
    value: number;
}

export type AnalyticsData = {
    completedCoursesCount: number;
    completedLessonsCount: number;
    completedCompetitions: number;
    coursesByCategory: Array<CoursesByCategoryData>;
    scoreByMonth: StatisticHorizontalBarProps;
}