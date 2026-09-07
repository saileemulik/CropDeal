import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  private apiUrl = 'http://localhost:5035/api/Transaction';

  constructor(private http: HttpClient) {}
   private showFormSubject = new BehaviorSubject<boolean>(false);
  showForm$ = this.showFormSubject.asObservable();

  triggerShowTransactionForm() {
    this.showFormSubject.next(true);
  }
  private transactionData: any = null;

  // Add a method to reset form visibility when needed
  hideTransactionForm() {
    this.showFormSubject.next(false);
  }

  postTransaction(payload: any): Observable<any> {
    const token = localStorage.getItem('token');

    if (!token) {
      console.error('Token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(
      `${this.apiUrl}/Dealer/CreateTransaction`,
      payload,
      { headers }
    );
  }



  setOrderId(transactionId: string, orderId: string): Observable<any> {
  const token = localStorage.getItem('token');
  const headers = new HttpHeaders()
    .set('Authorization', `Bearer ${token}`)
    .set('Content-Type', 'application/json');

  const body = { orderId };

  console.log('SetOrderId Payload:', body);

  return this.http.put(
    `${this.apiUrl}/${transactionId}/set-order-id`,
    body,
    { headers }
  );
}

  updateTransactionStatus(id: string, status: string): Observable<any> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }

  const headers = new HttpHeaders({
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json' // explicitly set content type
  });

  return this.http.put<any>(
    `${this.apiUrl}/Admin/${id}/Status`,
    JSON.stringify(status), // ensures valid JSON string
    { headers }
  );
}


  getMyTransactionsDealer(): Observable<{ txns: any[] }> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  return this.http.get<{ txns: any[] }>(`${this.apiUrl}/Dealer/GetMyTransactions`, {
    headers,
  });
}

getMyTransactionsFarmer(): Observable<{ txns: any[] }> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  return this.http.get<{ txns: any[] }>(`${this.apiUrl}/Farmer/GetMyTransactions`, {
    headers,
  });
}



  getMyFarmers(): Observable<any[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/Dealer/GetMyFarmers`, {
      headers,
    });
  }

  getAllTransactionsAdmin(): Observable<any[]> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.get<any[]>(`${this.apiUrl}/Admin/GetAllTransactions`, {
      headers,
    });
  }

  payTransaction(id: string): Observable<any> {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('Token is missing.');
    }
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post(
      `${this.apiUrl}/Transaction/Dealer/PayTransaction/${id}`,
      { headers }
    );
  }

downloadInvoice(transactionId: string): void {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token missing');
    alert("You're not logged in!");
    return;
  }

  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  const url = `${this.apiUrl}/DownloadInvoice/${transactionId}`;

  this.http.get(url, {
    headers,
    responseType: 'blob'
  }).subscribe({
    next: (blob) => {
      const a = document.createElement('a');
      const objectUrl = URL.createObjectURL(blob);
      a.href = objectUrl;
      a.download = `Invoice_${transactionId}.pdf`;
      a.click();
      URL.revokeObjectURL(objectUrl);
    },
    error: (err) => {
      console.error('Invoice download failed', err);
      alert("Failed to download invoice. Please try again.");
    }
  });
}

getTransactionByDealerAndListing(dealerId: string, listingId: string): Observable<any> {
  const token = localStorage.getItem('token');
  if (!token) {
    console.error('Token is missing.');
  }
  const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  return this.http.get<any>(
    `${this.apiUrl}/Dealer/GetTransactionByDealerAndListing/${dealerId}/${listingId}`,
    { headers }
  );
}

 setTransactionData(data: any) {
    this.transactionData = data;
  }

  getTransactionData() {
    return this.transactionData;
  }

  clearTransactionData() {
    this.transactionData = null;
  }

}
