export interface Negotiation {
  id?: string;
  listingId: number;
  dealerId: string;
  farmerId: string;
  quantity: number;
  negotiatedPricePerKg: number;
  totalPrice: number;
  status?: string;
}

export enum PriceStatus
{
    Pending = 'Pending',
    Accepted = 'Accepted',
    Rejected = 'Rejected',
}