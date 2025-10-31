
export type UserWithStatus = {
    user: {
        id: string;
        firstName: string;
        secoundName: string;
        age: number;
        country: string;
        grade: string;
        email: string;
        // Добавьте другие поля пользователя, если нужно
    };
    status: string | null; // Например: 'Pending', 'Approved', 'Rejected' или null
}; 