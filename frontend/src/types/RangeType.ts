export type RangeType = 'Indoor' | 'Outdoor' | 'Virtual' | 'Other';

export interface RangeDTO {
  id: string;
  location: string;
  description: string;
  capacity: number;
  type: RangeType;
  organizationId: string;
  isActive: boolean;
}

export interface CreateRangeDTO {
  location: string;
  description: string;
  capacity: number;
  type: RangeType;
  organizationId?: string;
}

export interface UpdateRangeDTO {
  location: string;
  description: string;
  capacity: number;
  type: RangeType;
} 