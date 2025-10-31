import { useState } from 'react';
import { useNavigate } from 'react-router';
import axios, { AxiosError, AxiosResponse } from 'axios';
import { useSnackbar } from 'notistack';

interface ServerError {
    Error: boolean;
    Show: boolean;
    Message: string;
    Code: string;
}

interface UseApiReturn<T> {
    resData: T | null;
    setResData: React.Dispatch<React.SetStateAction<T | null>>;
    loading: boolean;
    execute: (body?: any) => Promise<AxiosResponse<T> | undefined>;
    statusCode: number | null;
    setStatusCode: React.Dispatch<React.SetStateAction<number | null>>;
    error: any;
}

export function useApi<T, D = undefined>(request: (data?: D) => Promise<AxiosResponse<T>>): UseApiReturn<T> {
    const [resData, setData] = useState<T | null>(null);
    const [statusCode, setStatusCode] = useState<number | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<any>(null);
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();

    const refreshTokens = async () => {
        try {
            await axios.post("/api/auth/refresh");
            return true;
        } catch (err) {
            console.error("Ошибка при обновлении токена", err);
            return false;
        }
    };

    const execute = async (body?: D) => {
        setLoading(true);
        setError(null);
        try {
            const response: AxiosResponse<T> = await request(body);
            setData(response.data);
            setStatusCode(response.status);
            return response;
        } catch (err) {
            const error = err as AxiosError<ServerError>;
            let errorMessage = 'Неизвестная ошибка';

            setData(null);
            setError(err);

            if (error.response?.status === 401) {
                const refreshed = await refreshTokens();
                if (refreshed) {
                    try {
                        const retryResponse: AxiosResponse<T> = await request(body);
                        setData(retryResponse.data);
                        setStatusCode(retryResponse.status);
                        return retryResponse;
                    } catch (retryErr) {
                        navigate('/signin');
                        return;
                    }
                } else {
                    navigate('/signin');
                    return;
                }
            }

            if (error.response) {
                const serverError = error.response.data;

                if (serverError && serverError.Show === true) {
                    enqueueSnackbar(serverError.Message, { variant: 'error' });
                    return;
                }

                errorMessage = serverError.Message || 'Отсутствует сообщение об ошибке';
                console.error('Ошибка ответа сервера:', errorMessage);

                navigate(
                    `/error?message=${encodeURIComponent(errorMessage)}&statusCode=${
                        error.response.status
                    }&statusText=${error.response.statusText}`,
                );
            } else if (error.request) {
                errorMessage = 'Сервер не ответил. Проверьте подключение к сети.';
                console.error(error.request);
                navigate(`/error?message=${encodeURIComponent(errorMessage)}`);
            } else {
                errorMessage = error.message || 'Ошибка запроса';
                console.error('Ошибка запроса:', errorMessage);
                navigate(`/error?message=${encodeURIComponent(errorMessage)}`);
            }
        } finally {
            setLoading(false);
        }
    };

    return { resData, setResData: setData, loading, execute, statusCode, setStatusCode, error };
}
