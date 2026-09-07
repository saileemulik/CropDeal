import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../services/notification.service';
import { NotificationComponent } from '../notification/notification.component';
import { AuthService } from '../services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule, NotificationComponent],
  template: `
    <div class="notification-bell-container">
      <button class="notification-bell" (click)="togglePanel()">
        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.9 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z"/>
        </svg>
        <span *ngIf="unreadCount > 0" class="badge">{{ unreadCount }}</span>
      </button>
      
      <div *ngIf="showPanel" class="notification-panel-overlay" (click)="closePanel()">
        <div class="notification-panel-wrapper" (click)="$event.stopPropagation()">
          <app-notification></app-notification>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notification-bell-container {
      position: relative;
    }
    
    .notification-bell {
      position: relative;
      background: none;
      border: none;
      cursor: pointer;
      padding: 8px;
      border-radius: 50%;
      color: #666;
      transition: all 0.2s;
    }
    
    .notification-bell:hover {
      background: #f0f0f0;
      color: #333;
    }
    
    .badge {
      position: absolute;
      top: 0;
      right: 0;
      background: #dc3545;
      color: white;
      border-radius: 10px;
      padding: 2px 6px;
      font-size: 10px;
      font-weight: bold;
      min-width: 16px;
      text-align: center;
    }
    
    .notification-panel-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.1);
      z-index: 1000;
      display: flex;
      justify-content: center;
      align-items: flex-start;
      padding-top: 60px;
    }
    
    .notification-panel-wrapper {
      position: relative;
      z-index: 1001;
    }
  `]
})
export class NotificationBellComponent implements OnInit, OnDestroy {
  unreadCount = 0;
  showPanel = false;
  private subscription: Subscription = new Subscription();

  constructor(
    private notificationService: NotificationService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.subscription.add(
      this.notificationService.unreadCount$.subscribe(count => {
        this.unreadCount = count;
      })
    );
    
    // Wait for authentication to be established
    this.subscription.add(
      this.authService.isLoggedIn$.subscribe(isLoggedIn => {
        if (isLoggedIn) {
          setTimeout(() => {
            this.notificationService.loadUnreadCount();
          }, 500);
        }
      })
    );
    
    // Refresh count every 30 seconds if authenticated
    setInterval(() => {
      if (this.authService.isLoggedIn()) {
        this.notificationService.loadUnreadCount();
      }
    }, 30000);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  togglePanel() {
    this.showPanel = !this.showPanel;
  }

  closePanel() {
    this.showPanel = false;
  }
}