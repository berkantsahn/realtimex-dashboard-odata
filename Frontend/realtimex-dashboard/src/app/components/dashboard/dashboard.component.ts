import { Component, OnInit, OnDestroy } from '@angular/core';
import { RealTimeService } from '../../services/real-time.service';
import { AnnouncementService } from '../../services/announcement.service';
import { NotificationService } from '../../services/notification.service';
import { Subscription } from 'rxjs';
import { Chart, ChartConfiguration, ChartType } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: false
})
export class DashboardComponent implements OnInit, OnDestroy {
  realTimeData: any;
  announcements: any[] = [];
  notifications: any[] = [];
  private subscriptions: Subscription[] = [];

  // Grafik özellikleri
  public lineChartData: ChartConfiguration['data'] = {
    datasets: [
      {
        data: [],
        label: 'Gerçek Zamanlı Veri',
        backgroundColor: 'rgba(148,159,177,0.2)',
        borderColor: 'rgba(148,159,177,1)',
        pointBackgroundColor: 'rgba(148,159,177,1)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgba(148,159,177,0.8)',
        fill: 'origin',
      }
    ],
    labels: []
  };

  public lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    elements: {
      line: {
        tension: 0.5
      }
    },
    scales: {
      y: {
        beginAtZero: true
      }
    },
    plugins: {
      legend: { display: true }
    }
  };

  public lineChartType: ChartType = 'line';

  constructor(
    private realTimeService: RealTimeService,
    private announcementService: AnnouncementService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.startRealTimeConnection();
    this.loadInitialData();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.realTimeService.stopConnection();
  }

  private startRealTimeConnection() {
    this.realTimeService.startConnection();
    
    this.subscriptions.push(
      this.realTimeService.getDataStream().subscribe(data => {
        this.realTimeData = data;
        this.updateChartData(data);
      }),
      this.realTimeService.getAnnouncementStream().subscribe(announcement => {
        this.announcements.unshift(announcement);
      }),
      this.realTimeService.getNotificationStream().subscribe(notification => {
        this.notifications.unshift(notification);
      })
    );
  }

  private updateChartData(data: any) {
    if (data && data.value) {
      const now = new Date();
      this.lineChartData.labels?.push(now.toLocaleTimeString());
      this.lineChartData.datasets[0].data.push(data.value);

      // Son 10 veriyi göster
      if (this.lineChartData.labels!.length > 10) {
        this.lineChartData.labels?.shift();
        this.lineChartData.datasets[0].data.shift();
      }
    }
  }

  private loadInitialData() {
    this.announcementService.getActiveAnnouncements().subscribe(
      announcements => this.announcements = announcements
    );

    this.notificationService.getNotifications().subscribe(
      notifications => this.notifications = notifications
    );
  }

  markNotificationAsRead(id: number) {
    this.notificationService.markAsRead(id).subscribe();
  }

  markAllNotificationsAsRead() {
    this.notificationService.markAllAsRead().subscribe();
  }

  deleteNotification(id: number) {
    this.notificationService.deleteNotification(id).subscribe(() => {
      this.notifications = this.notifications.filter(n => n.id !== id);
    });
  }
} 