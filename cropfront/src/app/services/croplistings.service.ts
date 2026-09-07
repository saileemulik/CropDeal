import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CropListingsService {

  private apiUrl = 'http://localhost:5035/api/CropListing';

  constructor(private http: HttpClient) {}

  getAllCropListings(): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any>(`${this.apiUrl}/all`, { headers });
  }

  getAll(): Observable<any>{
    const token = localStorage.getItem('token');
    if(!token){
      console.error("Token is misssing.");
    }
    const headers = new HttpHeaders().set('Authorization',`Bearer ${token}`);
    return this.http.get<any>(`${this.apiUrl}`, {headers});
  }

  getCropById(id: string): Observable<any>{
    const token = localStorage.getItem('token');
    if(!token){
      console.error("Token is misssing.");
    }
    const headers = new HttpHeaders().set('Authorization',`Bearer ${token}`);
    return this.http.get<any>(`${this.apiUrl}/${id}`, {headers});
  }

  addCropListing(crop: any): Observable<any> {
     const token = localStorage.getItem('token');
    if(!token){
      console.error("Token is misssing.");
    }
    const headers = new HttpHeaders().set('Authorization',`Bearer ${token}`);

    return this.http.post<any>(`${this.apiUrl}`, crop, { headers });
  }


  updateCropListing(id: string, crop: any): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put<any>(`${this.apiUrl}/${id}`, crop, { headers });
  }


  deleteCropListing(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete<any>(`${this.apiUrl}/${id}`, { headers });
  }


  getMyListings(): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any>(`${this.apiUrl}/MyListings`, { headers });
  }

  getListingsByDealerLocation(dealerId: string): Observable<any> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }

  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  return this.http.get<any>(`${this.apiUrl}/Dealer/${dealerId}`, { headers });
}
}
