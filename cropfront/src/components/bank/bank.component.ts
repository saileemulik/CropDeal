import { Component } from '@angular/core';
import { BankService } from '../../app/services/bank.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ChangeDetectorRef } from '@angular/core';
import { UserRole } from '../../app/models/signup';
import { Router, RouterLink } from '@angular/router';
import { LoginService } from '../../app/services/login.service';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-bank',
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './bank.component.html',
  styleUrl: './bank.component.css'
})
export class BankComponent {
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  activeSection: string = '';
  
  showBankDetails: boolean = false;
  isBankEdit: boolean = false;
  isBankFormVisible = false;
 bank: any = {
  accounts: {
    accountNumber: '',
    ifscCode: '',
    bankName: '',
    branchName: '',
  }
};

   constructor(private bankService: BankService,  private cdRef: ChangeDetectorRef,public router : Router, private loginService : LoginService, private snackbar: SnackbarService) {}
   isActive(route: string): boolean {
  return this.router.url === route;
}
     
       ngOnInit(): void {
        this.currentUserRole = this.loginService.getUserRole() as UserRole;
         this.activeSection= 'bank';
         this.getBankDetails();
       }
showForm() {
    this.isBankFormVisible = true;
  }
       
     addBankDetails(): void {
  const payload = this.bank.accounts;  // <-- send only accounts object
  console.log('Payload being sent:', payload);

  this.bankService.postBankDetails(payload).subscribe({
    next: () => {
      this.snackbar.success('Bank Details added successfully');
      this.isBankFormVisible = false;
      this.getBankDetails();
    },
    error: (err) => {
      console.error('Failed to add bank details:', err);
      this.snackbar.error('Failed to add bank details');
    },
  });
}


updateBankDetails(): void {
  const payload = this.bank.accounts;
  console.log("Payload being sent:", payload);

  this.bankService.editBankDetails(payload).subscribe({
    next: (response) => {
      this.snackbar.success('Bank Details updated successfully');
      this.getBankDetails();
      this.cdRef.detectChanges();
      this.isBankFormVisible = false;
      this.isBankEdit = false;
    },
    error: (error) => {
      console.error('Error updating Bank Details', error);
      this.snackbar.error('There was an error updating the Bank Details.');
    }
  });
}



 getBankDetails() {
  this.bankService.getBankDetails().subscribe({
    next: (data : any) => {
      if (data && data.accounts && Array.isArray(data.accounts) && data.accounts.length > 0) {
        this.bank.accounts = data.accounts[0];  
      } else {
        this.bank.accounts = {
          accountNumber: '',
          ifscCode: '',
          bankName: '',
          branchName: ''
        };
      }

      this.showBankDetails = true;
      this.cdRef.detectChanges();
      console.log('Bank details loaded:', this.bank.accounts);
    },
    error: (error) => {
      this.showBankDetails = false;
      console.error('Error fetching Bank Details:', error);
    }
  });
}




}
