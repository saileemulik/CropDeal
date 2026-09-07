import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DealerService {
private apiUrl = 'http://localhost:5035/api/Auth';
  constructor(private http: HttpClient) { }
   getDashboardStats(): Observable<any> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any>(`${this.apiUrl}/DealerDashboard`, {headers});
  }
}
