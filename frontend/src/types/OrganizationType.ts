export type OrganizationType = {
    id: string
    name: string
    description: string
    email: string
    phoneNumber: string
    address: string
    createdAt: string
    members: Array<OrganizationMemberType>
    ranges: Array<RangeType>
};

export type OrganizationMemberType = {
    userId: string
    userName: string
    status: string
    role: string
    joinedAt: string
}

export type RangeType = {
    id: string;
    location: string;
    description: string;
    capacity: string;
    type: string;
    isActive: boolean;
}