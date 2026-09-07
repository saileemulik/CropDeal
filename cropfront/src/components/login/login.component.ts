import { Component } from '@angular/core';
import { LoginService } from '../../app/services/login.service'; 
import { Router, RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule, Location } from '@angular/common';
import { UserRole } from '../../app/models/signup';
import { AuthService } from '../../app/services/auth.service';
import { SnackbarService } from '../../app/services/snackbar.service';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ HttpClientModule, FormsModule, CommonModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  otp = '';
 message: string = '';
  password: string = '';
  loginError: string = '';
showPassword: boolean = false;
showNewPassword: boolean = false;

  UserRole = UserRole;

  forgotEmail: string = '';
showForgotPasswordForm: boolean = false;

resetToken: string = '';
newPassword: string = '';
showResetPasswordForm: boolean = false;
 roles = Object.values(UserRole);
  selectedRole: UserRole = UserRole.Farmer;
showGoogleContainer = false;

  constructor(private loginService: LoginService, private authService: AuthService, private router: Router, private location: Location, private snackbar: SnackbarService) {}

   onGoogleLogin() {
    this.authService.loginWithGoogle(this.selectedRole);
  }

  onSubmit() {
    const loginData = { email: this.email, password: this.password };

    this.loginService.login(loginData).subscribe({
      next: (response) => {
        console.log('Login successful', response);
        this.loginService.saveLoginData(response);


        // Use AuthService to set login status and token
        this.authService.login(response.token, response.user);
        

        // Save other details to localStorage
        localStorage.setItem('userId', response.userId);
        localStorage.setItem('role', response.role);
        this.authService.setUserRole(response.role);
        this.snackbar.success("Login Successful");

        const role: UserRole = response.role as UserRole;

        if (role === UserRole.Admin) {
          this.router.navigate(['/admindashboard']);
        } else if (role === UserRole.Farmer) {
          this.router.navigate(['/farmerdashboard']);
        } else if (role === UserRole.Dealer) {
          this.router.navigate(['/dealerdashboard']);
        } else {
          this.router.navigate(['/']);
        }
        this.loginService.saveLoginData(response);
      },
     error: (error) => {
  console.error('Login failed', error);

  // Check if backend provided a specific error message
  let backendMessage = 'Login Failed';
  if (error.status === 401 && error.error) {
    backendMessage = error.error; // This will show: "Your account is inactive. Please contact support."
  }

  this.snackbar.error(backendMessage);
  this.loginError = backendMessage;
}

    });
  }

  // Trigger forgot password
submitForgotPassword() {
  if (!this.forgotEmail) {
    this.snackbar.warning("Please enter your email.");
    return;
  }

  this.loginService.forgotPassword(this.forgotEmail).subscribe({
    next: (res) => {
     this.snackbar.info("Reset token sent to your email (or check console).");

      this.email = this.forgotEmail;

      this.showForgotPasswordForm = false;
      this.showResetPasswordForm = true;
    },
    error: (err) => {
      console.error(err);
      this.snackbar.error("Error sending token. Email might be invalid.");
    }
  });
}


// Submit new password with token
reset() {
  console.log(this.email); // Add this before the service call

  this.loginService.resetPassword({
    email: this.email,
    otp: this.otp, 
    newPassword: this.newPassword
  }).subscribe({
    next: (res) => {
      console.log(this.email);
      console.log(this.otp);
      console.log(this.newPassword);
      console.log(res);
      this.message = 'Password reset successfully.';
      this.snackbar.success('Password reset successfully.');
    },
    error: err => {
      console.error(err);
      this.message = 'Failed to reset password.';
    }
  });
}


generateOtp(): string {
    return Math.floor(100000 + Math.random() * 900000).toString();
  }

  
  sendOtp() {
    this.loginService.forgotPassword(this.email).subscribe({
      next: res => this.message = 'OTP sent to email.',
      error: err => this.message = 'Error sending OTP.'
    });
  }

   verifyOtp() {
    this.loginService.verifyOtp({ email: this.email, otp: this.otp }).subscribe({
      next: res => this.message = 'OTP verified. You can now reset your password.',
      error: err => this.message = 'Invalid OTP.'
    });
  }
goBack() {
  this.location.back();
}
  
  
}


