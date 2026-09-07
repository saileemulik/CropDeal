import { Component, OnInit } from '@angular/core';
import { Negotiation } from '../../app/models/priceNegotiation';
import { PricenegotiationService } from '../../app/services/pricenegotiation.service';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-negotiations',
  templateUrl: './negotiations.component.html',
  imports: [CommonModule, RouterLink, FormsModule],
  styleUrls: ['./negotiations.component.css']
})
export class NegotiationsComponent implements OnInit {

  negotiations: Negotiation[] = [];
  cropName: string = '';
  totalPrice: number = 0;
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  negotiationView: 'pending' | 'history' = 'pending';
  pendingNegotiations: any[] = [];
historyNegotiations: any[] = [];

  farmerId?: string;
  dealerId?: string;

 negotiation = {
  farmerId: '',
  dealerId: '',
  cropListingId: 0,
  proposedPrice: 0,
  listingId: 0,
  quantity: 0,
  negotiatedPricePerKg: 0,
  totalPrice: 0
};

  constructor(
    private negotiationService: PricenegotiationService,
    private loginService: LoginService,
    private router: Router,
    private snackbar: SnackbarService
  ) { }

  ngOnInit() {
  this.currentUserRole = this.loginService.getUserRole() as UserRole;

  const userId = localStorage.getItem('userId');

  if (!userId) {
    console.error('User ID not found in localStorage!');
    return;
  }

  if (this.currentUserRole === UserRole.Farmer) {
    this.farmerId = userId;
    this.negotiation.farmerId = userId;
  } else if (this.currentUserRole === UserRole.Dealer) {
    this.dealerId = userId;
    this.negotiation.dealerId = userId;
  }

  this.loadNegotiations(); // Only call after userId is set
}

  negotiate(listing: any) {
  this.negotiation = {
    farmerId: listing.farmerId,
    dealerId: this.dealerId || '', 
    cropListingId: listing.id,     
    proposedPrice: 0,            
    listingId: listing.id,
    quantity: listing.quantity,
    negotiatedPricePerKg: 0,      
    totalPrice: 0                  
  };

  this.cropName = listing.cropName;
}

  loadNegotiations() {
  if (this.currentUserRole === UserRole.Farmer) {
    if (!this.farmerId) {
      console.error('Farmer Id is undefined');
      return;
    }

    this.negotiationService.getNegotiationsForFarmer(this.farmerId).subscribe({
      next: (data) => {
        this.negotiations = data;
        this.pendingNegotiations = data.filter(n => n.status === 'Pending');
        this.historyNegotiations = data.filter(n => n.status !== 'Pending');
      },
      error: (err) => console.error('Failed to load negotiations for farmer', err),
    });

  } else if (this.currentUserRole === UserRole.Dealer) {
    if (!this.dealerId) {
      console.error('Dealer Id is undefined');
      return;
    }

    this.negotiationService.getNegotiationsForDealer(this.dealerId).subscribe({
      next: (data) => {
        this.negotiations = data;
        this.pendingNegotiations = data.filter(n => n.status === 'Pending');
        this.historyNegotiations = data.filter(n => n.status !== 'Pending');
      },
      error: (err) => console.error('Failed to load negotiations for dealer', err),
    });
  }
}

toggleNegotiationView(view: 'pending' | 'history') {
  this.negotiationView = view;
}

 acceptNegotiation(id?: string) {
  if (!id) {
    console.error('Invalid negotiation ID:', id);
    return;
  }

  this.negotiationService.acceptNegotiation(id).subscribe({
    next:negotiation => {
      this.snackbar.success('Negotiation accepted.');

      // Notify subscribers with negotiation info (including price, quantity, listingId, dealerId)
      this.negotiationService.notifyNegotiationAccepted(negotiation);

      this.loadNegotiations();
    },
    error: (err) => {
      console.error('Accept failed', err);
      this.snackbar.error('Failed to accept negotiation.');
    }
  });
}

rejectNegotiation(id?: string) {
  if (!id) {
    console.error('Invalid negotiation ID:', id);
    return;
  }

  this.negotiationService.rejectNegotiation(id).subscribe({
    next: () => {
      this.snackbar.success('Negotiation rejected.');
      this.loadNegotiations();
    },
    error: (err) => {
      console.error('Reject failed', err);
      this.snackbar.error('Failed to reject negotiation.');
    }
  });
}

navigateToNegotiations(): void {
  this.router.navigate(['/subscription']);
}
  submitNegotiation(): void {
  this.negotiation.totalPrice = this.negotiation.proposedPrice * this.negotiation.quantity;

  this.negotiationService.createNegotiation(this.negotiation).subscribe({
    next: () => {
      this.snackbar.success("Negotiation submitted successfully!");
      this.negotiation = {
        farmerId: '',
        dealerId: '',
        cropListingId: 0,
        proposedPrice: 0,
        listingId: 0,
        quantity: 0,
        negotiatedPricePerKg: 0,
        totalPrice: 0
      };
    },
    error: (err) => {
      console.error('Negotiation failed', err);
      this.snackbar.error("Failed to submit negotiation");
    }
  });
}
clearNegotiationForm(): void {
  this.negotiation = {
    cropListingId: 0,
    farmerId: '',
    dealerId: '',
    proposedPrice: 0,
   listingId: 0,
        quantity: 0,
        negotiatedPricePerKg: 0,
        totalPrice: 0
  };
  this.cropName = ''; // Make sure cropName is declared in the component
}

}
