import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ChatComponent } from '../components/chat/chat.component';
import { MediaViewerComponent } from '../components/media-viewer/media-viewer.component';

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';

// Pipes
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
    ReactiveFormsModule,
    RouterModule.forChild([
      { path: 'chat', component: ChatComponent }
    ]),
    // Material Modules
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatMenuModule,
    MatProgressBarModule
  ],
  exports: [
    ChatComponent,
    MediaViewerComponent,
    FileSizePipe
  ]
})
export class ChatModule { } 