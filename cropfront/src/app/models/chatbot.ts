export interface ChatbotRequest {
  message: string;
  userId?: string;
}

export interface ChatbotResponse {
  response: string;
  timestamp: Date;
}

export interface ChatMessage {
  text: string;
  isUser: boolean;
  timestamp: Date;
}