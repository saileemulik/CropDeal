export interface Notification {
  id: string;
  farmerId: string;
  dealerId: string;
  cropId: string;
  cropName: string;
  message: string;  
  isRead: boolean;
  createdAt: Date;
}
