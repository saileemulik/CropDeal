export interface PickupSchedule {
  id?: string;
  listingId: string;
  farmerId: string;
  dealerId: string;
  proposedPickupSlot: string;
  confirmedPickupSlot?: string;
  status: PickUpStatus
}
export enum PickUpStatus
{
    Proposed = 'Proposed',
    Confirmed = 'Confirmed',
    Scheduled = 'Scheduled',
    InTransit = 'InTransit',
    Delivered = 'Delivered',
    Cancelled = 'Cancelled',
}