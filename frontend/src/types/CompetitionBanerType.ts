export interface CompetitionMemberType{
  id: string;
  fullName: string;
  age: number;
  country: string;
  grade: string;
  result?: number;
}

export interface CompetitionType{
  id: string;
  title: string;
  description: string;
  date: string;
  time: string;
  memberCount: number;
  maxMemberCount: number;
  venue: string;
  country: string;
  city: string;
  status: string;
  exercise: string;
  organiser: string;
  members?: CompetitionMemberType[]
}