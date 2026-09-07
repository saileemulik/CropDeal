import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChatbotRequest, ChatbotResponse } from '../models/chatbot';

@Injectable({
  providedIn: 'root'
})
export class ChatbotService {
  private apiUrl = 'https://localhost:7035/api/chatbot';

  constructor(private http: HttpClient) { }

  sendMessage(request: ChatbotRequest): Observable<ChatbotResponse> {
    return this.http.post<ChatbotResponse>(`${this.apiUrl}/chat`, request);
  }
}