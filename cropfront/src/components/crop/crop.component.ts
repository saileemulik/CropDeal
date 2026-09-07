import { Component } from '@angular/core';
import { CropService } from '../../app/services/crop.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { CropTypeEnum, RequestStatus } from '../../app/models/crop';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-crop',
  standalone: true,
  imports: [FormsModule, CommonModule, HttpClientModule, RouterLink],
  templateUrl: './crop.component.html',
  styleUrl: './crop.component.css'
})
export class CropComponent {
  crop = {
    name: '',
  };
  cropType: CropTypeEnum | '' = ''; 
  crops: any = {};
  cropTypes: string[] = ['Grain', 'Vegetable', 'Fruit', 'Legume', 'Oilseed', 'Root', 'Tuber'];
  activeSection: string = '';
  isFormVisble : boolean = false;
  requestFormVisble : boolean = false;
  editingCropId: string | null = null;
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  cropName: string = '';
  description: string = '';
  type : string ='';
  requests: any[] = [];
  RequestStatus = RequestStatus;  
  requestStatusOptions = Object.values(RequestStatus);
  cropTypeOptions = Object.values(CropTypeEnum);
 requestDecision: Record<string, RequestStatus | ''> = {};  // decision per request
  requestCropType: Record<string,  ''> = {};
  showAll: boolean = false;
  
  activeView: 'crops' | 'requests' = 'crops';
  constructor(private cropService: CropService, private loginService : LoginService, private snackbar: SnackbarService) {}
  ngOnInit(): void {
     this.currentUserRole = this.loginService.getUserRole() as UserRole;
    this.activeSection = 'crop';
    if(this.currentUserRole=='Admin')
   {
    
    this.getCrop();
    this.loadPendingRequests();
   }
    if(this.currentUserRole=='Farmer')
   {
    
    this.getStatusOfRequestedCrops();
    this.loadMyRequests();
   
   }
  
   
  }
   toggleAddForm(): void {
    this.isFormVisble = !this.isFormVisble;
    if (!this.isFormVisble) {
      this.getCrop();
    }
  }

 addCrop() {
  if (!this.crop.name || !this.cropType) {
    this.snackbar.warning('Please fill out all fields before adding.');
    return;
  }

  this.cropService.addCrop(this.crop, this.cropType).subscribe({
    next: () => {
      this.snackbar.success('Crop added successfully');
      this.activeSection = 'crop';
      this.crop = { name: '' };
      this.cropType = '';
      this.isFormVisble = false;
      this.getCrop();
    },
    error: (err) => {
      console.error('Failed to add crop:', err);
      this.snackbar.error('Failed to add crop');
    }
  });
}
submitRequestView() {
  this.activeView = 'crops';
  this.requestFormVisble = true;
}

editCrop(crop: any) {
  this.editingCropId = crop.id;
  this.crop = { ...crop };
  this.cropType = crop.type as CropTypeEnum;  // Cast to enum type
  this.isFormVisble = true;
}

updateCrop() {
  if (!this.editingCropId || !this.crop.name || !this.cropType) {
    this.snackbar.warning('Please fill out all fields before updating.');
    return;
  }

  this.cropService.updateCrop(this.editingCropId, this.crop, this.cropType).subscribe({
    next: () => {
      this.snackbar.success('Crop updated successfully');
      this.getCrop();
      this.cancelEdit();
    },
    error: (err) => {
      console.error('Failed to update crop:', err);
      this.snackbar.error('Failed to update crop');
    }
  });
}
  deleteCrop(id: string) {
    this.isFormVisble=false;
    if (!id) {
      this.snackbar.warning('No crop selected to delete');
      return;
    }

    this.cropService.deleteCrop(id).subscribe({
      next: () => {
        this.snackbar.success('Crop deleted successfully');
        this.getCrop();
      },
      error: (err) => {
        console.error('Failed to delete crop:', err);
        this.snackbar.error('Failed to delete crop');
      }
    });
  }

