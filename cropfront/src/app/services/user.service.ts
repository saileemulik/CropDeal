import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'http://localhost:5035/api/UserProfile';

  constructor(private http: HttpClient) {}


  getUserProfile(): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {

      console.error('Token is missing.');

    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<any>(`${this.apiUrl}/UserProfile`, { headers });
  }

  updateUserProfile(userData: any): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
      return new Observable();
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put(`${this.apiUrl}/UpdateUserProfile`, userData, { headers });
  }

}
