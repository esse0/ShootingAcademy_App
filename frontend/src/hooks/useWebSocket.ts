import { useEffect, useRef } from 'react';
import { useSnackbar } from 'notistack';
import * as signalR from '@microsoft/signalr';

const WS_URL = "/hubs/notifications";

export const useWebSocket = () => {
    const connection = useRef<signalR.HubConnection | null>(null);
    const { enqueueSnackbar } = useSnackbar();
    const onInviteChangedRef = useRef<() => void>(() => { });
    const onUpdateNotificationsRef = useRef<() => void>(() => { });
    const onUpdateGroupsRef = useRef<() => void>(() => { });
    const onUpdateGroupInfoRef = useRef<() => void>(() => { });

    const setOnInviteChanged = (cb: () => void) => {
        onInviteChangedRef.current = cb;
    };

    const setOnUpdateNotifications = (cb: () => void) => {
        onUpdateNotificationsRef.current = cb;
    };

    const setOnGroups = (cb: () => void) => {
        onUpdateGroupsRef.current = cb;
    };

    const setOnGroupInfo = (cb: () => void) => {
        onUpdateGroupInfoRef.current = cb;
    };

    useEffect(() => {
        if (!connection.current) {
            connection.current = new signalR.HubConnectionBuilder()
                .withUrl(WS_URL, {
                    withCredentials: true,
                    skipNegotiation: false,
                    transport: signalR.HttpTransportType.WebSockets
                })
                .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
                .configureLogging(signalR.LogLevel.Information)
                .build();

            connection.current.on('ReceiveNotification', (notification) => {
                console.log('Получено уведомление:', notification);
                enqueueSnackbar(notification.message, {
                    variant: notification.variant || 'info',
                    autoHideDuration: 3000
                });
                if (onUpdateNotificationsRef.current) {
                    console.log('Обновляем список уведомлений');
                    onUpdateNotificationsRef.current();
                }
                if (onUpdateGroupInfoRef.current) {
                    console.log('Вызываем обновление списка участников');
                    onUpdateGroupInfoRef.current();
                }
            });

            connection.current.on('InvitationResponse', (response) => {
                console.log('Получен ответ на приглашение:', response);
                enqueueSnackbar(
                    response.accepted
                        ? 'Приглашение принято'
                        : 'Приглашение отклонено',
                    {
                        variant: response.accepted ? 'success' : 'info',
                        autoHideDuration: 3000
                    }
                );
                if (onUpdateNotificationsRef.current) {
                    console.log('Обновляем список уведомлений после ответа');
                    onUpdateNotificationsRef.current();
                }
            });

            connection.current.on('UpdateGroupInviteMembers', () => {
                if (onInviteChangedRef.current) onInviteChangedRef.current();
            });

            connection.current.on('UpdateOrganizationInviteMembers', () => {
                if (onInviteChangedRef.current) onInviteChangedRef.current();
            });

            connection.current.on('UpdateNotifications', () => {
                if (onUpdateNotificationsRef.current) {
                    onUpdateNotificationsRef.current();
                }
                if (onUpdateGroupsRef.current) {
                    onUpdateGroupsRef.current();
                }
            });

            connection.current.onreconnecting((error) => {
                console.log('Переподключение к SignalR:', error);
                enqueueSnackbar('Переподключение к серверу...', { variant: 'warning' });
            });

            connection.current.onreconnected((connectionId) => {
                console.log('Переподключено к SignalR:', connectionId);
                enqueueSnackbar('Соединение восстановлено', { variant: 'success' });
            });

            connection.current.onclose((error: any) => {
                console.log('Соединение закрыто:', error);
            });
        }

        const startConnection = async () => {
            try {
                if (connection.current?.state === signalR.HubConnectionState.Disconnected) {
                    await connection.current.start();
                    console.log('SignalR подключен');
                }
            } catch (err) {
                console.error('Ошибка подключения к SignalR:', err);
                enqueueSnackbar('Ошибка подключения к серверу', { variant: 'error' });
            }
        };

        startConnection();

        return () => {
            if (connection.current) {
                connection.current.stop()
                    .catch(err => console.error('Ошибка при закрытии соединения:', err));
            }
        };
    }, [enqueueSnackbar]);

    const sendMessage = async (method: string, ...args: any[]) => {
        try {
            if (connection.current?.state === signalR.HubConnectionState.Connected) {
                await connection.current.invoke(method, ...args);
            } else {
                console.error('SignalR не подключен');
                enqueueSnackbar('Нет соединения с сервером', { variant: 'error' });
            }
        } catch (error) {
            console.error('Ошибка при отправке сообщения:', error);
            enqueueSnackbar('Ошибка при отправке сообщения', { variant: 'error' });
        }
    };

    const inviteToGroup = (groupId: string, invitedUserId: string, message: string) => {
        sendMessage('InviteToGroup', groupId, invitedUserId, message);
    };

    const inviteToOrganization = (organizationId: string, invitedUserId: string, message: string) => {
        sendMessage('InviteToOrganization', organizationId, invitedUserId, message);
    };

    const respondToInvitation = (notificationId: string, accept: boolean, responseMessage?: string) => {
        sendMessage('RespondToInvitation',notificationId, accept, responseMessage);
    };

    const cancelInvitation = (groupId: string, invitedUserId: string) => {
        sendMessage('CancelInvitation',groupId, invitedUserId);
    };

    const cancelOrganizationInvitation = (organizationId: string, invitedUserId: string) => {
        sendMessage('CancelOrganizationInvitation', organizationId, invitedUserId);
    };

    return {
        sendMessage,
        inviteToGroup,
        inviteToOrganization,
        respondToInvitation,
        cancelInvitation,
        cancelOrganizationInvitation,
        setOnInviteChanged,
        setOnUpdateNotifications,
        setOnGroups,
        setOnGroupInfo
    };
}; 