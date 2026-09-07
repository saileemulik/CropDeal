import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {PickupSchedule} from '../models/pickupschedule';
@Injectable({
  providedIn: 'root'
})
export class PickupscheduleService {
   private apiUrl = 'http://localhost:5035/api/Pickup';

  constructor(private http: HttpClient) { }

  createProposal(schedule: PickupSchedule): Observable<PickupSchedule> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<PickupSchedule>(`${this.apiUrl}/propose`, schedule, {headers});
  }

  // Confirm a pickup slot
  confirmPickupSlot(scheduleId: string, confirmedSlot: string): Observable<boolean> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put<boolean>(`${this.apiUrl}/confirm/${scheduleId}`, { confirmedSlot }, {headers});
  }

  // Update status
// pickup.service.ts
updateSingleScheduleStatus(scheduleId: string, status: string): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  });

  // return the observable!
  return this.http.put(`${this.apiUrl}/status/${scheduleId}`, JSON.stringify(status), { headers });
}




  // Get pickup schedules by listing ID
  getPickupByListingId(listingId: string): Observable<PickupSchedule[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    console.log("Calling backend with:", `${this.apiUrl}/listing/${listingId}`);

    return this.http.get<PickupSchedule[]>(`${this.apiUrl}/listing/${listingId}`, {headers});
  }

  // Get pickup by ID
  getById(scheduleId: string): Observable<PickupSchedule> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<PickupSchedule>(`${this.apiUrl}/${scheduleId}`, {headers});
  }

updateProposedSlot(id: string, newSlot: Date): Observable<any> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }

  const headers = new HttpHeaders()
    .set('Authorization', `Bearer ${token}`)
    .set('Content-Type', 'application/json');

  const rawBody = `"${newSlot.toISOString()}"`; // wrap in quotes to send plain JSON string

  return this.http.put(`${this.apiUrl}/update-proposal/${id}`, rawBody, { headers });
}



  cancelProposal(id: string): Observable<any> {
     const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.apiUrl}/cancel/${id}`, {headers});
  }
}
