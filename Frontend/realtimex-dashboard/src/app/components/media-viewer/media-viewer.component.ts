import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ChatMessage, MediaType } from '../../models/chat-message.model';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-media-viewer',
  templateUrl: './media-viewer.component.html',
  styleUrls: ['./media-viewer.component.scss']
})
export class MediaViewerComponent {
  MediaType = MediaType;
  hasError = false;

  constructor(
    public dialogRef: MatDialogRef<MediaViewerComponent>,
    @Inject(MAT_DIALOG_DATA) public message: ChatMessage,
    private chatService: ChatService
  ) {}

  getMediaUrl(url: string | undefined): string {
    if (!url) return '';
    return this.chatService.getMediaUrl(url);
  }

  downloadMedia(): void {
    if (!this.message.mediaUrl) return;
    
    const link = document.createElement('a');
    link.href = this.getMediaUrl(this.message.mediaUrl);
    link.download = this.message.mediaName || 'download';
    link.click();
  }

  onMediaError(event: Event): void {
    console.error('Media loading error:', event);
    this.hasError = true;
  }

  close(): void {
    this.dialogRef.close();
  }
} 