import { useApi } from "../hooks/useApi";
import { TrainingSessionDTO, CreateTrainingSessionDTO, UpdateTrainingSessionDTO, GroupDTO, RangeDTO } from "../types/TrainingSessionType";
import axios from "axios";

// Получение списка групп тренера
export const useGetCoachGroups = () => {
    return useApi<GroupDTO[]>(() => axios.get("/api/trainingsession/groups"));
};

// Получение списка доступных помещений
export const useGetAvailableRanges = () => {
    return useApi<RangeDTO[]>(() => axios.get("/api/trainingsession/ranges"));
};

// Создание тренировки
export const useCreateTrainingSession = () => {
    return useApi<TrainingSessionDTO, CreateTrainingSessionDTO>(
        (data) => axios.post("/api/trainingsession", data)
    );
};

// Обновление тренировки
export const useUpdateTrainingSession = () => {
    return useApi<TrainingSessionDTO, { id: string; data: UpdateTrainingSessionDTO }>(
        ({ id, data } = {} as { id: string; data: UpdateTrainingSessionDTO }) => axios.put(`/api/trainingsession/${id}`, data)
    );
};

// Удаление тренировки
export const useDeleteTrainingSession = () => {
    return useApi<void, string>(
        (id) => axios.delete(`/api/trainingsession/${id}`)
    );
};

// Получение расписания
export const useGetSchedule = () => {
    return useApi<TrainingSessionDTO[], { coachId?: string; athleteId?: string }>(
        (data) => axios.post("/api/trainingsession/schedule", data)
    );
};

// Получение расписания пользователя
export const useGetUserSchedule = () => {
    return useApi<TrainingSessionDTO[]>(() => axios.get("/api/trainingsession/user-schedule"));
}; 