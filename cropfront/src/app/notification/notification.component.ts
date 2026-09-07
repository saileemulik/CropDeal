import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, Notification } from '../services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-panel">
      <div class="notification-header">
        <h3>Notifications</h3>
        <button *ngIf="notifications.length > 0" (click)="markAllAsRead()" class="mark-all-btn">
          Mark All Read
        </button>
      </div>
      
      <div class="notification-list" *ngIf="notifications.length > 0; else noNotifications">
        <div *ngFor="let notification of notifications" 
             class="notification-item" 
             [class.unread]="!notification.isRead"
             (click)="markAsRead(notification.id)">
          <div class="notification-content">
            <p>{{ notification.message }}</p>
            <small>{{ formatDate(notification.createdAt) }}</small>
          </div>
          <div *ngIf="!notification.isRead" class="unread-indicator"></div>
        </div>
      </div>
      
      <ng-template #noNotifications>
        <div class="no-notifications">
          <p>No notifications</p>
        </div>
      </ng-template>
    </div>
  `,
  styles: [`
    .notification-panel {
      max-width: 400px;
      background: white;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      overflow: hidden;
    }
    
    .notification-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px;
      background: #f8f9fa;
      border-bottom: 1px solid #e9ecef;
    }
    
    .notification-header h3 {
      margin: 0;
      font-size: 18px;
      color: #333;
    }
    
    .mark-all-btn {
      background: #007bff;
      color: white;
      border: none;
      padding: 6px 12px;
      border-radius: 4px;
      cursor: pointer;
      font-size: 12px;
    }
    
    .notification-list {
      max-height: 400px;
      overflow-y: auto;
    }
    
    .notification-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 16px;
      border-bottom: 1px solid #f0f0f0;
      cursor: pointer;
      transition: background-color 0.2s;
    }
    
    .notification-item:hover {
      background: #f8f9fa;
    }
    
    .notification-item.unread {
      background: #e3f2fd;
    }
    
    .notification-content {
      flex: 1;
    }
    
    .notification-content p {
      margin: 0 0 4px 0;
      font-size: 14px;
      color: #333;
    }
    
    .notification-content small {
      color: #666;
      font-size: 12px;
    }
    
    .unread-indicator {
      width: 8px;
      height: 8px;
      background: #007bff;
      border-radius: 50%;
      margin-left: 8px;
    }
    
    .no-notifications {
      padding: 40px 16px;
      text-align: center;
      color: #666;
    }
  `]
})
export class NotificationComponent implements OnInit {
  notifications: Notification[] = [];

  constructor(private notificationService: NotificationService) {}

  ngOnInit() {
    this.loadNotifications();
  }

  loadNotifications() {
    this.notificationService.getNotifications().subscribe({
      next: (notifications) => {
        this.notifications = notifications;
      },
      error: (error) => {
        console.error('Error loading notifications:', error);
      }
    });
  }

  markAsRead(id: string) {
    this.notificationService.markAsRead(id).subscribe({
      next: () => {
        const notification = this.notifications.find(n => n.id === id);
        if (notification) {
          notification.isRead = true;
        }
        this.notificationService.loadUnreadCount();
      },
      error: (error) => {
        console.error('Error marking notification as read:', error);
      }
    });
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        this.notifications.forEach(n => n.isRead = true);
        this.notificationService.loadUnreadCount();
      },
      error: (error) => {
        console.error('Error marking all notifications as read:', error);
      }
    });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString();
  }
}