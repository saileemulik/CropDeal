import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AdminService } from '../../app/services/admin.service';
import { UserRole, UserStatus } from '../../app/models/signup';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-updateprofile',
  imports: [CommonModule,FormsModule],
  templateUrl: './updateprofile.component.html',
  styleUrl: './updateprofile.component.css'
})
export class UpdateprofileComponent {
  activeSection: string = '';
  allUsers: any = {};
  status: UserStatus = UserStatus.Active;
  filteredUsers: any[] = [];
  userStatusList: string[] = Object.values(UserStatus);
  isUserEditing = false;
  selectedUser: any = {};
  stars = [1, 2, 3, 4, 5];
  availableRoles : string[] = Object.values(UserRole);
hoveredRating = 0;
searchName: string = '';
selectedRole: string = '';

  constructor(private adminService : AdminService, private snackbar : SnackbarService){}

   ngOnInit(): void {
    this.activeSection = 'manageuser';
    this.selectedUser = null;
    this.getUsers();
  }
  isGridView = true; // Default view mode

toggleView() {
  this.isGridView = !this.isGridView;
}


   getUsers() {
  this.adminService.getAllUsers().subscribe({
    next: (data) => {
      this.allUsers = data;
      this.filteredUsers = data; // initialize with all data
      this.isUserEditing = false;
    },
    error: (error) => {
      console.error('Error fetching users:', error);
    }
  });
}
filterUsers() {
  const nameFilter = this.searchName.toLowerCase();
  const roleFilter = this.selectedRole;

  this.filteredUsers = this.allUsers.filter((user : any) => {
    const matchesName = user.name?.toLowerCase().includes(nameFilter);
    const matchesRole = roleFilter ? user.role === roleFilter : true;
    return matchesName && matchesRole;
  });
}


  deleteUser(id: string) {
    if (!id) {
      this.snackbar.warning('No user selected to delete');
      return;
    }

    this.adminService.deleteUser(id).subscribe({
      next: () => {
        this.snackbar.success('User deleted successfully');
        this.getUsers();
      },
      error: (err) => {
        console.error('Failed to delete user:', err);
        this.snackbar.error('Failed to delete user');
      }
    });
  }

  enableUserEdit(user: any) {
    this.isUserEditing = true;
    this.selectedUser = { ...user };
  }

  cancelUserEdit() {
    this.isUserEditing = false;
    this.selectedUser = null;
  }

  updateUserProfile() {
    this.adminService.editUsers(this.selectedUser.id, this.selectedUser).subscribe({
      next: () => {
        // this.activeSection = 'editUsers';
        console.log('Sending update:', {
  id: this.selectedUser.id,
  status: this.selectedUser.status,
  averageRating: this.selectedUser.averageRating
});
        this.snackbar.success("User profile updated successfully");
  
        this.cancelUserEdit();
        this.getUsers();
      },
      error: (error) => {
        console.error("Error updating user profile", error);
       this.snackbar.error("Error updating user profile");
      }
    });
  }
  onStatusChange(event: Event) {
  const checked = (event.target as HTMLInputElement).checked;
  if (this.selectedUser) {
    this.selectedUser.status = checked ? 'Active' : 'Inactive';
  }
}

  toggleStatus() {
  this.selectedUser.status = this.selectedUser.status === 'Active' ? 'Inactive' : 'Active';
}


setRating(value: number) {
  this.selectedUser.averageRating = value;
}

hoverRating(value: number) {
  if (value > 0) {
    this.selectedUser.averageRating = value - 0.5;
  } else {
    
  }
}
}
