import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection;
  private realTimeDataSubject = new BehaviorSubject<any>(null);
  private announcementSubject = new BehaviorSubject<any>(null);

  public realTimeData$ = this.realTimeDataSubject.asObservable();
  public announcements$ = this.announcementSubject.asObservable();

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.signalRUrl)
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveRealTimeData', (data) => {
      this.realTimeDataSubject.next(data);
    });

    this.hubConnection.on('ReceiveAnnouncement', (announcement) => {
      this.announcementSubject.next(announcement);
    });
  }

  async startConnection(): Promise<void> {
    try {
      await this.hubConnection.start();
      console.log('SignalR Connected!');
    } catch (err) {
      console.log('Error while starting connection: ' + err);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  async joinGroup(groupName: string): Promise<void> {
    try {
      await this.hubConnection.invoke('JoinGroup', groupName);
    } catch (err) {
      console.log('Error while joining group: ' + err);
    }
  }

  async leaveGroup(groupName: string): Promise<void> {
    try {
      await this.hubConnection.invoke('LeaveGroup', groupName);
    } catch (err) {
      console.log('Error while leaving group: ' + err);
    }
  }

  async sendRealTimeData(data: any): Promise<void> {
    try {
      await this.hubConnection.invoke('SendRealTimeData', data);
    } catch (err) {
      console.log('Error while sending real-time data: ' + err);
    }
  }

  async sendAnnouncement(announcement: any): Promise<void> {
    try {
      await this.hubConnection.invoke('SendAnnouncement', announcement);
    } catch (err) {
      console.log('Error while sending announcement: ' + err);
    }
  }
} 