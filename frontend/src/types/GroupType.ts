import { FullUserModel } from "./UserProfileData";

export default interface GroupType {
    id?: string;
    organisationName: string;
    coach?: FullUserModel;
    members?: FullUserModel[];
}