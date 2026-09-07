import { Component, ElementRef, HostListener } from '@angular/core';
import { SubscriptionService } from '../../app/services/subscription.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TransactionStatus } from '../../app/models/transaction';
import { TransactionService } from '../../app/services/transaction.service';
import { Router, RouterLink } from '@angular/router';
import { CropService } from '../../app/services/crop.service';
import { CropListingsService } from '../../app/services/croplistings.service';
import { AddressService } from '../../app/services/address.service';
import { UserService } from '../../app/services/user.service';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { PaymentService } from '../../app/services/payment.service';
import { v4 as uuidv4 } from 'uuid';
import { Subscription  as CropSubscription} from '../../app/models/subscription';
import { NotificationService } from '../../app/services/notification.service';
import { NotificationBellComponent } from '../../app/notification-bell/notification-bell.component';
import { Negotiation } from '../../app/models/priceNegotiation';
import { PricenegotiationService } from '../../app/services/pricenegotiation.service';
import { Subscription} from 'rxjs';
import { PickupSchedule, PickUpStatus } from '../../app/models/pickupschedule';
import { PickupscheduleService } from '../../app/services/pickupschedule.service';
import { SnackbarService } from '../../app/services/snackbar.service';
declare var Razorpay: any;
@Component({
  selector: 'app-subscription',
  imports: [FormsModule, CommonModule, RouterLink, NotificationBellComponent],
  templateUrl: './subscription.component.html',
  styleUrl: './subscription.component.css',
})

export class SubscriptionComponent {
  pickupStatuses: { [listingId: string]: PickUpStatus } = {};
  pickupSchedules: { [listingId: string]: PickupSchedule[] } = {};
PickUpStatus = PickUpStatus;
  negotiations: Negotiation[] = [];
    cropName: string = '';
     farmerId?: string;
  dealerId?: string;
  activeSection: string = '';
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  user: any = {};
  cropListings: any[] = [];
  subscribedListing: Set<string> = new Set();
  subscriptions: any[] = [];
  listings: any = [];
  crops: any[] = [];
  users: any[] = [];
  listingPricePerKg: number = 0;
  availableQuantity: number = 0; 
  quantityError = false;
  showTransactions: boolean = true;
  isFormVisible: boolean = false;
  isListingVisible : boolean = true;
  loading = false;
  transactions: any[] = [];
  subscribedListings: CropSubscription[] = [];
  cropNameSubscriptions: any[] = [];
  fullNegotiations: Negotiation[] = []; 
  selectedCropWithListings: string | null = null;
filteredListings: any[] = [];
  newCropName: string = '';
  selectedView: 'listing' | 'crop' = 'listing'; 
   isNegoForm: boolean = false;
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

  subscribedCropNames : string[] = [];
currentUserId: string = '';
negotiatedListings = new Set<number>()
negotiationSub?: Subscription;
currentNegotiation?: any; 

  constructor(
    private subscriptionService: SubscriptionService,
    private cropService: CropService,
    private cropListingService: CropListingsService,
    private addressService: AddressService,
    private loginService: LoginService,
    private userProfileService: UserService,
    private paymentService: PaymentService,
    private transactionService: TransactionService,
    private notificationService: NotificationService,
    private pickupService: PickupscheduleService,
    private elementRef : ElementRef,
    private negotiationService: PricenegotiationService,
    private router: Router,
    private snackbar: SnackbarService
  ) {}
@HostListener('document:click', ['$event'])
onDocumentClick(event: MouseEvent): void {
  // Notification panel is now handled by NotificationBellComponent
}

