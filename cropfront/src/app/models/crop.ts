export interface Crop {
    id: string;
    name: string;
    type: string;
  }

export enum CropAvailability
  {
      Available ='Available',
      OutOfStock ='OutOfStock'
  }

 
export enum CropTypeEnum {
  Grain = 'Grain',
  Fruit = 'Fruit',
  Vegetable = 'Vegetable'
}
export enum RequestStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected'
}


  