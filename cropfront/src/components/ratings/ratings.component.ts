import { Component } from '@angular/core';
import { RatingService } from '../../app/services/rating.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-ratings',
  imports: [FormsModule, CommonModule],
  templateUrl: './ratings.component.html',
  styleUrl: './ratings.component.css'
})
export class RatingsComponent {
  activeSection: string = '';
  showRating: boolean = false;
  ratings: any[] = [];
  averageRating: number = 0;
  rating: any = {
    transactionId: '',
    farmerId: '',
    rating: null,
    comment: '',
  };
   UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  isReviewEdit: boolean = false;
  isReviewFormVisible = false;

  constructor(private ratingService: RatingService, private loginService : LoginService, private snackbar : SnackbarService) {}

  ngOnInit(): void {
     this.currentUserRole = this.loginService.getUserRole() as UserRole;
    this.activeSection= 'rating';
    if(this.currentUserRole=='Admin')
    {
      this.getRating();
    }
    if(this.currentUserRole=='Farmer')
    {
      this.getFarmerRating();
    }
    if(this.currentUserRole=='Dealer')
    {
      this.getDealerRating();
    }
  }

  getRating() {
  this.ratingService.getAllRating().subscribe({
    next: (data) => {
      console.log('Received ratings in component:', data);
      this.ratings = data;
      this.showRating = this.ratings.length > 0;
    },
    error: (err) => {
      console.error('Error fetching ratings:', err);
      this.showRating = false;
    }
  });
}

getFarmerRating() {
  this.ratingService.getRatingFarmer().subscribe({
    next: (data) => {
      console.log('Received ratings in component:', data);
      this.ratings = data.reviews;
      this.averageRating = data.averageRating;
      this.showRating = this.ratings.length > 0;
      if (!this.showRating) {
        console.warn('No ratings to display.');
      }
    },
    error: (err) => {
      console.error('Error fetching ratings:', err);
      this.showRating = false;
      alert(err?.error?.message || 'Failed to load ratings.');
    }
  });
}


//  editListing(listing: any): void {
//     this.isEdit = true;
//     // this.editableListing = { ...listing }; 
//   }
  
  // updateListings(): void {
  //   if (!this.editableListing?.id) {
  //     alert("No listing selected for update.");
  //     return;
  //   }
  
  //   this.cropListingsService.updateCropListing(this.editableListing.id, this.editableListing).subscribe({
  //     next: () => {
  //       alert('Listing updated successfully');
  //       this.isEdit = false;
  //       this.editableListing = null;
  //       this.loadCropListings();
  //     },
  //     error: (err) => {
  //       console.error('Update error:', err);
  //       alert('Failed to update listing');
  //     }
  //   });
  // }
  

  deleteRatings(id: string) {
    if (!id) {
      this.snackbar.warning('No ratings selected to delete');
      return;
    }

    this.ratingService.deleteReview(id).subscribe({
      next: () => {
        this.snackbar.success('Ratings deleted successfully');
        this.getRating();
      },
      error: (err) => {
        console.error('Failed to delete ratings:', err);
        this.snackbar.error('Failed to delete ratings');
      }
    });
  }
getDealerRating() {
  this.ratingService.getRating().subscribe({
    next: (data) => {
      console.log('Received ratings in component:', data);
      this.ratings = data;
      this.showRating = this.ratings.length > 0;
    },
    error: (err) => {
      console.error('Error fetching ratings:', err);
      this.showRating = false;
    }
  });
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

    // this.cdRef.detectChanges(); // optional - only if view is not updating properly
  }
 getStarHTML(rating: number): string {
  const fullStars = Math.floor(rating);
  const halfStar = rating % 1 >= 0.25 && rating % 1 < 0.75;
  const emptyStars = 5 - fullStars - (halfStar ? 1 : 0);

  let stars = '';

  for (let i = 0; i < fullStars; i++) {
    stars += '<i class="fas fa-star text-warning"></i>';
  }
  if (halfStar) {
    stars += '<i class="fas fa-star-half-alt text-warning"></i>';
  }
  for (let i = 0; i < emptyStars; i++) {
    stars += '<i class="far fa-star text-warning"></i>';
  }

  return stars;
}



}
