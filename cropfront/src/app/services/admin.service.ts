import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private apiUrl = 'http://localhost:5035/api/Admin';

  constructor(private http: HttpClient) { }

  getAllUsers(): Observable<any[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/GetAllUsers`, { headers });
  }

  editUsers(id : string,  updatedUser: any): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {

      console.error('Token is missing.');

    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put<any>(`${this.apiUrl}/EditUser/${id}`,updatedUser,  { headers });
  }

  deleteUser(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete<any>(`${this.apiUrl}/DeleteUser/${id}`, { headers });
  }
   getDashBoard(): Observable<any[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/DashboardStats`, { headers });
  }

}
