import { FullUserModel } from "./UserProfileData";



export type faqsType = {
    id: string;
    question: string;
    answer: string;
};
export type FeaturesType = {
    id: string;
    title: string;
    description: string;
};

export type CourseType = {
    id: string;
    title: string;
    description: string;
    duration: string;
    level: string;
    rate: number;
    peopleRateCount: number;
    is_closed: boolean;
    instructor: FullUserModel;
    modules: Array<ModulesType>;
    features: Array<FeaturesType>;
    faqs: Array<faqsType>;
    category: string;
};


export type CreateCourseType = {
    id: string;
    title: string;
    description: string;
    duration: string;
    level: string;
    rate: number;
    category: string;
    modules: Array<ModulesType>;
    features?: Array<FeaturesType>;
    faqs?: Array<faqsType>;
};

export default interface ModulesType {

    id: string;
    title: string;
    lessons: LessonType[];
}

export interface LessonType {
    id: string;
    title: string;
    description: string
    videoId?: string;
}