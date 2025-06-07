import { useState } from 'react';
import { useNavigate } from 'react-router';
import axios, { AxiosError, AxiosResponse } from 'axios';

interface ServerError {
    error: boolean;
    show: boolean;
    message: string;
    code: string;
}

export function useApi<T, D = undefined>(request: (data?: D) => Promise<AxiosResponse<T>>) {
    const [resData, setData] = useState<T | null>(null);
    const [statusCode, setStatusCode] = useState<number | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const navigate = useNavigate();

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
        try {
            const response: AxiosResponse<T> = await request(body);
            setData(response.data);
            setStatusCode(response.status);
            return response;
        } catch (err) {
            const error = err as AxiosError<ServerError>;
            let errorMessage = 'Неизвестная ошибка';

            setData(null);

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

                if (serverError && serverError.show === true) {
                    alert(serverError.message);
                    return;
                }

                errorMessage = serverError.message || 'Отсутствует сообщение об ошибке';
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

    return { resData, setResData: setData, loading, execute, statusCode, setStatusCode };
}
