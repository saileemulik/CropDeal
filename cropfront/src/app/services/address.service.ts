import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  private apiUrl = 'http://localhost:5035/api/Address';

  constructor(private http : HttpClient) { }

  getAddress():Observable<any []>{
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}`, { headers });
  }


  postAddress(address: any): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {

      console.error('Token is missing.');

    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(`${this.apiUrl}`,address,  { headers });
  }


  editAddress( updatedAddress: any): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');

    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put<any>(`${this.apiUrl}`,updatedAddress,  { headers });
  }
}
