import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

export interface Notification {
  id: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = 'http://localhost:5035/api/Notification';
  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  constructor(private http: HttpClient) {}

  getNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>(this.apiUrl);
  }

  getUnreadCount(): Observable<{count: number}> {
    return this.http.get<{count: number}>(`${this.apiUrl}/unread-count`);
  }

  markAsRead(id: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/read`, {});
  }

  markAllAsRead(): Observable<any> {
    return this.http.put(`${this.apiUrl}/mark-all-read`, {});
  }

  updateUnreadCount(count: number) {
    this.unreadCountSubject.next(count);
  }

  loadUnreadCount() {
    this.getUnreadCount().subscribe({
      next: (response) => {
        this.updateUnreadCount(response.count);
      },
      error: (error) => {
        // Silently handle auth errors - user might not be logged in
        if (error.status !== 401) {
          console.error('Error loading notification count:', error);
        }
      }
    });
  }
}
