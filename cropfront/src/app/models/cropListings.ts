import { Crop, CropAvailability } from "./crop";

export interface CropListings {
    id: string;
    cropId: string;
    description: string;
    quantity: number;
    pricePerKg: number;
    unit: string;
    status: CropAvailability;
    location : string;
    crop: Crop;
    locationCategory: 'City' | 'State' | 'Other';
    imageBase64?: string;
    farmerId: string; 
  }