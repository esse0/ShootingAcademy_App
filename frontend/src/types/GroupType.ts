import { UserModelWithProfilePhoto } from "./UserModelWithProfilePhoto";
import { FullUserModel } from "./UserProfileData";

export default interface GroupType {
    id?: string;
    organisationName: string;
    coach?: UserModelWithProfilePhoto;
    members?: FullUserModel[];
}