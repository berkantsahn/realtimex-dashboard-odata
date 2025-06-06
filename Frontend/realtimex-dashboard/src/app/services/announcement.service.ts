import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  constructor(private http: HttpClient) { }

  getActiveAnnouncements(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/announcement`);
  }

  getAnnouncement(id: number): Observable<any> {
    return this.http.get(`${environment.apiUrl}/announcement/${id}`);
  }

  createAnnouncement(announcement: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/announcement`, announcement);
  }

  updateAnnouncement(id: number, announcement: any): Observable<any> {
    return this.http.put(`${environment.apiUrl}/announcement/${id}`, announcement);
  }

  deleteAnnouncement(id: number): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/announcement/${id}`);
  }
} 