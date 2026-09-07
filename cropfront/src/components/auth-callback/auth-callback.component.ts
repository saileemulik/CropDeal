import { Component } from '@angular/core';
import { AuthService } from '../../app/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserRole } from '../../app/models/signup';

@Component({
  selector: 'app-auth-callback',
  imports: [],
  templateUrl: './auth-callback.component.html',
  styleUrl: './auth-callback.component.css'
})
export class AuthCallbackComponent {
  constructor(private authService: AuthService, private route: ActivatedRoute, private router: Router) {}

 ngOnInit() {
  const queryParams = this.route.snapshot.queryParams;
  const token = queryParams['token'];
  const role = queryParams['role'];

  console.log('Token:', token);
  console.log('Role:', role);

  if (token) {
    localStorage.setItem('token', token);
  }

  if (role) {
    localStorage.setItem('role', role);
  }

  this.authService.handleLoginCallback(); // Optional

  setTimeout(() => {
    console.log('Navigating based on role:', role);
    if (role === 'Farmer') {
      this.router.navigate(['/farmerdashboard']);
    } else if (role === 'Dealer') {
      this.router.navigate(['/dealerdashboard']);
    } else if (role === 'Admin') {
      this.router.navigate(['/admindashboard']);
    } else {
      this.router.navigate(['/home']);
    }
  }, 300);
}

}





