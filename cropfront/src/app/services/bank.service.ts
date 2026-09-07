import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BankService {
  private apiUrl = 'http://localhost:5035/api/BankAccount';

  constructor(private http : HttpClient) { }

  getBankDetails():Observable<any []>{
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}`, { headers });
  }


  postBankDetails(bank: any): Observable<any> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }
  const headers = new HttpHeaders()
    .set('Authorization', `Bearer ${token}`)
    .set('Content-Type', 'application/json');  // <-- Add this header

  return this.http.post<any>(`${this.apiUrl}`, bank, { headers });
}

editBankDetails(updatedBankDetails: any): Observable<any> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }
  const headers = new HttpHeaders()
    .set('Authorization', `Bearer ${token}`)
    .set('Content-Type', 'application/json');  // <-- Add this header

  return this.http.put<any>(`${this.apiUrl}`, updatedBankDetails, { headers });
}
}
