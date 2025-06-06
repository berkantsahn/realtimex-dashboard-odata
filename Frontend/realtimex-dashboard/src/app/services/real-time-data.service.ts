import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RealTimeDataService {
  constructor(private http: HttpClient) { }

  getLatestData(deviceId: string, count: number = 10): Observable<any> {
    return this.http.get(`${environment.apiUrl}/realtimedata/${deviceId}/latest?count=${count}`);
  }

  getDataByDateRange(deviceId: string, startDate: Date, endDate: Date): Observable<any> {
    return this.http.get(`${environment.apiUrl}/realtimedata/${deviceId}/range`, {
      params: {
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString()
      }
    });
  }

  addData(data: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/realtimedata`, data);
  }

  processData(id: number): Observable<any> {
    return this.http.post(`${environment.apiUrl}/realtimedata/${id}/process`, {});
  }
} 