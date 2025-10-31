import { useEffect, useState } from 'react';
import { useAtom } from 'jotai';
import { notificationsAtom } from '../jotai/atoms';
import { useApi } from './useApi';
import axios from 'axios';
import { Notification } from '../types/NotificationType';

export const useNotifications = (pageSize: number = 5) => {
    const [allNotifications, setAllNotifications] = useAtom(notificationsAtom);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    const { execute: fetchNotifications, resData: notificationsData } = useApi<Notification[]>(async () => {
        return axios.get('/api/notification');
    });

    useEffect(() => {
        fetchNotifications();
    }, []);

    useEffect(() => {
        if (notificationsData) {
            setAllNotifications(notificationsData);
            setTotalPages(Math.ceil(notificationsData.length / pageSize));
        }
    }, [notificationsData, setAllNotifications, pageSize]);

    // Получаем уведомления для текущей страницы
    const notifications = allNotifications.slice(
        (page - 1) * pageSize,
        page * pageSize
    );

    const nextPage = () => {
        if (page < totalPages) {
            setPage(prev => prev + 1);
        }
    };

    const previousPage = () => {
        if (page > 1) {
            setPage(prev => prev - 1);
        }
    };

    return {
        notifications,
        currentPage: page,
        totalPages,
        hasNextPage: page < totalPages,
        hasPreviousPage: page > 1,
        nextPage,
        previousPage,
        refreshNotifications: fetchNotifications,
        setPage,
        allNotifications
    };
}; 