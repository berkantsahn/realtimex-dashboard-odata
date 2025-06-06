import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject, from } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { ChatMessage, MediaType } from '../models/chat-message.model';
import { HttpClient, HttpEventType } from '@angular/common/http';
import { map, switchMap } from 'rxjs/operators';

interface MessageReadEvent {
  messageId: number;
  readAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: HubConnection;
  private messagesSubject = new BehaviorSubject<ChatMessage[]>([]);
  private messageReadSubject = new Subject<MessageReadEvent>();
  private messageDeletedSubject = new Subject<number>();

  public messages$ = this.messagesSubject.asObservable();
  public messageRead$ = this.messageReadSubject.asObservable();
  public messageDeleted$ = this.messageDeletedSubject.asObservable();

  constructor(
    private authService: AuthService,
    private http: HttpClient
  ) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/chathub`, {
        accessTokenFactory: () => this.authService.getToken()
      })
      .withAutomaticReconnect()
      .build();

    this.setupHubConnection();
  }

  private setupHubConnection() {
    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      const currentMessages = this.messagesSubject.value;
      this.messagesSubject.next([...currentMessages, message]);
    });

    this.hubConnection.on('MessageRead', (data: MessageReadEvent) => {
      this.messageReadSubject.next(data);
    });

    this.hubConnection.on('MessageDeleted', (messageId: number) => {
      this.messageDeletedSubject.next(messageId);
    });
  }

  async startConnection() {
    try {
      await this.hubConnection.start();
      console.log('Chat Hub Connected!');
    } catch (err) {
      console.error('Error while starting chat connection: ' + err);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  async stopConnection() {
    try {
      await this.hubConnection.stop();
      console.log('Chat Hub Disconnected!');
    } catch (err) {
      console.error('Error while stopping chat connection: ' + err);
    }
  }

  sendMessage(receiverId: string, content: string, mediaFile?: File): Observable<ChatMessage> {
    if (mediaFile) {
      const formData = new FormData();
      formData.append('file', mediaFile);
      formData.append('receiverId', receiverId);
      formData.append('content', content);
      
      return this.http.post<ChatMessage>(`${environment.apiUrl}/api/chat/upload`, formData).pipe(
        switchMap(message => from(this.hubConnection.invoke('SendMessage', receiverId, content, message.mediaUrl)).pipe(
          map(() => message)
        ))
      );
    } else {
      const message: ChatMessage = {
        id: 0, // Backend will assign the actual ID
        receiverId,
        content,
        senderId: this.authService.getCurrentUserId(),
        sentAt: new Date(),
        isRead: false,
        type: MediaType.Text
      };

      return from(this.hubConnection.invoke('SendMessage', receiverId, content)).pipe(
        map(() => message)
      );
    }
  }

  async markMessageAsRead(messageId: number) {
    try {
      await this.hubConnection.invoke('MarkMessageAsRead', messageId);
    } catch (err) {
      console.error('Error while marking message as read: ' + err);
    }
  }

  getMessages(userId: string, otherUserId: string): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${environment.apiUrl}/api/chat/messages/${userId}/${otherUserId}`);
  }

  uploadMedia(file: File, receiverId: number): Observable<ChatMessage> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('receiverId', receiverId.toString());

    return this.http.post<ChatMessage>(`${environment.apiUrl}/api/chat/upload`, formData, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map(event => {
        if (event.type === HttpEventType.Response) {
          return event.body as ChatMessage;
        }
        throw new Error('Upload failed');
      })
    );
  }

  markAsRead(messageId: number): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/api/chat/messages/${messageId}/read`, {});
  }

  deleteMessage(messageId: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/chat/messages/${messageId}`);
  }

  getMediaUrl(url: string): string {
    if (!url) return '';
    if (url.startsWith('http')) return url;
    return `${environment.apiUrl}/api/media/${url}`;
  }

  getThumbnailUrl(url: string): string {
    if (!url) return '';
    if (url.startsWith('http')) return url;
    return `${environment.apiUrl}/api/media/thumbnail/${url}`;
  }

  getMediaTypeIcon(mediaType: MediaType): string {
    switch (mediaType) {
      case MediaType.Image: return 'image';
      case MediaType.Video: return 'videocam';
      case MediaType.Audio: return 'audiotrack';
      case MediaType.Document: return 'description';
      default: return 'attach_file';
    }
  }

  formatFileSize(bytes: number): string {
    if (!bytes) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  formatDuration(seconds: number): string {
    if (!seconds) return '0:00';
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = Math.floor(seconds % 60);
    return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
  }
} 