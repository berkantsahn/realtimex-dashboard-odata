import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ODataService {
  private apiUrl = `${environment.apiUrl}/data`;

  constructor(private http: HttpClient) { }

  getData<T>(endpoint: string, options: {
    filter?: string;
    orderby?: string;
    top?: number;
    skip?: number;
    select?: string[];
    expand?: string[];
  } = {}): Observable<T> {
    let params = new HttpParams();

    if (options.filter) {
      params = params.set('$filter', options.filter);
    }
    if (options.orderby) {
      params = params.set('$orderby', options.orderby);
    }
    if (options.top) {
      params = params.set('$top', options.top.toString());
    }
    if (options.skip) {
      params = params.set('$skip', options.skip.toString());
    }
    if (options.select && options.select.length > 0) {
      params = params.set('$select', options.select.join(','));
    }
    if (options.expand && options.expand.length > 0) {
      params = params.set('$expand', options.expand.join(','));
    }

    return this.http.get<T>(`${environment.apiUrl}/${endpoint}`, { params });
  }

  update(id: number, data: any, options?: any): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}`, data, options);
  }

  create(data: any, options?: any): Observable<any> {
    return this.http.post(this.apiUrl, data, options);
  }

  delete(id: number, options?: any): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, options);
  }
} 