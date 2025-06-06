import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatMenuModule } from '@angular/material/menu';

import { ChatComponent } from '../components/chat/chat.component';
import { MediaViewerComponent } from '../components/media-viewer/media-viewer.component';
import { FileSizePipe } from '../pipes/file-size.pipe';

@NgModule({
  declarations: [
    ChatComponent,
    MediaViewerComponent,
    FileSizePipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressBarModule,
    MatDialogModule,
    MatSnackBarModule,
    MatMenuModule
  ],
  exports: [
    ChatComponent
  ]
})
export class ChatModule { } 