  ngOnInit(): void {
    this.currentUserRole = this.loginService.getUserRole() as UserRole;
    this.currentUserId = localStorage.getItem('userId') || '';


    this.activeSection = 'subscription';
    this.loadUserProfile();
    if(this.currentUserRole == 'Dealer')
    {
      this.negotiationSub = this.negotiationService.negotiationAccepted$.subscribe(negotiation => {
      this.currentNegotiation = negotiation; // Save if you want to use later

      if (negotiation && negotiation.cropListingId === this.trans.listingId) {
        this.trans.quantity = negotiation.negotiatedQuantity || this.trans.quantity;
        this.trans.totalPrice =
          (negotiation.negotiatedPricePerKg || this.getSelectedListingPrice()) * this.trans.quantity;

        console.log('Transaction updated from negotiation:', this.trans);
      }
    });
      this.loadCropsListingsAndSubscriptions();
    // this.getlistings();
     this.loadNegotiations();
     this.loadFullNegotiations();
    this.getMySubscriptions();

    }
     if(this.currentUserRole == 'Farmer')
    {
      this.getSubscribedListings();

    // this.getlistings();
    // this.viewListingDetails();
    }
  }
  
  // ngOnDestroy() {
  //   this.negotiationSub?.();
  // }

  loadUserProfile() {
    this.userProfileService.getUserProfile().subscribe({
      next: (data) => {
        this.user = data;
      },
      error: (error) => {
        console.error('Error fetching user profile', error);
      },
    });
  }
  loadCropsListingsAndSubscriptions() {
    // Step 1: load crops
    this.cropService.getAllCrops().subscribe({
      next: (crops) => {
        this.crops = crops;
        // Step 2: load listings after crops are ready
        this.loadListings().then(() => {
          // Step 3: load subscriptions after listings are ready
          
          this.getMySubscriptions();
          this.loadPickupSchedules();
        });
        
      },
      error: (error) => {
        console.error('Error fetching crops', error);
      },
    });
  }

  loadListings(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.addressService.getAddress().subscribe({
        next: (address: any) => {
          const dealerCity = address.city;
          const dealerState = address.state;

          this.cropListingService.getAll().subscribe({
            next: (data: any[]) => {
              const sorted = data.sort((a, b) => {
                const aScore =
                  a.location === dealerCity
                    ? 2
                    : a.location === dealerState
                    ? 1
                    : 0;
                const bScore =
                  b.location === dealerCity
                    ? 2
                    : b.location === dealerState
                    ? 1
                    : 0;
                return bScore - aScore;
              });

              this.listings = sorted.map((listing) => {
                const crop = this.crops.find((c) => c.id === listing.cropId);
                return {
                  ...listing,
                  crop: crop ? { name: crop.name } : { name: 'Unknown' },
                };
              });
              resolve();
            },
            error: (error) => {
              console.error('Error fetching crop listings', error);
              reject(error);
            },
          });
        },
        error: (error) => {
          console.error('Error fetching dealer address', error);
          reject(error);
        },
      });
    });
  }

  subscribeCrop(listingId: string): void {
    const subscriptionData = {
      cropListingId: listingId,
      userId: this.user.id,
    };

    console.log('Subscription request:', subscriptionData);
    if (this.isSubscribed(listingId)) {
      alert('You are already subscribed to this crop.');
      return;
    }

    this.subscriptionService.postSubscription(subscriptionData).subscribe({
      next: (res) => {
        console.log('Subscribed successfully:', res);
        alert('Crop subscribed successfully');
        this.subscribedListing.add(listingId);
        this.subscriptions.push(res);
        this.getMySubscriptions(); 
        // this.cdRef.detectChanges();
      },
      error: (err) => {
        console.error('Subscription failed:', err);
        alert(err.error?.error || 'Subscription failed');
      },
    });
  }