  getCrop() {
    this.cropService.getAllCrops().subscribe({
      next: (data) => {
        this.isFormVisble=false;
        this.crops = data;
      },
      error: (error) => {
        console.error('Error fetching crops:', error);
      }
    });
  }
  
cancelEdit() {
  this.crop = { name: '' };
  this.type = '';
  this.editingCropId = null;
  this.isFormVisble = false;
}


submitRequest() {
  this.activeView='crops';
  this.requestFormVisble = true;
    if (!this.cropName || !this.description || !this.type) {
      this.snackbar.warning('Please fill in all fields before submitting.');
      return;
    }

    const data = {
      cropName: this.cropName,
      description: this.description,
      type: this.type
    };

    this.cropService.requestCrop(data).subscribe({
      next: res => {
        this.snackbar.success(res.message || 'Crop request submitted!');
        this.resetForm();
      },
      error: err => {
        console.error(err);
       this.snackbar.error('Error submitting crop request');
      }
    });
  }

  resetForm() {
    this.cropName = '';
    this.description = '';
    this.type = '';
  }

   showCrops() {
    this.activeView = 'crops';
    this.isFormVisble = false;
    this.getCrop();
  }


   showRequests() {
    this.activeView = 'requests';
    this.loadPendingRequests();
  }

  loadPendingRequests() {
    this.cropService.getPendingCropRequests().subscribe({
      next: (data) => {
        console.log(data);
        this.requests = data;
        this.requestDecision = {};
        this.requestCropType = {};
        data.forEach((r : any) => {
          this.requestDecision[r.id] = '';
          this.requestCropType[r.id] = '';
        });
        // Removed loadMyRequests() call - only show pending requests for admin
      },
      error: (err) => console.error(err),
    });
  }
loadMyRequests() {
  this.cropService.getMyCropRequests().subscribe({
    next: (data) => {
      console.log("All Requests:", data);
      this.requests = data;
      this.requestDecision = {};
      this.requestCropType = {};
      data.forEach((r: any) => {
        this.requestDecision[r.id] = '';
        this.requestCropType[r.id] = '';
      });
    },
    error: (err) => console.error(err),
  });
}

 approveOrRejectRequest(requestId: string, status: RequestStatus, type: string) {
    const cropDto = {
      CropName: this.requests.find(r => r.id === requestId)?.cropName || '',
      Type: type
      // add Description if needed, or any other property your backend expects
    }; // wrap type in an object to match DTO on backend
  console.log('Sending cropDto:', cropDto);


  this.cropService.handleCropRequest(requestId, status, cropDto).subscribe({
    
    next: () => {

      this.snackbar.info(`Request ${status.toLowerCase()}!`);
      // Only reload pending requests for admin view
      this.loadPendingRequests();
    },
    error: () => alert(`Failed to ${status.toLowerCase()} request`),
  });
}

isValidRequest(requestId: string): boolean {
  const decision = this.requestDecision[requestId];
  const type = this.requestCropType[requestId];
  return decision != null && decision !== '' && typeof type === 'string' && type.trim() !== '';
}

handleRequestSubmit(requestId: string): void {
  if (this.isValidRequest(requestId)) {
    const decision = this.requestDecision[requestId] as RequestStatus;
    const type = this.requestCropType[requestId].trim();
    this.approveOrRejectRequest(requestId, decision, type);
  } else {
    this.snackbar.warning('Please select both a decision and a crop type.');
  }
}

getStatusOfRequestedCrops(){
  this.activeView = 'requests';
  this.cropService.getCropRequestStatus().subscribe(
    {
      next: (data) => {
        this.requestFormVisble=false;
        this.requests = data;
        console.log(this.requests);
      },
      error: (error) => {
        console.error('Error fetching requested crops:', error);
      }
    });
  
}
toggleRequestView(showAll: boolean): void {
  this.showAll = showAll;
  if (showAll) {
    this.loadMyRequests(); // loads all requests
  } else {
    this.loadPendingRequests(); // loads only pending
  }
}



}
