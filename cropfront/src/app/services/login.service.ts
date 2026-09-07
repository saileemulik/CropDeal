import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  // private apiUrl = 'http://localhost:5035/api/Auth';
  private apiUrl = 'http://localhost:5035/api/Auth';

  constructor(private http: HttpClient) { }

  login(user: { email: string, password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/signin`, user);
  }

  saveLoginData(response: any): void {
  localStorage.setItem('token', response.token);
  localStorage.setItem('role', response.role);

  const user = {
    email: response.email,
    userId: response.userId,
    role: response.role
  };
  localStorage.setItem('user', JSON.stringify(user));
}
  getUser(): any {
    const userString = localStorage.getItem('user');
    return userString ? JSON.parse(userString) : null;
  }

  getUserRole(): string {
    return localStorage.getItem('role') || '';
  }

 // auth.service.ts

forgotPassword(email: string): Observable<any> {
  return this.http.post(`${this.apiUrl}/forgot-password`, { email });
}

verifyOtp(data: { email: string, otp: string }): Observable<any> {
  return this.http.post(`${this.apiUrl}/verify-otp`, data);
}

resetPassword(data: { email: string, otp: string, newPassword: string }): Observable<any> {
  const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  return this.http.post(`${this.apiUrl}/reset-password`, data, { headers });
}


}
