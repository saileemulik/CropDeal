import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { Negotiation } from '../models/priceNegotiation';

@Injectable({
  providedIn: 'root'
})
export class PricenegotiationService {
  private baseUrl = 'http://localhost:5035/api/PriceNegotiation';
  private negotiationAcceptedSource = new Subject<any>();
  negotiationAccepted$ = this.negotiationAcceptedSource.asObservable();

  notifyNegotiationAccepted(data: any) {
    this.negotiationAcceptedSource.next(data);
  }

  constructor(private http: HttpClient) { }

  createNegotiation(payload: any): Observable<Negotiation> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<Negotiation>(this.baseUrl, payload, {headers});
  }

  getNegotiationsForFarmer(farmerId: string): Observable<Negotiation[]> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<Negotiation[]>(`${this.baseUrl}/farmer/${farmerId}`, {headers});
  }

  getNegotiationsForDealer(dealerId: string): Observable<Negotiation[]> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<Negotiation[]>(`${this.baseUrl}/dealer/${dealerId}`, {headers});
  }

  acceptNegotiation(id: string): Observable<any> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put(`${this.baseUrl}/${id}/accept`, {}, {headers});
  }

  rejectNegotiation(id: string): Observable<any> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put(`${this.baseUrl}/${id}/reject`, {}, {headers});
  }
}
