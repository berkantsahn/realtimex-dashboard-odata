export interface Announcement {
    id: number;
    title: string;
    content: string;
    priority: AnnouncementPriority;
    createdAt: Date;
    updatedAt?: Date;
    expiresAt?: Date;
    createdBy: number;
    isActive: boolean;
}

export enum AnnouncementPriority {
    Low = 'Low',
    Medium = 'Medium',
    High = 'High',
    Critical = 'Critical'
} 