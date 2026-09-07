import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RatingService {

  private apiUrl = 'http://localhost:5035/api/Review';
  constructor(private http : HttpClient) { }

   getRating():Observable<any []>{
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/Dealer/GetOwnReviews`, { headers });
  }

  getAllRating():Observable<any []>{
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/Admin/GetAllReviews`, { headers });
  }

  // updateReview(id: string, review: any): Observable<any> {
  //   const token = localStorage.getItem('token');
  //   if (!token) {
  //     console.error('Token is missing.');
  //   }

  //   const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  //   return this.http.put<any>(`${this.apiUrl}/${id}`, review, { headers });
  // }


  deleteReview(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete<any>(`${this.apiUrl}/Admin/DeletReview/${id}`, { headers });
  }


  getRatingFarmer(): Observable<{ averageRating: number, reviews: any[] }> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  return this.http.get<{ averageRating: number, reviews: any[] }>(
    `${this.apiUrl}/Farmer/ReceivedReviews`, { headers }
  );
}


  postRating(rating: any): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {

      console.error('Token is missing.');

    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(`${this.apiUrl}/Dealer/AddReview`,rating,  { headers });
  }
}
