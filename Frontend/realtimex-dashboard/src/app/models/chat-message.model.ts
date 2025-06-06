export interface ChatMessage {
    id: number;
    senderId: string;
    receiverId: string;
    content: string;
    mediaUrl?: string;
    mediaName?: string;
    mediaSize?: number;
    mediaType?: MediaType;
    type?: MediaType;
    sentAt: Date;
    isRead: boolean;
    readAt?: Date;
}

export enum MediaType {
    Text = 'Text',
    Audio = 'Audio',
    Image = 'Image',
    Video = 'Video',
    Document = 'Document'
} 