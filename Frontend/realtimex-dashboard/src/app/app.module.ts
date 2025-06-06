import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgChartsModule } from 'ng2-charts';
import { DxDataGridModule, DxLoadPanelModule, DxPopupModule, DxFormModule } from 'devextreme-angular';
import { CommonModule } from '@angular/common';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { LoginComponent } from './components/login/login.component';
import { DataGridComponent } from './components/data-grid/data-grid.component';
import { ChatModule } from './modules/chat.module';

import { AuthService } from './services/auth.service';
import { RealTimeService } from './services/real-time.service';
import { AnnouncementService } from './services/announcement.service';
import { NotificationService } from './services/notification.service';
import { ChatService } from './services/chat.service';
import { CryptoService } from './services/crypto.service';
import { ODataService } from './services/odata.service';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    LoginComponent,
    DataGridComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatListModule,
    MatTooltipModule,
    NgChartsModule,
    DxDataGridModule,
    DxLoadPanelModule,
    DxPopupModule,
    DxFormModule,
    ChatModule
  ],
  providers: [
    AuthService,
    RealTimeService,
    AnnouncementService,
    NotificationService,
    ChatService,
    CryptoService,
    ODataService
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  bootstrap: [AppComponent]
})
export class AppModule { } 