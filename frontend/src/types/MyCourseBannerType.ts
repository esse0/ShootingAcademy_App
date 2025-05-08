export default interface MyCourseBannerType {
    id: string;
    title: string;
    duration: string;
    level: string;
    completed_percent: number;
    is_closed: boolean;
    started_at: string;
    finished_at?: string;
};