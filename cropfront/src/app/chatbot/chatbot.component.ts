import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatbotService } from '../services/chatbot.service';
import { ChatMessage } from '../models/chatbot';

@Component({
  selector: 'app-chatbot',
  imports: [CommonModule, FormsModule],
  templateUrl: './chatbot.component.html',
  styleUrl: './chatbot.component.css'
})
export class ChatbotComponent {
  messages: ChatMessage[] = [];
  currentMessage = '';
  isLoading = false;
  isOpen = false;

  constructor(private chatbotService: ChatbotService) {
    this.messages.push({
      text: 'Hello! I\'m your CropDeal assistant. How can I help you today?',
      isUser: false,
      timestamp: new Date()
    });
  }

  toggleChat() {
    this.isOpen = !this.isOpen;
  }

  sendMessage() {
    if (!this.currentMessage.trim() || this.isLoading) return;

    const userMessage: ChatMessage = {
      text: this.currentMessage,
      isUser: true,
      timestamp: new Date()
    };

    this.messages.push(userMessage);
    const messageToSend = this.currentMessage;
    this.currentMessage = '';
    this.isLoading = true;

    this.chatbotService.sendMessage({ message: messageToSend }).subscribe({
      next: (response) => {
        this.messages.push({
          text: response.response,
          isUser: false,
          timestamp: new Date(response.timestamp)
        });
        this.isLoading = false;
      },
      error: (error) => {
        this.messages.push({
          text: 'Sorry, I\'m having trouble right now. Please try again later.',
          isUser: false,
          timestamp: new Date()
        });
        this.isLoading = false;
      }
    });
  }

  onKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }
}
