export interface Transaction {
    id : string;
    
    cropName: string;
    quantity: number;
    pricePerKg: number;
    totalPrice: number;
    orderId: string;
    currency:string;
    status: string;
    createdAt: string;
  }
  

export enum TransactionStatus
{
    Pending = 'Pending',
    Completed = 'Completed',
    Cancelled = 'Cancelled',
}