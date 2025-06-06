import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private http: HttpClient) { }

  getNotifications(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/notification`);
  }

  markAsRead(id: number): Observable<any> {
    return this.http.put(`${environment.apiUrl}/notification/${id}/read`, {});
  }

  markAllAsRead(): Observable<any> {
    return this.http.put(`${environment.apiUrl}/notification/read-all`, {});
  }

  deleteNotification(id: number): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/notification/${id}`);
  }
} 