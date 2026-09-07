export interface CreatePaymentOrderRequestDto {
  id: string;
  listingId: string;
  quantity: number;
  totalPrice: number;
  currency: string;
}