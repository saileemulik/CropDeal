import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AddressService } from '../../app/services/address.service';
import { Router, RouterLink } from '@angular/router';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-address',
  standalone:true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './address.component.html',
  styleUrl: './address.component.css'
})
export class AddressComponent {
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  activeSection: string = '';

  constructor(private addressService : AddressService, public router : Router, private loginService : LoginService, private snackbar : SnackbarService) {}
  isActive(route: string): boolean {
  return this.router.url === route;
}

  address: any = {
    street: '',
    city: '',
    state: '',
    zipCode: ''
  };
  isEdit : boolean = false;
  isFormVisible = false;
  showAddress : boolean = false;

  ngOnInit(): void {
    this.currentUserRole = this.loginService.getUserRole() as UserRole;
    this.activeSection = 'address';
    this.getAddress();
  }

  showForm() {
    this.isFormVisible = true;
  }
  


  addAddress() : void {
    this.addressService.postAddress(this.address).subscribe({
      next: ()=>{
        this.snackbar.success('Address added successfully');
        this.isFormVisible=false;
        this.getAddress();

      },
      error: (err) => {
        console.error('Failed to add address:', err);
        this.snackbar.error('Failed to add address');
      }
    })
  }

  updateAddress(): void {
    const updatedAddress = this.address;
  
    this.addressService.editAddress(updatedAddress).subscribe(
      (response) => {
        console.log(response); 
        this.snackbar.success('Address updated successfully');     
        this.getAddress();
        this.isFormVisible=false; 
        this.isEdit = false;
      },
      (error) => {
        console.error('Error updating address', error);
        this.snackbar.error('There was an error updating the address.');
      }
    );
  }

  getAddress() {
   
    this.addressService.getAddress().subscribe(
      (data) => {
        this.address = data; 
        
        this.showAddress = true;
        console.log(this.address); 
        
      },
      (error) => {
        this.showAddress= false;
        console.error('Error fetching address:', error);
      }
    );
  }
}
