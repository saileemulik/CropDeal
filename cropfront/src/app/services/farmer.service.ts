import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FarmerService {

  private apiUrl = 'http://localhost:5035/api/Auth';  // adjust this if your controller route is different

  constructor(private http: HttpClient) { }

  getDashboardStats(): Observable<any> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any>(`${this.apiUrl}/dashboard`, {headers});
  }
}
