import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { RequestStatus } from '../models/crop';

@Injectable({
  providedIn: 'root',
})
export class CropService {
  private apiUrl = 'http://localhost:5035/api';

  constructor(private http: HttpClient) {}

  addCrop(crop: any, type: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/Crop?type=${type}`, crop, {
      headers,
    });
  }

  deleteCrop(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.apiUrl}/Crop/${id}`, { headers });
  }
  updateCrop(id: string, crop: any, type: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
      return throwError(() => new Error('Token is missing'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const url = `${this.apiUrl}/Crop/${id}?type=${type}`;

    return this.http.put<any>(url, crop, { headers });
  }

  getAllCrops(): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.apiUrl}/Crop`, { headers });
  }

   requestCrop(cropRequest: any): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
      return throwError(() => new Error('Token is missing'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(`${this.apiUrl}/Crop/CropRequest`, cropRequest, {
      headers,
    });
  }


  getPendingCropRequests(): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
      return throwError(() => new Error('Token is missing'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.apiUrl}/Crop/GetCropRequests`, { headers });
  }
getMyCropRequests(): Observable<any[]> {
   const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
      return throwError(() => new Error('Token is missing'));
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  return this.http.get<any[]>(`${this.apiUrl}/Crop/MyCropRequests`, { headers });
}


 handleCropRequest(requestId: string, status: RequestStatus, cropDto: any) {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
    return throwError(() => new Error('Token is missing'));
  }

  const headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  });

  return this.http.post(
    `${this.apiUrl}/Crop/ApproveRequest/${requestId}?status=${status}`,
    cropDto,
    { headers }
  );
}

getCropRequestStatus(): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get(`${this.apiUrl}/Crop/MyCropRequests`, { headers });
  }



}
