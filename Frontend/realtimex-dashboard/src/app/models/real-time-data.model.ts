export interface RealTimeData {
    id: number;
    deviceId: string;
    value: number;
    timestamp: Date;
    type: string;
    status: string;
    metadata?: { [key: string]: any };
} 