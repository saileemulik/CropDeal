import { Component } from '@angular/core';
import { UserService } from '../../app/services/user.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { UserRole } from '../../app/models/signup';
import { LoginService } from '../../app/services/login.service';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-users',
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent {
  user: any = {};
  activeSection: string = '';
  isEditing = false;
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;

  constructor (private userProfileService: UserService, private loginService : LoginService, public router : Router, private snackbar: SnackbarService){}

  ngOnInit(): void {
    this.currentUserRole = this.loginService.getUserRole() as UserRole;
    this.activeSection = 'users';
    this.loadUserProfile();
  }
isActive(route: string): boolean {
  return this.router.url === route;
}
  loadUserProfile() {
    this.userProfileService.getUserProfile().subscribe({
      next: (data) => {
        this.user = data;
      },
      error: (error) => {
        console.error('Error fetching user profile', error);
      }
    });
  }

  enableEdit() {
    this.isEditing = true;
  }

  cancelEdit() {
    this.isEditing = false;
    this.loadUserProfile();
  }

  updateProfile(): void {
    this.userProfileService.updateUserProfile(this.user).subscribe({
      next: (response) => {
        this.snackbar.success(response.message);
        this.loadUserProfile();
        this.isEditing = false;
      },
      error: (error) => {
        console.error('Error updating profile', error);
        this.snackbar.error('There was an error updating the profile.');
      }
    });
  }
getFullStars(rating: number): number[] {
  return Array(Math.floor(rating)).fill(0);
}

hasHalfStar(rating: number): boolean {
  return rating % 1 >= 0.25 && rating % 1 < 0.75;
}

getEmptyStars(rating: number): number[] {
  const fullStars = Math.floor(rating);
  const hasHalf = this.hasHalfStar(rating) ? 1 : 0;
  return Array(5 - fullStars - hasHalf).fill(0);
}

}
