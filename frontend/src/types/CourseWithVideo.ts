import { faqsType, FeaturesType } from "./CourseTypes";
import { FullUserModel } from "./UserProfileData";

export interface LessonTypeWithVideo {
    id: string;
    title: string;
    description: string
    videoUri?: string;
    isCompleted: boolean;
}

export default interface ModulesTypeWithVideo {
    id: string;
    title: string;
    lessons: LessonTypeWithVideo[];
}

export type CourseTypeWithVideo = {
    id: string;
    title: string;
    description: string;
    duration: string;
    level: string;
    rate: number;
    peopleRateCount: number;
    is_closed: boolean;
    instructor: FullUserModel;
    modules: Array<ModulesTypeWithVideo>;
    features: Array<FeaturesType>;
    faqs: Array<faqsType>;
    category: string;
};