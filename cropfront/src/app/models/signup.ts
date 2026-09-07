export interface Signup {
  id?: string;
  name: string;
  email: string;
  phoneNumber: string;
  password: string;
  confirmPassword: string;
  role: UserRole;
  location: string;
  createdAt: Date;
  updatedAt: Date;
}
export enum UserRole {
    Admin = 'Admin',
    Dealer = 'Dealer',
    Farmer = 'Farmer'
  }

export enum UserStatus
  {
      Active ='Active',
      Inactive ='Inactive'
  }
