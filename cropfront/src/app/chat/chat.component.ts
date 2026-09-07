import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="chat-btn" (click)="toggle()" *ngIf="!open">💬</div>
    
    <div class="chat-box" *ngIf="open">
      <div class="chat-header">
        <span>CropDeal Assistant</span>
        <button (click)="toggle()">×</button>
      </div>
      
      <div class="chat-messages">
        <div *ngFor="let msg of messages" [class]="msg.isUser ? 'user-msg' : 'bot-msg'">
          {{msg.text}}
        </div>
      </div>
      
      <div class="chat-input">
        <input [(ngModel)]="message" (keyup.enter)="send()" placeholder="Type message...">
        <button (click)="send()">Send</button>
      </div>
    </div>
  `,
  styles: [`
    .chat-btn {
      position: fixed;
      bottom: 20px;
      right: 20px;
      width: 60px;
      height: 60px;
      background: #4CAF50;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      font-size: 24px;
      z-index: 1000;
    }
    
    .chat-box {
      position: fixed;
      bottom: 20px;
      right: 20px;
      width: 300px;
      height: 400px;
      background: white;
      border: 1px solid #ddd;
      border-radius: 10px;
      display: flex;
      flex-direction: column;
      z-index: 1000;
    }
    
    .chat-header {
      background: #4CAF50;
      color: white;
      padding: 10px;
      display: flex;
      justify-content: space-between;
      border-radius: 10px 10px 0 0;
    }
    
    .chat-messages {
      flex: 1;
      padding: 10px;
      overflow-y: auto;
    }
    
    .user-msg {
      background: #e3f2fd;
      padding: 8px;
      margin: 5px 0;
      border-radius: 10px;
      text-align: right;
    }
    
    .bot-msg {
      background: #f5f5f5;
      padding: 8px;
      margin: 5px 0;
      border-radius: 10px;
    }
    
    .chat-input {
      display: flex;
      padding: 10px;
      border-top: 1px solid #ddd;
    }
    
    .chat-input input {
      flex: 1;
      padding: 8px;
      border: 1px solid #ddd;
      border-radius: 5px;
      margin-right: 5px;
    }
    
    .chat-input button {
      padding: 8px 15px;
      background: #4CAF50;
      color: white;
      border: none;
      border-radius: 5px;
      cursor: pointer;
    }
  `]
})
export class ChatComponent {
  open = false;
  message = '';
  messages: {text: string, isUser: boolean}[] = [
    {text: 'Hi! How can I help you with CropDeal?', isUser: false}
  ];

  constructor(private http: HttpClient) {}

  toggle() {
    this.open = !this.open;
  }

  send() {
    if (!this.message.trim()) return;
    
    this.messages.push({text: this.message, isUser: true});
    const userMsg = this.message;
    this.message = '';
    
    this.http.post<{reply: string}>('http://localhost:5035/api/chat', {message: userMsg})
      .subscribe({
        next: (res) => this.messages.push({text: res.reply, isUser: false}),
        error: () => this.messages.push({text: 'Error occurred', isUser: false})
      });
  }
}