import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {

  private apiUrl = 'http://localhost:5035/api/Subscription';

  constructor(private http: HttpClient) {}

  postSubscription(subscriptionData: any): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {

      console.error('Token is missing.');

    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(`${this.apiUrl}`, subscriptionData, { headers });
  }

  deleteSubscription(cropListingId: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete<any>(`${this.apiUrl}/${cropListingId}`, { headers });
  }

  getMySubscriptions(): Observable<any[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}`, { headers });
  }


  getSubscribedListings(): Observable<any[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/Farmer/SubscribedListings`, { headers });
  }


  subscribeToCropName(cropName: string): Observable<any> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/subscribe-cropname`, { cropName }, { headers });
  }

  // Unsubscribe from crop name/type
  unsubscribeFromCropName(cropName: string): Observable<any> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.apiUrl}/unsubscribe-cropname?cropName=${cropName}`, { headers });
  }

  getCropNameSubscriptions(): Observable<string[]> {
  const token = localStorage.getItem('token');
  console.log('Token used:', token); // 👈 Add this
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  const url = `${this.apiUrl}/dealer-cropname-subscriptions`;
  console.log('Sending GET to:', url);
  return this.http.get<string[]>(url, { headers });
}


}

