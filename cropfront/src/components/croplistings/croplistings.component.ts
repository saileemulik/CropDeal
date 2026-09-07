import { ChangeDetectorRef, Component } from '@angular/core';
import { CropListingsService } from '../../app/services/croplistings.service';
import { CommonModule } from '@angular/common';
import { CropService } from '../../app/services/crop.service';
import { AddressService } from '../../app/services/address.service';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { SubscriptionService } from '../../app/services/subscription.service';
import { AuthService } from '../../app/services/auth.service';
import { UserService } from '../../app/services/user.service';
import { CropAvailability } from '../../app/models/crop';
import { FormsModule } from '@angular/forms';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-croplistings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './croplistings.component.html',
  styleUrl: './croplistings.component.css',
})
export class CroplistingsComponent {
  user: any = {};
  crops: any[] = [];
  cropListing = {
    cropId: '',
    pricePerKg: 0,
    quantity: 0,
    location: '',
    description: '',
    imageUrl: '',
    unit: '',
    status: '',
  };

  cropListings: any[] = [];
  subscribedListings: Set<string> = new Set();
  subscribedCropNames : string[] = [];
  subscriptions: any[] = [];
  listings: any = [];
  image: File | null = null;
  imagePreview: string | null = null;
  activeSection: string = '';
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  editableListing: any = null;
  selectedCropId: string = '';
  quantity: number = 0;
  price: number = 0;
  unit: string = '';
  location: string = '';
  description: string = '';
  status: CropAvailability = CropAvailability.Available;
  cropStatusList: string[] = Object.values(CropAvailability);
  isEdit: boolean = false;
  searchName: string = '';
  searchType: string = '';
  minPrice: number | null = null;
  maxPrice: number | null = null;
  filteredListings: any[] = [];
  cropNameSubscriptions: string[] = [];
  newCropName: string = '';
  listingOrCropView: 'listings' | 'crops' = 'listings';
  activeListingView: 'add' | 'view' | 'edit' = 'view';

  constructor(
    private cropListingService: CropListingsService,
    private cropService: CropService,
    private subscriptionService: SubscriptionService,
    private loginService: LoginService,
    private authService: AuthService,
    private userProfileService: UserService,
    private addressService: AddressService,
    private snackbar : SnackbarService,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadUserProfile();
    this.authService.user$.subscribe((user) => {
      this.user = user;
      console.log('Loaded user:', this.user);
    });

    this.currentUserRole = this.loginService.getUserRole() as UserRole;

    this.activeSection = 'listing';
    const role = this.loginService.getUserRole() as UserRole;
    this.getCrops();

    if (role == 'Admin') {
      this.getCropListings();
    }
    if (role == 'Dealer') {
      this.getMySubscriptions();
      this.getlistings();
      this.filterListings();
    }
    if (role == 'Farmer') {
      this.loadCrops();
      this.loadCropListings();
    }
  }

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
  getCrops() {
    this.cropService.getAllCrops().subscribe(
      (crops: any[]) => {
        this.crops = crops;
        this.getCropListings();
      },
      (error) => {
        console.error('Error fetching crops', error);
      }
    );
  }
  filterListings(): void {
  const name = this.searchName.toLowerCase().trim();
  const type = this.searchType.toLowerCase().trim();

  // Check if any filter is applied
  const filtersApplied = !!(name || type || this.minPrice != null || this.maxPrice != null);

  if (!filtersApplied) {
    // No filters applied: clear filteredListings to show original listings only
    this.filteredListings = [];
    return;
  }

  this.filteredListings = this.listings.filter((l: any) => {
    const price = l.pricePerKg;

    return (
      (!name || l.crop.name.toLowerCase().includes(name)) &&
      (!type ||
        l.crop.customType?.toLowerCase().includes(type) ||
        l.crop.type?.toLowerCase().includes(type)) &&
      (this.minPrice == null || price >= this.minPrice) &&
      (this.maxPrice == null || price <= this.maxPrice)
    );
  });
}

