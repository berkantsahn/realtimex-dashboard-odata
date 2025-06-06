import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { RealTimeData } from '../models/real-time-data.model';
import { ChatMessage } from '../models/chat-message.model';
import { Announcement } from '../models/announcement.model';
import { Notification } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class RealTimeService {
  private hubConnection: HubConnection;
  private dataSubject = new Subject<RealTimeData>();
  private messageSubject = new Subject<ChatMessage>();
  private announcementSubject = new Subject<Announcement>();
  private notificationSubject = new Subject<Notification>();

  // Observable streams
  public dataReceived = this.dataSubject.asObservable();
  public messageReceived = this.messageSubject.asObservable();
  public announcementReceived = this.announcementSubject.asObservable();
  public notificationReceived = this.notificationSubject.asObservable();

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hubs/dashboard`)
      .withAutomaticReconnect()
      .build();

    this.startConnection();
  }

  public startConnection(): Promise<void> {
    return this.hubConnection.start()
      .then(() => {
        console.log('SignalR Bağlantısı başarılı');
        this.registerHandlers();
      })
      .catch(err => {
        console.error('SignalR Bağlantı hatası:', err);
        throw err;
      });
  }

  public stopConnection(): Promise<void> {
    return this.hubConnection.stop();
  }

  private registerHandlers(): void {
    // Gerçek zamanlı veri akışı
    this.hubConnection.on('ReceiveData', (data: RealTimeData) => {
      this.dataSubject.next(data);
    });

    // Chat mesajları
    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      this.messageSubject.next(message);
    });

    // Duyurular
    this.hubConnection.on('ReceiveAnnouncement', (announcement: Announcement) => {
      this.announcementSubject.next(announcement);
    });

    // Bildirimler
    this.hubConnection.on('ReceiveNotification', (notification: Notification) => {
      this.notificationSubject.next(notification);
    });
  }

  // Stream metodları
  public getDataStream(): Observable<RealTimeData> {
    return this.dataReceived;
  }

  public getAnnouncementStream(): Observable<Announcement> {
    return this.announcementReceived;
  }

  public getNotificationStream(): Observable<Notification> {
    return this.notificationReceived;
  }

  // Veri gönderme metodları
  public sendMessage(message: ChatMessage): Promise<void> {
    return this.hubConnection.invoke('SendMessage', message);
  }

  public broadcastData(data: RealTimeData): Promise<void> {
    return this.hubConnection.invoke('BroadcastData', data);
  }

  public broadcastAnnouncement(announcement: Announcement): Promise<void> {
    return this.hubConnection.invoke('BroadcastAnnouncement', announcement);
  }
} 