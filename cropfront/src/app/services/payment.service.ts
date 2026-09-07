import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import {CreatePaymentOrderRequestDto} from '../models/CreatePaymentOrderRequestDto';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {

  private apiUrl = 'http://localhost:5035/api/Payment';
  constructor(private http: HttpClient) {}

   createOrder(requestDto: CreatePaymentOrderRequestDto): Observable<any> {
  const token = localStorage.getItem('token');

  if (!token) {
    console.error('Token is missing.');
    return throwError(() => new Error('Token missing'));
  }

  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  console.log('Sending Payment Request:', requestDto); // Check payload

  return this.http.post<any>(`${this.apiUrl}/create-order`, requestDto, { headers });
}

    verifyPayment(data: any): Observable<any> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }

  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  console.log('Verify Payment Payload:', data);

  return this.http.post<any>(`${this.apiUrl}/verify`, data, { headers });
}

}
