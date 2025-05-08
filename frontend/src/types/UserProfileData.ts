export interface UserProfileData{
    Name: string,
    LastName: string,
    Email: string,
    Age: number,
    Grade: string,
    Country: string,
    City: string,
    Address: string,
}

export interface FullUserModel { // from server
    id: string,
    firstName: string,
    secoundName: string,
    patronymicName: string,
    age: number,
    grade: string,
    country: string,
    city: string,
    address: string,
    email: string,
    role: string,
}