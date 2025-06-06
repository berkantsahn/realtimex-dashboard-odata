import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ChatService } from '../../services/chat.service';
import { RealTimeService } from '../../services/real-time.service';
import { AuthService } from '../../services/auth.service';
import { ChatMessage, MediaType } from '../../models/chat-message.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MediaViewerComponent } from '../media-viewer/media-viewer.component';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {
  @ViewChild('messageContainer') private messageContainer!: ElementRef;
  @ViewChild('fileInput') private fileInput!: ElementRef;

  messages: ChatMessage[] = [];
  newMessage = '';
  currentUserId = '';
  selectedUserId = '';
  isUploading = false;
  uploadProgress = 0;
  MediaType = MediaType;

  constructor(
    private chatService: ChatService,
    private realTimeService: RealTimeService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    this.currentUserId = this.authService.getCurrentUserId();
  }

  ngOnInit(): void {
    // Yeni mesajları dinle
    this.realTimeService.messageReceived.subscribe((message: ChatMessage) => {
      this.messages.push(message);
      this.scrollToBottom();
      
      // Mesaj bana geldiyse okundu olarak işaretle
      if (message.receiverId === this.currentUserId) {
        this.markAsRead(message.id);
      }
    });

    // Mesaj okundu bildirimlerini dinle
    this.chatService.messageRead$.subscribe(data => {
      const message = this.messages.find(m => m.id === data.messageId);
      if (message) {
        message.isRead = true;
        message.readAt = data.readAt;
      }
    });

    // Mesaj silindi bildirimlerini dinle
    this.chatService.messageDeleted$.subscribe(messageId => {
      this.messages = this.messages.filter(m => m.id !== messageId);
    });
  }

  loadMessages(otherUserId: string): void {
    this.selectedUserId = otherUserId;
    this.chatService.getMessages(this.currentUserId, otherUserId)
      .subscribe({
        next: (messages) => {
          this.messages = messages;
          this.scrollToBottom();
          
          // Okunmamış mesajları okundu olarak işaretle
          messages
            .filter(m => !m.isRead && m.receiverId === this.currentUserId)
            .forEach(m => this.markAsRead(m.id));
        },
        error: (error) => {
          console.error('Failed to load messages:', error);
          this.showError('Failed to load messages');
        }
      });
  }

  sendMessage(): void {
    if (!this.newMessage.trim()) return;

    const message: ChatMessage = {
      id: 0,
      senderId: this.currentUserId,
      receiverId: this.selectedUserId,
      content: this.newMessage,
      type: MediaType.Text,
      sentAt: new Date(),
      isRead: false
    };

    this.chatService.sendMessage(message).subscribe({
      next: (response) => {
        this.messages.push(response);
        this.newMessage = '';
        this.scrollToBottom();
      },
      error: (error) => {
        console.error('Failed to send message:', error);
        this.showError('Failed to send message');
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];
      this.uploadMedia(file);
    }
  }

  uploadMedia(file: File): void {
    this.isUploading = true;
    this.uploadProgress = 0;

    this.chatService.uploadMedia(file, parseInt(this.selectedUserId)).pipe(
      filter(message => message !== null)
    ).subscribe({
      next: (message) => {
        if (message) {
          this.messages.push(message);
          this.scrollToBottom();
          this.isUploading = false;
          this.uploadProgress = 0;
          if (this.fileInput) {
            this.fileInput.nativeElement.value = '';
          }
        }
      },
      error: (error) => {
        console.error('Failed to upload media:', error);
        this.isUploading = false;
        this.uploadProgress = 0;
        this.showError('Failed to upload media');
      }
    });
  }

  openMediaViewer(message: ChatMessage): void {
    this.dialog.open(MediaViewerComponent, {
      data: message,
      maxWidth: '100vw',
      maxHeight: '100vh',
      height: '100%',
      width: '100%',
      panelClass: 'media-viewer-dialog'
    });
  }

  markAsRead(messageId: number): void {
    this.chatService.markAsRead(messageId).subscribe({
      error: (error) => {
        console.error('Failed to mark message as read:', error);
      }
    });
  }

  deleteMessage(messageId: number): void {
    this.chatService.deleteMessage(messageId).subscribe({
      next: () => {
        this.messages = this.messages.filter(m => m.id !== messageId);
      },
      error: (error) => {
        console.error('Failed to delete message:', error);
        this.showError('Failed to delete message');
      }
    });
  }

  getMediaUrl(mediaUrl: string): string {
    return this.chatService.getMediaUrl(mediaUrl);
  }

  getThumbnailUrl(thumbnailUrl: string): string {
    return this.chatService.getThumbnailUrl(thumbnailUrl);
  }

  getMediaTypeIcon(mediaType: MediaType): string {
    return this.chatService.getMediaTypeIcon(mediaType);
  }

  formatFileSize(bytes: number): string {
    return this.chatService.formatFileSize(bytes);
  }

  formatDuration(seconds: number): string {
    return this.chatService.formatDuration(seconds);
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      if (this.messageContainer) {
        const element = this.messageContainer.nativeElement;
        element.scrollTop = element.scrollHeight;
      }
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom'
    });
  }
} 