export interface UserModelWithProfilePhoto{ // from server
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
    profilePhotoUri: string
}