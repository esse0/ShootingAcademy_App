export enum NotificationType {
  GroupInvitation,
  OrganizationInvitation,
  Message,
  System,
  TrainingReminder
}

export interface Notification {
  id: string;
  userId: string;
  senderId?: string;
  type: NotificationType;
  title: string;
  message: string;
  isRead: boolean;
  requiresResponse: boolean;
  response?: string;
  inviteId?: string;
  responseAt?: string;
  createdAt: string;
}