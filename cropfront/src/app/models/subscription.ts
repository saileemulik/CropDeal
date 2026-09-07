import { Crop } from "./crop";

export interface Subscription {
    cropListingId: string;
    crop: Crop;
    dealerName: string;  
    cropName: string;
  createdAt: Date;
}