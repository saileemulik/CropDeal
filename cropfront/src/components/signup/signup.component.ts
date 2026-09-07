import { Component } from '@angular/core';
import { SignupService } from '../../app/services/signup.service';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule  } from '@angular/router';
import { UserRole, Signup } from '../../app/models/signup';
import { CommonModule, Location } from '@angular/common';
import { SnackbarService } from '../../app/services/snackbar.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule,FormsModule, HttpClientModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {
  name: string = '';
  email: string = '';
  phoneNumber: string = '';
  password: string = '';
  confirmPassword: string = '';
  location: string = '';
  role: UserRole = UserRole.Farmer;
  showPassword: boolean = false;
showNewPassword: boolean = false;

  nameError: string = '';
  emailError: string = '';
  phoneNumberError: string = '';
  passwordError: string = '';
  confirmPasswordError: string = '';
  locationError: string = '';
  roleError: string = '';
  
  userRoleList: string[] = Object.values(UserRole);

  
  constructor(private signupService: SignupService, private router: Router, private locations : Location, private snackbar : SnackbarService) {}

  ngOnInit() {
    console.log('SignupComponent initialized');
    console.log('User Roles:', this.userRoleList);
  }

  onSubmit() {
    if (this.password.length < 6) {
      this.passwordError = 'Password must be at least 6 characters long.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.confirmPasswordError = 'Passwords do not match';
      return;
    }
  
    const signupData: Signup = {
      name: this.name,
      email: this.email,
      phoneNumber: this.phoneNumber,
      password: this.password,
      confirmPassword: this.confirmPassword,
      location: this.location,
      role: this.role,  
      createdAt: new Date(),  // Set createdAt
      updatedAt: new Date(),
    };
  
    this.signupService.signUp(signupData, this.role).subscribe({
      next: (response) => {
        console.log('Signup successful', response);
        this.snackbar.success('Signup successful! You can now log in.');
        this.router.navigate(['/login']);
      },
      error: (error) => {
        console.error('Signup failed', error);
        if (error.error?.errors) {
          const errors = error.error.errors;
          const allErrors: string[] = [];
          
          Object.keys(errors).forEach(field => {
            const fieldErrors = errors[field];
            if (Array.isArray(fieldErrors)) {
              allErrors.push(...fieldErrors);
            } else {
              allErrors.push(fieldErrors);
            }
          });
          
          this.snackbar.error(allErrors.join('\n'));
        } else {
          this.snackbar.error('Signup failed: Please check your input');
        }
      }
    });
  }


goToLogin() {
    this.router.navigate(['/login']);
  }
  goBack() {
  this.locations.back();
}
}