  getCropListings() {
    this.cropListingService.getAllCropListings().subscribe(
      (data: any[]) => {
        this.cropListings = data.map((listing) => {
          const crop = this.crops.find((c) => c.id === listing.cropId);
          return {
            ...listing,
            crop: crop ? { name: crop.name } : { name: 'Unknown' },
          };
        });
      },
      (error) => {
        console.error('Error fetching crop listings', error);
      }
    );
  }
  loadCrops(): void {
    this.cropService.getAllCrops().subscribe({
      next: (data) => (this.crops = data),
      error: (error) => console.error('Error fetching crops', error),
    });
  }
  addListing(): void {
    if (
      !this.selectedCropId ||
      this.quantity <= 0 ||
      this.price <= 0 ||
      !this.imagePreview
    ) {
      this.snackbar.warning('Please fill all the fields including the image');
      return;
    }

    const base64 = this.imagePreview.split(',')[1];

    const listing = {
      cropId: this.selectedCropId,
      quantity: this.quantity,
      pricePerKg: this.price,
      unit: this.unit,
      description: this.description,
      location: this.location,
      status: this.status,
      imageBase64: base64,
    };

    this.cropListingService.addCropListing(listing).subscribe({
      next: () => {
        this.snackbar.success('Listing added successfully');
        this.loadCropListings();
      },
      error: () => this.snackbar.error('Failed to add listing'),
    });
  }
// isLoading = true;
  loadCropListings(): void {
    this.cropListingService.getMyListings().subscribe({
      next: (data) => {
        this.cropListings = data.map((listing: any) => {
          const crop = this.crops.find((c) => c.id === listing.cropId);
          return {
            ...listing,
            crop: crop ? { name: crop.name } : { name: 'Unknown' },
          };
          // this.isLoading = false;
        });
        // this.cdRef.detectChanges();
      },
      error: (error) => console.error('Error fetching crop listings', error),
    });
  }

 editListing(listing: any): void {
  this.editableListing = { ...listing };
  this.activeListingView = 'edit';
}
cancelEdit(): void {
  this.editableListing = null;
  this.activeListingView = 'view';
}
onEditImageSelected(event: any) {
  const file = event.target.files[0];
  if (file) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      // Set preview for UI and Base64 for backend update
      this.editableListing.imageUrl = e.target.result.split(',')[1]; // remove "data:image/jpeg;base64,"
    };
    reader.readAsDataURL(file);

