import { atom } from "jotai";
import { atomWithStorage } from "jotai/utils";
import { Notification } from "../types/NotificationType";

export const userAtom = atomWithStorage("userFields", 
    {
        id: "",
        firstName: "Mr.",
        secoundName: "Guest",
        patronymicName: "",
        age: 0,
        grade: "",
        country: "",
        city: "",
        address: "",
        email: "add email",
        role: "guest",
        profilePhotoUri: ""
    });

export const notificationsAtom = atom<Notification[]>([]);

export const unreadNotificationsAtom = atom((get) =>
  get(notificationsAtom).filter((n) => !n.isRead)
);

// Количество непрочитанных (для бейджа)
export const unreadCountAtom = atom((get) =>
  get(unreadNotificationsAtom).length
);

// Добавить новое уведомление
export const addNotificationAtom = atom(null, (get, set, notification: Notification) => {
  const current = get(notificationsAtom);
  set(notificationsAtom, [notification, ...current]);
});

// Отметить уведомление как прочитанное
export const markAsReadAtom = atom(null, (get, set, id: string) => {
  const updated = get(notificationsAtom).map((n) =>
    n.id === id ? { ...n, isRead: true } : n
  );
  set(notificationsAtom, updated);
});

// Очистить все уведомления
export const clearNotificationsAtom = atom(null, (_get, set) => {
  set(notificationsAtom, []);
});