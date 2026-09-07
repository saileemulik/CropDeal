import { Component, OnInit } from '@angular/core';
import { PickupSchedule, PickUpStatus } from '../../app/models/pickupschedule';
import { PickupscheduleService } from '../../app/services/pickupschedule.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { CropListingsService } from '../../app/services/croplistings.service';
import { CropListings } from '../../app/models/cropListings';
import { ActivatedRoute, Router } from '@angular/router';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-pickup',
  imports: [FormsModule, CommonModule],
  templateUrl: './pickup.component.html',
  styleUrl: './pickup.component.css'
})
export class PickupComponent implements OnInit{
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
 listingId: string = ''; 
editingScheduleId: string | null = null;
updatedScheduleDate: { [id: string]: string } = {};
  newSchedule: PickupSchedule = {
    listingId: '',
    farmerId:'',
    dealerId:'',
    proposedPickupSlot: '',
    status: PickUpStatus.Proposed
  };
  activeSection: string = 'list';
  schedules: PickupSchedule[] = [];
  selectedScheduleId: string = '';
  updatedProposedSlot: any;
  confirmedSlot: any;
  PickupStatus = PickUpStatus; // make accessible in HTML
  pickupStatuses: string[] = Object.values(PickUpStatus);
  newStatus: string = PickUpStatus.Proposed;
  currentUserId : string ='';
  isProposalCreated : boolean = false;

  constructor(private pickupService: PickupscheduleService, private loginService : LoginService,  public router : Router,
     private route: ActivatedRoute,private cropListingService : CropListingsService, private snackbar: SnackbarService) {}

  ngOnInit(): void {
    this.currentUserRole = this.loginService.getUserRole() as UserRole;
    this.currentUserId = localStorage.getItem('userId') || '';
      this.route.paramMap.subscribe(params => {
    const id = params.get('listingId');
    if (id) {
      this.listingId = id ?? '';
      console.log(this.listingId)
      this.loadSchedules();
    } else {
       this.snackbar.error('Missing listing ID in route.');
    }
  });
    this.loadSchedules();
  }

 loadSchedules(): void {
  this.pickupService.getPickupByListingId(this.listingId).subscribe(res => {
    this.schedules = res;

    // ✅ Check if the dealer has already created a non-cancelled proposal
    this.isProposalCreated = this.schedules.some(schedule =>
      schedule.dealerId === this.currentUserId &&
      schedule.status !== PickUpStatus.Cancelled
    );
  });
}


 createProposal(): void {
  this.newSchedule.listingId = this.listingId;
  this.newSchedule.dealerId = this.currentUserId;

  // Fetch the CropListing to get the farmerId
  this.cropListingService.getCropById(this.listingId).subscribe({
    next: (listing: CropListings) => {
      console.log('Listing fetched from backend:', listing);

      // Assign the farmerId directly since it's always present
      this.newSchedule.farmerId = listing.farmerId;
      // const date = new Date();
      // this.newSchedule.proposedPickupSlot = date.toISOString().slice(0, 16); 
      this.newSchedule.status = PickUpStatus.Proposed;

      // Call the createProposal API
      this.pickupService.createProposal(this.newSchedule).subscribe({
        next: () => {
           this.snackbar.success('Schedule Created');
           this.isProposalCreated = true; 

          // Reset form after successful creation
          this.newSchedule = {
            listingId: this.listingId,
            dealerId: this.currentUserId,
            farmerId: '',
            proposedPickupSlot: '',
            status: PickUpStatus.Proposed
          };
          this.loadSchedules();
        },
        error: (error) => {
          console.error('Error creating schedule:', error);
           this.snackbar.error('Failed to create schedule.');
        }
      });
    },
    error: (err) => {
      console.error('Failed to fetch listing:', err);
       this.snackbar.error('Could not retrieve farmer ID for the listing.');
    }
  });
}




  confirmSlot(): void {
    this.pickupService.confirmPickupSlot(this.selectedScheduleId, this.confirmedSlot).subscribe(() => {
      this.confirmedSlot = '';
       this.snackbar.success('Slot Confirmed');
      this.loadSchedules();
    });
  }

  updateStatus(): void {
    this.pickupService.updateSingleScheduleStatus(this.selectedScheduleId, this.newStatus).subscribe(() => {
     
      this.newStatus = '';
       this.snackbar.success('Status Updated');
      this.loadSchedules();
    });
  }
UpdateSlot(newSlotString: string): void {
  const newSlot = new Date(newSlotString);

  // Check if the schedule is confirmed
  const selectedSchedule = this.schedules.find(s => s.id === this.selectedScheduleId);
  if (!selectedSchedule) {
     this.snackbar.info('Schedule not found.');
    return;
  }

  if (selectedSchedule.status === 'Confirmed') {
     this.snackbar.warning('⚠️ Cannot update a confirmed schedule.');
    return;
  }

  this.pickupService.updateProposedSlot(this.selectedScheduleId, newSlot).subscribe({
    next: () => {
      console.log('Updating schedule with ID:', this.selectedScheduleId);
       this.snackbar.success('Slot Updated');
      this.loadSchedules();
    },
    error: (err) => {
      console.error('Failed to update slot:', err);
       this.snackbar.error('Failed to update slot');
    }
  });
}



  cancelProposal(): void {
  this.pickupService.getById(this.selectedScheduleId).subscribe({
    next: (schedule) => {
      if (schedule.status?.toLowerCase() === 'confirmed') {
         this.snackbar.warning('❌ Cannot cancel the proposal after it is confirmed.');
        return;
      }

      // Proceed to cancel
      this.pickupService.cancelProposal(this.selectedScheduleId).subscribe({
        next: () => {
           this.snackbar.success('✅ Schedule cancelled successfully.');
          this.isProposalCreated = false;
          this.loadSchedules();
        },
        error: (err) => {
          console.error('❌ Failed to cancel proposal:', err);
           this.snackbar.error('Failed to cancel the proposal. Please try again.');
        }
      });
    },
    error: (err) => {
      console.error('❌ Failed to fetch schedule:', err);
       this.snackbar.error('Could not check schedule status. Try again.');
    }
  });
}


  updateSingleScheduleStatus(scheduleId: string, newStatus: string): void {
  this.pickupService.updateSingleScheduleStatus(scheduleId, newStatus).subscribe({
    next: () => {
       this.snackbar.success('Status updated');
      this.loadSchedules(); // reload pickup list
    },
    error: (err) => {
      console.error('Error updating status:', err);
       this.snackbar.error('Failed to update status');
    }
  });
}

navigateToSubscription() {
  this.router.navigate(['/subscription']);
}

startEditing(id: string) {
  this.editingScheduleId = id;
  const schedule = this.schedules.find(s => s.id === id);
  if (schedule) {
    this.updatedScheduleDate[id] = schedule.proposedPickupSlot; // Pre-fill existing slot
  }
}

confirmUpdate(id: string): void {
  const newSlotStr = this.updatedScheduleDate[id]; 
  
  if (newSlotStr) {
    const newSlot = new Date(newSlotStr); 

    this.pickupService.updateProposedSlot(id, newSlot).subscribe({
      next: () => {
         this.snackbar.success('Slot Updated');
        this.editingScheduleId = null;
        this.loadSchedules();
      },
      error: err => {
        console.error('Error updating slot:', err);
         this.snackbar.error('Failed to update slot');
      }
    });
  } else {
    this.snackbar.info('Please select a new date and time.');
  }
}


cancelSchedule(id: string) {
  this.selectedScheduleId = id;
  this.cancelProposal(); // Your existing method
}
}
