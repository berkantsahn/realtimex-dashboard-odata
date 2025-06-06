import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';

import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DataGridComponent } from './components/data-grid/data-grid.component';

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';

// DevExtreme
import { DxDataGridModule, DxLoadPanelModule, DxButtonModule, DxLoadIndicatorModule } from 'devextreme-angular';

// Charts
import { NgChartsModule } from 'ng2-charts';

// Chat Module
import { ChatModule } from './modules/chat.module';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent) },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'data-grid', component: DataGridComponent }
];

const materialModules = [
  MatCardModule,
  MatSnackBarModule,
  MatDialogModule,
  MatButtonModule,
  MatIconModule,
  MatProgressBarModule,
  MatFormFieldModule,
  MatInputModule,
  MatProgressSpinnerModule,
  MatToolbarModule,
  MatMenuModule
];

const devExtremeModules = [
  DxDataGridModule,
  DxLoadPanelModule,
  DxButtonModule,
  DxLoadIndicatorModule
];

@NgModule({
  declarations: [
    DashboardComponent,
    DataGridComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(routes),
    ...materialModules,
    ...devExtremeModules,
    NgChartsModule,
    ChatModule
  ],
  providers: []
})
export class AppModule { } 