    // Optionally store the file if you want to send as multipart/form-data
    this.onFileSelected = file;
  }
}


  updateListings(): void {
    if (!this.editableListing?.id) {
      this.snackbar.warning('No listing selected for update.');
      return;
    }

    this.cropListingService
      .updateCropListing(this.editableListing.id, this.editableListing)
      .subscribe({
        next: () => {
          this.snackbar.success('Listing updated successfully');
          this.isEdit = false;
          this.editableListing = null;
          this.loadCropListings();
        },
        error: (err) => {
          console.error('Update error:', err);
          this.snackbar.error('Failed to update listing');
        },
      });
  }

  deleteListings(id: string) {
    if (!id) {
      this.snackbar.warning('No listings selected to delete');
      return;
    }

    this.cropListingService.deleteCropListing(id).subscribe({
      next: () => {
       this.snackbar.success('Crop Listings deleted successfully');
        this.loadCropListings();
      },
      error: (err) => {
        console.error('Failed to delete listings:', err);
        this.snackbar.error('Failed to delete listings');
      },
    });
  }
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];

    if (file && allowedTypes.includes(file.type)) {
      this.image = file;

      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = reader.result as string;
      };
      reader.readAsDataURL(file);
    } else {
      this.snackbar.warning('Please upload a valid image file (JPEG/PNG).');
    }
  }

  getlistings(): void {
    const dealerId = localStorage.getItem('userId');
    if (!dealerId) {
      console.error('Dealer ID is missing. Please log in again.');
      return;
    }

    this.addressService.getAddress().subscribe(
      (address: any) => {
        const dealerCity = address.city;
        const dealerState = address.state;

        this.cropListingService.getAll().subscribe(
          (data: any[]) => {
            console.log('All Listings:', data);
            this.filteredListings = [...this.listings];
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
             this.filteredListings = [];
          },
          (error) => {
            console.error('Error fetching crop listings', error);
          }
        );
      },
      (error) => {
        console.error('Error fetching dealer address', error);
      }
    );
  }
  isCropNameAlreadySubscribedForListing(listing: any): boolean {
  return this.subscribedCropNames.includes(listing.crop?.name);
}


  subscribeCrop(listingId: string): void {
    if (!this.user || !this.user.id) {
      // console.error('User not logged in or ID missing');
      return;
    }

    const subscriptionData = {
      cropListingId: listingId,
      userId: this.user.id,
    };

    console.log('Subscription request:', subscriptionData);
    if (this.isSubscribed(listingId)) {
      this.snackbar.warning('You are already subscribed to this crop.');
      return;
    }

    this.subscriptionService.postSubscription(subscriptionData).subscribe({
      next: (res) => {
        console.log('Subscribed successfully:', res);
        this.getMySubscriptions();
        this.snackbar.success('Crop subscribed successfully');
        this.subscribedListings.add(listingId);
        this.subscriptions.push(res);
        // this.cdRef.detectChanges();
      },
      error: (err) => {
        console.error('Subscription failed:', err);
        this.snackbar.error(err.error?.error || 'Subscription failed');
      },
    });
  }
  resetFilters(): void {
  this.searchName = '';
  this.searchType = '';
  this.minPrice = null;
  this.maxPrice = null;
  this.filteredListings = [...this.listings];
}


  getMySubscriptions() {
    this.subscriptionService.getMySubscriptions().subscribe({
      next: (res) => {
        console.log('Fetched Subscriptions:', res);

        this.subscriptions = res.map((subscription) => {
          const cropListing = this.listings.find(
            (listing: any) => listing.id === subscription.cropListingId
          );

          const crop = cropListing
            ? this.crops.find((c: any) => c.id === cropListing.cropId)
            : null;

          // const farmer = crop ? this.users.find((c: any) => c.id === crop.farmerId) : null;
          // console.log(crop.farmerId);

          const cropName = crop ? crop.name : 'Unknown Crop';
          // const farmerName = farmer ? farmer.name : 'Unknown name';

          console.log(cropName);
          return {
            ...subscription,
            cropName: cropName,
            // farmerName: farmerName
          };
        });

        this.subscribedListings.clear();
        this.subscriptions.forEach((sub) => {
          this.subscribedListings.add(sub.cropListingId);
        });
    console.log('Calling getCropNameSubscriptions...');
  this.subscriptionService.getCropNameSubscriptions().subscribe({
    next: (names) => {
      console.log('Fetched Crop Name Subscriptions:', names);
      this.cropNameSubscriptions = names.map(n => n.trim().toLowerCase());
      this.subscribedCropNames = [...this.cropNameSubscriptions];
console.log('Normalized Subscribed Names:', this.subscribedCropNames);
    },
    error: (err) => {
      console.error('Error fetching crop name subscriptions:', err);
    }
  });
        // this.cdRef.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching subscriptions:', err);
        this.snackbar.error('Error fetching subscriptions. Please try again.');
      },
    });
  }

  unsubscribeCrop(cropListingId: string) {
    this.subscriptionService.deleteSubscription(cropListingId).subscribe({
      next: (res: any) => {
        this.snackbar.success(res.message);
        this.getMySubscriptions();
      },
      error: (err) => {
        console.error(err);
        this.snackbar.error(err?.error?.message || 'Unsubscribe failed');
      },
    });
  }

  isSubscribed(listingId: string): boolean {
  const listing = this.listings.find((l : any) => l.id === listingId);
  if (!listing) return false;

  // Check if either listing ID is in the list OR crop name is in crop subscriptions
  return this.subscribedListings.has(listingId) || this.subscribedCropNames.includes(listing.crop.name);
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




  getSubscriptionId(listingId: string): string | null {
    const subscription = this.subscriptions.find(
      (s) => s.cropListingId === listingId
    );
    return subscription ? subscription.id : null;
  }

 subscribeToCropName(cropName: string | undefined) {
  if (!cropName?.trim()) return;

  cropName = cropName.trim();

  if (this.isSubscribedToCrop(cropName)) {
    this.snackbar.warning("Already subscribed to this crop name or a related listing.");
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


}
