import React from 'react';
import { Card, CardContent, Typography, Button, Stack, Box } from '@mui/material';
import { Notification, NotificationType } from '../../types/NotificationType';


interface NotificationCardProps {
    notification: Notification;
    onAccept?: () => void;
    onDecline?: () => void;
}

export const NotificationCard: React.FC<NotificationCardProps> = ({
    notification,
    onAccept,
    onDecline,
}) => {
    const isInvitation = notification.type === NotificationType.GroupInvitation || notification.type === NotificationType.OrganizationInvitation;

    return (
        <Card
            sx={{
                mb: 1,
                backgroundColor: notification.isRead ? 'inherit' : 'rgba(69, 92, 199, 0.05)',
                '&:hover': {
                    backgroundColor: 'rgba(69, 92, 199, 0.1)',
                }
            }}
        >
            <CardContent>
                <Stack spacing={1}>
                    <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                        <Typography variant="subtitle1" fontWeight="bold">
                            {notification.title}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                            {notification.createdAt}
                        </Typography>
                    </Box>

                    <Typography variant="body2" color="text.secondary">
                        {notification.message}
                    </Typography>

                    {notification.requiresResponse && isInvitation && !notification.response && (
                        <Stack direction="row" spacing={1} mt={1}>
                            <Button
                                variant="contained"
                                color="primary"
                                size="small"
                                onClick={onAccept}
                                sx={{ flex: 1 }}
                            >
                                Принять
                            </Button>
                            <Button
                                variant="outlined"
                                color="error"
                                size="small"
                                onClick={onDecline}
                                sx={{ flex: 1 }}
                            >
                                Отклонить
                            </Button>
                        </Stack>
                    )}

                    {notification.response && (
                        <Typography
                            variant="caption"
                            color={notification.response === 'Принято' ? 'success.main' : 'error.main'}
                            sx={{ mt: 1 }}
                        >
                            {notification.response === 'Принято' ? 'Принято' : 'Отклонено'}
                        </Typography>
                    )}
                </Stack>
            </CardContent>
        </Card>
    );
};