import { Component } from '@angular/core';
import { TransactionService } from '../../app/services/transaction.service';
import { TransactionStatus } from '../../app/models/transaction';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PaymentService } from '../../app/services/payment.service';
import { v4 as uuidv4 } from 'uuid';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { Router } from '@angular/router';
import { CropListingsService } from '../../app/services/croplistings.service';
import { RatingService } from '../../app/services/rating.service';
import { SnackbarService } from '../../app/services/snackbar.service';
declare var Razorpay: any;
@Component({
  selector: 'app-transaction',
  imports:[FormsModule, CommonModule],
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.css'] 
})
export class TransactionComponent {
  transactionData: any;
  showForm = false;
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  activeSection: string = '';
  user: any = {};
  editableTransaction: any = null;
  isTransactionEdit: boolean = false;
  showTransactions : boolean = true;
  isFormVisible : boolean = false;
  listings: any = [];
  transactions: any[] = [];
  trans = {
    id: '',
    listingId: '',
    quantity: 0,
    pricePerKg: 0,
    totalPrice: 0,
    orderId: '',
    currency: '',
    status: 'Pending', // Add status here to resolve the error
  };
   rating: any = {
    transactionId: '',
    farmerId: '',
    rating: null,
    comment: '',
  };
  showRating: boolean = true;
  isReviewEdit: boolean = false;
  isReviewFormVisible = false;

  TransactionStatus = TransactionStatus;
  statusOptions = Object.values(TransactionStatus);

  constructor(private transactionService: TransactionService, private router : Router, private snackbar :SnackbarService,
     private paymentService : PaymentService, private loginService : LoginService, private cropListingService : CropListingsService, private ratingService : RatingService) {}

  ngOnInit(): void {
    this.loadListings();
    this.transactionData = this.transactionService.getTransactionData();
  if (this.transactionData) {
    this.showForm = true;
  } else {
    this.showForm = false;
    this.getTransactions();
  }

  this.currentUserRole = this.loginService.getUserRole() as UserRole;
  this.activeSection = 'transaction';
}



  ngOnDestroy() {
  this.transactionService.hideTransactionForm();
}

  showTransForm(listingId: string) {
      this.trans = {
        id: '',
        listingId: listingId,
        quantity: 0,
        pricePerKg: 0,
        orderId: '',
        currency: 'INR',
        totalPrice: 0,
        status: TransactionStatus.Pending,
      };
      this.showTransactions=false;
      this.isFormVisible = true;
      this.activeSection = 'transaction';
      const selectedListing = this.listings.find((l: any) => l.id === listingId);
      if (selectedListing) {
        this.trans.totalPrice = 0;
      }
      
    }

    loadListings(): void {
  this.cropListingService.getAll().subscribe({
    next: (data) => {
      this.listings = data;
      console.log(this.listings);
    },
    error: (err) => console.error('Error loading listings', err)
  });
}

  getTransactions(): void {
  const role = this.loginService.getUserRole() as UserRole; 

  if (role === 'Admin') {
    this.transactionService.getAllTransactionsAdmin().subscribe({
      next: (data: any) => {
        this.transactions = data.allTxns;
        this.showTransactions = true;
        console.log('Admin Transactions:', data);
      },
      error: (err) => console.error('Error fetching admin transactions:', err),
    });
  } else if (role === 'Dealer') {
    this.transactionService.getMyTransactionsDealer().subscribe({
      next: (data: any) => {
        this.transactions = data.txns;
        this.showTransactions = true;
        console.log('Dealer Transactions:', data);
      },
      error: (err) => console.error('Error fetching dealer transactions:', err),
    });
  } else if(role =='Farmer')
  {
    this.transactionService.getMyTransactionsFarmer().subscribe({
      next: (data) => {
        console.log('Raw API Response:', data);
        console.log('Data type:', typeof data);
        console.log('Is Array:', Array.isArray(data));

        // Ensure correct assignment
        this.transactions = data.txns;
      },
      error: (err) => console.error('Error fetching transactions:', err),
    });
  }
   else {
    console.warn('Unsupported role:', role);
  }
}


  editTransaction(transaction: any): void {
    this.isTransactionEdit = true;
    this.editableTransaction = { ...transaction };
  }

  updateTransaction(): void {
    if (!this.editableTransaction?.id) {
      this.snackbar.warning("No transaction selected for update.");
      return;
    }

    this.transactionService
      .updateTransactionStatus(this.editableTransaction.id, this.editableTransaction.status)
      .subscribe({
        next: () => {
          this.snackbar.success('Transaction Status updated successfully');
          this.isTransactionEdit = false;
          this.editableTransaction = null;
          this.getTransactions();
        },
        error: (err) => {
          console.error('Update error:', err);
          this.snackbar.error('Failed to update transaction');
        },
      });
  }

   get selectedListing() {
      return this.listings.find((l: any) => l.id === this.trans.listingId);
    }
  
    updateTotalPrice() {
      const selected = this.listings.find(
        (l: any) => l.id === this.trans.listingId
      );
      if (selected && this.trans.quantity) {
        this.trans.totalPrice =
          Number(this.trans.quantity) * Number(selected.pricePerKg);
      } else {
        this.trans.totalPrice = 0;
      }
    }


 

  downloadInvoice(transactionId: string): void {
    this.transactionService.downloadInvoice(transactionId);
  }

   startReview(transaction: any): void {
    this.activeSection = 'rating'; // navigate to rating section
    console.log('Transaction passed to startReview:', transaction);

   this.isReviewFormVisible = true;
  this.showRating = false;
  this.rating = {
    transactionId: transaction.id,
    farmerId: transaction.farmerId,
    rating: null,
    comment: ''
  };

}

setStarRating(star: number) {
  this.rating.rating = star;
}

setStarRatingHover(event: MouseEvent, star: number) {
  const rect = (event.target as HTMLElement).getBoundingClientRect();
  const x = event.clientX - rect.left;
  const width = rect.width;

  this.rating.rating = x < width / 2 ? star - 0.5 : star;
}

clearStarRatingHover() {

}



  submitReview() {
  if (!this.rating.transactionId || !this.rating.farmerId || !this.rating.rating) {
    this.snackbar.warning('Please fill all required fields for rating.');
    return;
  }

  this.ratingService.postRating(this.rating).subscribe({
    next: (res) => {
      this.snackbar.success('Review submitted successfully');
      this.isReviewFormVisible = false;
      this.router.navigate(['/rating']);
      // this.getRating(); // Refresh list of ratings
      // Reset form
      this.rating = {
        transactionId: '',
        farmerId: '',
        rating: null,
        comment: '',
      };
       this.router.navigate(['/rating']);
    },
    error: (err) => {
      console.error('Failed to submit review:', err);
      this.snackbar.error('Failed to submit review');
    },
  });
}
}
