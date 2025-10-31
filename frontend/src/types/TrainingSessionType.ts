export interface TrainingSessionDTO {
    id: string;
    trainerId: string;
    trainerName: string;
    groupId: string;
    groupName: string;
    rangeId: string;
    rangeName: string;
    rangeLocation: string;
    status: string;
    startDate: string;
    endDate: string;
}

export interface CreateTrainingSessionDTO {
    groupId: string;
    rangeId: string;
    status: string;
    startDate: string;
    endDate: string;
}

export interface UpdateTrainingSessionDTO {
    status?: string;
    startDate?: string;
    endDate?: string;
}

export interface GroupDTO {
    id: string;
    name: string;
}

export interface RangeDTO {
    id: string;
    name: string;
    location: string;
} 