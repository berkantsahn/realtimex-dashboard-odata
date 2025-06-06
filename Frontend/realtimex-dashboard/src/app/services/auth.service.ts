import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { CryptoService } from './crypto.service';

interface User {
  id: string;
  username: string;
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(
    private http: HttpClient,
    private cryptoService: CryptoService
  ) {
    this.currentUserSubject = new BehaviorSubject<User | null>(
      this.getUserFromStorage()
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  private getUserFromStorage(): User | null {
    const userStr = localStorage.getItem('currentUser');
    return userStr ? JSON.parse(userStr) : null;
  }

  public getCurrentUserId(): string {
    const user = this.currentUserSubject.value;
    return user ? user.id : '';
  }

  public getToken(): string {
    const user = this.currentUserSubject.value;
    return user ? user.token : '';
  }

  public isAuthenticated(): boolean {
    return !!this.currentUserSubject.value;
  }

  login(username: string, password: string): Observable<User> {
    const encryptedPassword = this.cryptoService.encrypt(password);
    
    return this.http.post<User>(`${environment.apiUrl}/auth/login`, {
      username,
      password: encryptedPassword
    }).pipe(
      map(user => {
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
        return user;
      })
    );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
} 