isCropNameAlreadySubscribedForListing(listing: any): boolean {
  return this.subscribedCropNames.includes(listing.crop?.name);
}

  getMySubscriptions() {
  this.subscriptionService.getMySubscriptions().subscribe({
    next: (res) => {
      console.log('Fetched Subscriptions:', res);
      
    
      this.subscriptions = res
        .map((subscription) => {
          const cropListing = this.listings.find(
            (listing: any) => listing.id === subscription.cropListingId
          );

          if (!cropListing) return null; 

          const crop = this.crops.find((c: any) => c.id === cropListing.cropId);
          const cropName = crop ? crop.name : 'Unknown';

          return {
            ...subscription,
            cropName: cropName,
          };
        })
        .filter((sub) => sub !== null);

     
      this.subscribedListing.clear();
      this.subscriptions.forEach((sub) => {
        this.subscribedListing.add(sub.cropListingId);
      });

      this.subscriptionService.getCropNameSubscriptions().subscribe({
        next: (names) => {
           this.cropNameSubscriptions = names.map(n => n.trim().toLowerCase());
      this.subscribedCropNames = [...this.cropNameSubscriptions];
console.log('Normalized Subscribed Names:', this.subscribedCropNames);
        },
        error: (err) => {
          console.error('Error fetching crop name subscriptions:', err);
        }
      });
    this.subscriptions.forEach(subscription => {
  this.pickupService.getPickupByListingId(subscription.cropListingId).subscribe(schedules => {
    const latest = schedules[schedules.length - 1];
    if (latest) {
      this.pickupStatuses[subscription.cropListingId] = latest.status as PickUpStatus;
    }
  });
});

    },
    error: (err) => {
      console.error('Error fetching subscriptions:', err);
      alert('Error fetching subscriptions. Please try again.');
    },
  });
}


  unsubscribeCrop(cropListingId: string) {
    this.subscriptionService.deleteSubscription(cropListingId).subscribe({
      next: (res: any) => {
        alert(res.message);
        this.getMySubscriptions();
      },
      error: (err) => {
        console.error(err);
        alert(err?.error?.message || 'Unsubscribe failed');
      },
    });
  }

  isSubscribed(listingId: string): boolean {
  const listing = this.listings.find((l : any) => l.id === listingId);
  if (!listing) return false;

  // Check if either listing ID is in the list OR crop name is in crop subscriptions
  return this.subscribedListing.has(listingId) || this.subscribedCropNames.includes(listing.crop.name);
}

  getSubscriptionId(listingId: string): string | null {
    const subscription = this.subscriptions.find(
      (s) => s.cropListingId === listingId
    );
    return subscription ? subscription.id : null;
  }
 goBackToSubscriptions() {
  this.isFormVisible = false;
}

 getSubscribedListings(): void {
    this.loading = true;  
    this.subscriptionService.getSubscribedListings().subscribe({
      next: (data: any) => {
        console.log('Subscribed Listings:', data); 
        if (data && data.subscribedListings) {
         this.subscribedListings = data.subscribedListings.map((sub: any) => ({
  ...sub,
  dealerName: sub.dealer?.name || 'Unknown Dealer' 
}));
        } else {
          this.subscribedListings = []; 
        }
  
        this.loading = false;  
      },
      error: (error) => {
        console.error('Failed to fetch subscribed listings:', error);
        this.loading = false;  
      }
    });
  }
  isSubscribedToCrop(cropName: string): boolean {
  if (!cropName) return false;

  const normalizedCropName = cropName.trim().toLowerCase();

  const nameSubscribed = this.subscribedCropNames.some(
    (name) => name.trim().toLowerCase() === normalizedCropName
  );

  const listingSubscribed = this.subscriptions.some(
    (sub: any) => sub.cropName?.trim().toLowerCase() === normalizedCropName
  );

  console.log("Checking subscription for:", cropName);
  console.log("From name list?", nameSubscribed);
  console.log("From listing crop match?", listingSubscribed);

  return nameSubscribed || listingSubscribed;
}


  subscribeToCropName(cropName: string | undefined) {
  if (!cropName?.trim()) return;

  cropName = cropName.trim();

  if (this.isSubscribedToCrop(cropName)) {
    this.snackbar.info("Already subscribed to this crop name or a related listing.");
    return;
  }

  this.subscriptionService.subscribeToCropName(cropName).subscribe({
    next: () => {
      this.snackbar.success("Subscribed successfully to crop name");
      this.getMySubscriptions(); // Refresh
    },
    error: (err) => {
      console.error("Crop name subscription failed", err);
      this.snackbar.error(err?.error?.message || "Subscription failed");
    }
  });
}


