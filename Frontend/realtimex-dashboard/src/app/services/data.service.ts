import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { RealTimeData } from '../models/real-time-data.model';
import { Announcement } from '../models/announcement.model';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private baseUrl = `${environment.apiUrl}/odata`;

  constructor(private http: HttpClient) { }

  // Generic CRUD operations
  create<T>(entity: string, data: T): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}/${entity}`, data);
  }

  update<T>(entity: string, id: number | string, data: Partial<T>): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}/${entity}(${id})`, data);
  }

  delete(entity: string, id: number | string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${entity}(${id})`);
  }

  // RealTimeData OData endpoints
  getRealTimeData(options?: any): Observable<any> {
    let params = new HttpParams();
    if (options) {
      if (options.filter) params = params.append('$filter', options.filter);
      if (options.orderby) params = params.append('$orderby', options.orderby);
      if (options.top) params = params.append('$top', options.top);
      if (options.skip) params = params.append('$skip', options.skip);
      if (options.select) params = params.append('$select', options.select);
      if (options.expand) params = params.append('$expand', options.expand);
      if (options.count) params = params.append('$count', 'true');
    }
    return this.http.get<any>(`${this.baseUrl}/RealTimeData`, { params });
  }

  getRealTimeDataById(id: number): Observable<RealTimeData> {
    return this.http.get<RealTimeData>(`${this.baseUrl}/RealTimeData(${id})`);
  }

  createRealTimeData(data: RealTimeData): Observable<RealTimeData> {
    return this.create<RealTimeData>('RealTimeData', data);
  }

  updateRealTimeData(id: number, data: RealTimeData): Observable<RealTimeData> {
    return this.update<RealTimeData>('RealTimeData', id, data);
  }

  deleteRealTimeData(id: number): Observable<void> {
    return this.delete('RealTimeData', id);
  }

  // Announcement OData endpoints
  getAnnouncements(options?: any): Observable<any> {
    let params = new HttpParams();
    if (options) {
      if (options.filter) params = params.append('$filter', options.filter);
      if (options.orderby) params = params.append('$orderby', options.orderby);
      if (options.top) params = params.append('$top', options.top);
      if (options.skip) params = params.append('$skip', options.skip);
      if (options.select) params = params.append('$select', options.select);
      if (options.expand) params = params.append('$expand', options.expand);
      if (options.count) params = params.append('$count', 'true');
    }
    return this.http.get<any>(`${this.baseUrl}/Announcements`, { params });
  }

  getAnnouncementById(id: number): Observable<Announcement> {
    return this.http.get<Announcement>(`${this.baseUrl}/Announcements(${id})`);
  }

  createAnnouncement(announcement: Announcement): Observable<Announcement> {
    return this.create<Announcement>('Announcements', announcement);
  }

  updateAnnouncement(id: number, announcement: Announcement): Observable<Announcement> {
    return this.update<Announcement>('Announcements', id, announcement);
  }

  deleteAnnouncement(id: number): Observable<void> {
    return this.delete('Announcements', id);
  }
} 