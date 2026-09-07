import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Signup } from '../../app/models/signup';

@Injectable({
  providedIn: 'root'
})
export class SignupService {
  private apiUrl = 'http://localhost:5035/api/Auth/signup';

  constructor(private http: HttpClient) { }

  signUp(user: Signup, role: string): Observable<any> {
    const params = new HttpParams().set('role', role);
    return this.http.post<any>(this.apiUrl, user, { params });
  }
}