unsubscribeFromCropName(cropName: string) {
  if (!cropName?.trim()) return;
  if (!confirm(`Unsubscribe from "${cropName}"?`)) return;

  this.subscriptionService.unsubscribeFromCropName(cropName).subscribe({
    next: () => {
      this.snackbar.success("Unsubscribed successfully from crop name");
      this.getMySubscriptions(); // ✅ refresh after unsubscribing
    },
    error: (err) => {
      console.error("Crop name unsubscription failed", err);
      this.snackbar.error(err?.error?.message || "Unsubscription failed");
    }
  });
}


  viewListingDetails(listing: any): void {
    alert(`
  Crop: ${listing.crop?.name ?? 'N/A'}
  Type: ${listing.crop.type}
  Listing ID: ${listing.cropListingId}
  Dealer: ${listing.dealerName}
    `);
  }



  
   getTransactions() {
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

showTransForm(listingId: string) {
  const selectedListing = this.listings.find((l: any) => l.id === listingId);
  if (!selectedListing) return;

  // Get negotiated price if any
  const negotiatedPrice = this.getNegotiatedPrice(listingId);

  this.trans = {
    id: '',
    listingId: listingId,
    quantity: 0,
    pricePerKg: negotiatedPrice ?? selectedListing.pricePerKg,  // use negotiated if exists
    orderId: '',
    currency: 'INR',
    totalPrice: 0,
    status: TransactionStatus.Pending,
  };

  this.showTransactions = false;
  this.isFormVisible = true;
}

get selectedListing() {
    return this.listings.find((l: any) => l.id === this.trans.listingId);
  }
  getSelectedListingPrice(): number {
    const selected = this.listings.find((l: any) => l.id === this.trans.listingId);
    return selected ? Number(selected.pricePerKg) : 0;
  }

updateTotalPrice() {
  if (this.trans.quantity && this.trans.pricePerKg) {
    this.trans.totalPrice = Number(this.trans.quantity) * Number(this.trans.pricePerKg);
  } else {
    this.trans.totalPrice = 0;
  }
}





 makeTransaction() {
    // Validate form inputs
    this.showTransactions=false;
    if (
      !this.trans.listingId ||
      !this.trans.quantity ||
      this.trans.quantity <= 0
    ) {
      this.snackbar.warning('Please fill out the transaction form correctly.');
      return;
    }

    // Ensure listingId is always synced with selectedListing.id
    // this.trans.listingId = this.selectedListing.id;
     if (this.selectedListing && this.selectedListing.id === this.trans.listingId) {
    this.trans.listingId = this.selectedListing.id;
  }
    console.log('Selected Listing Object:', this.selectedListing);
    console.log('Listing ID from Object:', this.selectedListing?.id);
    console.log('Updated ListingId:', this.trans.listingId);

    // Calculate total price before sending
    this.updateTotalPrice();

    if (this.trans.totalPrice <= 0) {
      this.snackbar.warning('Total price must be greater than zero.');
      return;
    }

    console.log('Transaction Data Before API Call:', this.trans);

    // IMPORTANT: Do NOT create transaction before Razorpay order is created
    // First create Razorpay order, then create transaction with orderId

    this.initiatePayment(); // Kick off payment flow
  }

  initiatePayment() {
    if (
      !this.trans.listingId ||
      !this.trans.quantity ||
      this.trans.quantity <= 0
    ) {
      this.snackbar.warning('Invalid transaction details for payment.');
      return;
    }

    // Prepare payload for Razorpay order creation
    const payload = {
      id: uuidv4(),
      transactionId: this.trans.id || uuidv4(),
      listingId: this.trans.listingId,
      
      quantity: this.trans.quantity,
      totalPrice: this.trans.totalPrice,
      currency: this.trans.currency || 'INR',
    };

    console.log('Sending Payment Request to create Razorpay Order:', payload);

    this.paymentService.createOrder(payload).subscribe({
      next: (order: any) => {
        console.log('Order Created:', order);

        if (!order.orderId) {
          console.error('OrderId is missing from backend response!');
          this.snackbar.error('Payment order creation failed.');
          return;
        }

        const razorpayOrderId = order.orderId;
        console.log('Razorpay Order ID:', razorpayOrderId);

        // Now create transaction with this orderId
        const transactionPayload = {
          listingId: this.trans.listingId,
          quantity: this.trans.quantity,
          totalPrice: this.trans.totalPrice,
          currency: this.trans.currency || 'INR',
          orderId: razorpayOrderId,
          paymentGateway: 'Razorpay',
        };

        console.log('Sending Transaction Request:', transactionPayload);

        this.transactionService.postTransaction(transactionPayload).subscribe({
          next: (backendResponse) => {
            console.log('Transaction Created:', backendResponse);

            if (!backendResponse.transaction) {
              console.error('Backend response missing "transaction" data');
              return;
            }
            const transaction = backendResponse.transaction;

            // Merge backend created transaction data with razorpayOrderId to keep orderId intact
            this.trans = {
              ...backendResponse.transaction,
              orderId: razorpayOrderId,
            };

            console.log('Transaction after Assignment:', this.trans);

            // Open Razorpay checkout modal
            this.openRazorpayCheckout();
          },
          error: (err) => {
            console.error('Failed to create transaction:', err);
            this.snackbar.error('Error creating transaction.');
          },
        });
      },
      error: (err) => {
        console.error('Failed to create order:', err);
        this.snackbar.error('Error creating order.');
      },
    });
  }

  openRazorpayCheckout() {
    if (!this.trans.id || !this.trans.orderId) {
      console.error('Transaction ID or Order ID missing!');
      this.snackbar.warning('Cannot proceed without valid transaction/order ID.');
      return;
    }

    const options = {
      key: 'rzp_test_flDOA2YudIMyRX',
      amount: this.trans.totalPrice * 100,
      currency: this.trans.currency || 'INR',
      name: 'CropDeal Payment',
      description: 'Purchase Crop',
      order_id: this.trans.orderId,
      handler: (response: any) => {
        console.log('Razorpay Payment Response:', response);

        const paymentResponse = {
          razorpay_payment_id: response.razorpay_payment_id,
          razorpay_order_id: response.razorpay_order_id,
          razorpay_signature: response.razorpay_signature,
        };

        this.completeTransaction(paymentResponse);
      },
      prefill: {
        name: this.user.name,
        email: this.user.email,
      },
    };

    const rzp = new Razorpay(options);
    console.log('Opening Razorpay modal with Order ID:', this.trans.orderId);
    rzp.open();
  }

  completeTransaction(paymentResponse: any) {
    console.log('Payment Response:', paymentResponse);
    const verificationPayload = {
      id: this.trans.id,
      paymentId: paymentResponse.razorpay_payment_id,
      orderId: paymentResponse.razorpay_order_id,
      signature: paymentResponse.razorpay_signature,
    };

    console.log('Verifying Payment:', verificationPayload);

    // Update orderId for transaction on backend (optional but good to keep synced)
    this.transactionService
      .setOrderId(this.trans.id, paymentResponse.razorpay_order_id)
      .subscribe({
        next: () => {
          console.log('Order ID updated on backend');
          this.trans.orderId = paymentResponse.razorpay_order_id; // update local copy
        },
        error: (err) => console.error('Failed to update order ID:', err),
      });

    // Verify payment signature with backend
    this.paymentService.verifyPayment(verificationPayload).subscribe({
      next: (verificationResult: any) => {
        if (verificationResult.success) {
          console.log('Payment Verified:', verificationResult);
          this.snackbar.success('Transaction completed successfully.');
          this.router.navigate(['/transaction']);
 // Refresh transactions list
        } else {
          this.router.navigate(['/transaction']);
          this.snackbar.error('Payment verification failed.');
        }
      },
      error: (err) => {
         this.router.navigate(['/transaction']);
        console.error('Error verifying payment:', err);
        this.snackbar.warning('Wait for payment verification.');
      },
    });
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

negotiateFromListing(listing: any): void {
  this.isListingVisible = false;
  this.isNegoForm = true;

  if (!listing) {
    this.snackbar.warning('Listing details not found!');
    return;
  }

  this.negotiation = {
    farmerId: listing.farmerId,
    dealerId: this.currentUserId,
    cropListingId: listing.id,
    listingId: listing.id,
    proposedPrice: 0,
    quantity: 0,
    negotiatedPricePerKg: 0,
    totalPrice: 0
  };

  this.cropName = listing.crop?.name || 'Unknown Crop';
  this.listingPricePerKg = listing.pricePerKg;
  this.availableQuantity = listing.quantity;
  this.quantityError = false;
}


 negotiate(subscription: any) {
  this.isNegoForm= true;
  const listing = this.listings.find((l: any) => l.id === subscription.cropListingId);
  if (!listing) {
    this.snackbar.warning('Listing details not found!');
    return;
  }

  this.negotiation = {
    farmerId: listing.farmerId,
    dealerId: this.currentUserId,
    cropListingId: listing.id,
    listingId: listing.id,  // for backend property mapping
    proposedPrice: 0,
    quantity: 0,  // initial quantity, user inputs later
    negotiatedPricePerKg: 0,
    totalPrice: 0,
  };

  this.cropName = subscription.cropName;
  this.listingPricePerKg = listing.pricePerKg;

  // Set available quantity so that checkQuantity can use it
  this.availableQuantity = listing.quantity;
  this.quantityError = false;
}

checkQuantity() {
  if (this.negotiation.quantity > this.availableQuantity) {
    this.quantityError = true;
  } else {
    this.quantityError = false;
  }
}

submitNegotiation() {
  if (this.quantityError) {
    this.snackbar.warning(`Quantity cannot exceed available quantity (${this.availableQuantity}).`);
    return;
  }

  // Validate required fields
  if (!this.negotiation.farmerId || !this.negotiation.dealerId || !this.negotiation.listingId ||
      this.negotiation.proposedPrice <= 0 || this.negotiation.quantity <= 0) {
   this.snackbar.warning("Please fill all required fields correctly.");
    return;
  }

  // Build payload matching backend model (no navigation properties)
  const payload = {
    listingId: this.negotiation.listingId,
    dealerId: this.negotiation.dealerId,
    farmerId: this.negotiation.farmerId,
    quantity: this.negotiation.quantity,
    negotiatedPricePerKg: this.negotiation.proposedPrice,
    totalPrice: this.negotiation.quantity * this.negotiation.proposedPrice,
    status: "Pending" // or whatever default status you want to send
  };

  this.negotiationService.createNegotiation(payload).subscribe({
    next: (res) => {
      this.snackbar.success("Negotiation submitted successfully!");
      // reset form if needed
      const sub = this.subscriptions.find(s => s.cropListingId === this.negotiation.listingId);
  if (sub) {
    sub.hasNegotiated = true;
    sub.negotiationStatus = "Pending";
  }
      this.negotiation.quantity = 0;
      this.negotiation.proposedPrice = 0;
       this.loadNegotiations();
      this.isNegoForm = false;
    },
    error: (err) => {
      console.error('Negotiation failed', err);
      this.snackbar.error("Failed to submit negotiation");
    }
  });
}

viewNegotiationStatus(subscription: any): void {
 this.router.navigate(['/negotiation']);
}
navigateToNegotiations(): void {
  this.router.navigate(['/negotiation']);
}


loadNegotiations() {
  if (!this.currentUserId) {
    console.error('Dealer ID is missing');
    return;
  }

  this.negotiationService.getNegotiationsForDealer(this.currentUserId).subscribe({
    next: (negos) => {
      this.negotiatedListings = new Set(negos.map(n => n.listingId));
    },
    error: (err) => {
      console.error('Error fetching negotiations:', err);
    }
  });
}

loadFullNegotiations() {
  if (!this.currentUserId) {
    console.error('Dealer ID is missing');
    return;
  }

  this.negotiationService.getNegotiationsForDealer(this.currentUserId).subscribe({
    next: (negos) => {
      this.isNegoForm =false;
      this.fullNegotiations = negos; // store full negotiations for reference
      console.log('Full Negotiations Loaded:', this.fullNegotiations);
    },
    error: (err) => {
      console.error('Error fetching full negotiations:', err);
    }
  });
}
getNegotiatedPrice(listingId: string): number | null {
  const negotiation = this.fullNegotiations.find((n : any) => n.listingId === listingId);
  return negotiation ? negotiation.negotiatedPricePerKg : null;  // Adjust property name if needed
}


navigateToPickup(listingId: string): void {
  this.router.navigate(['/pickup-schedule', listingId]);
}

loadPickupSchedules(): void {
  this.cropListings.forEach(listing => {
    this.pickupService.getPickupByListingId(listing.id).subscribe({
      next: (schedules: PickupSchedule[]) => {
        this.pickupSchedules[listing.id] = schedules;
        console.log('✅ Loaded schedules for listing:', listing.id, schedules);
      },
      error: err => {
        console.error('❌ Failed to load pickup schedules for', listing.id, err);
      }
    });
  });
}



handlePaymentClick(cropListingId: string): void {
  this.pickupService.getPickupByListingId(cropListingId).subscribe({
    next: (schedules: PickupSchedule[]) => {
      const dealerSchedule = schedules.find(s =>
        s.dealerId?.toString().trim() === this.currentUserId?.toString().trim() &&
        s.status?.toLowerCase() !== 'cancelled'
      );

      if (!dealerSchedule) {
        this.snackbar.info("⚠️ Please create a pickup schedule before proceeding to payment.");
        return;
      }

      if (dealerSchedule.status !== PickUpStatus.Delivered){
        this.snackbar.info("📦 You can proceed to payment only after the crop is delivered.");
        return;
      }

      // ✅ Now check if already paid
      this.transactionService.getTransactionByDealerAndListing(this.currentUserId, cropListingId).subscribe({
    next: (txn) => {
      if (txn && txn.status?.toLowerCase() === 'completed') {
        this.snackbar.success("✅ Payment already completed for this crop listing.");
      } else {
        // ❗Not paid, show form
        this.showTransForm(cropListingId);
      }
    },
    error: (err) => {
      // ❗No previous transaction (404), so allow form
      console.warn("No previous transaction found, proceeding to form.");
      this.showTransForm(cropListingId);
    }
  });
    },
    error: (error) => {
      console.error("❌ Failed to load pickup schedules:", error);
      this.snackbar.error("❌ Could not check pickup schedule. Please try again later.");
    }
  });
}
toggleListings(cropName: string): void {
  this.isListingVisible= true;
  console.log('Crop name subscriptions:', this.cropNameSubscriptions);

  if (!cropName || cropName.trim() === '') {
    console.warn("Invalid crop name:", cropName);
    return;
  }

  // Toggle if same crop is clicked again
  if (this.selectedCropWithListings === cropName) {
    this.selectedCropWithListings = null;
    this.filteredListings = [];
    return;
  }



  // Switch crop name and filter listings based on crop.name
  this.selectedCropWithListings = cropName;

  this.filteredListings = this.listings.filter(
    (listing : any) => listing.crop?.name?.toLowerCase().trim() === cropName.toLowerCase().trim()
  );

  console.log("Filtered Listings for crop:", cropName, this.filteredListings);
}





}
