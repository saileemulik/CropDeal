import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  private apiUrl = 'http://localhost:5035/api/Report';
  constructor(private http: HttpClient) { }

 generateDealerReport(dealerId: string, filter: string | null): Observable<any> {
  const token = localStorage.getItem('token') ;
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);


  return this.http.post<any>(`${this.apiUrl}/Admin/GenerateReport/${dealerId}`, filter, { headers });
}

getAllReports(): Observable<any[]> {
  const token = localStorage.getItem('token') ;
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  return this.http.get<any[]>(`${this.apiUrl}/Admin/GetAllReports`, { headers });
}

getReportById(id: string): Observable<any> {
  const token = localStorage.getItem('token') ?? '';
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

  return this.http.get<any>(`${this.apiUrl}/Admin/GetReportById/${id}`, { headers });
}

 downloadDealerReport(reportId: string) {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get(`${this.apiUrl}/export/${reportId}`, {
      headers,
      responseType: 'blob'
    });
  }


}
