import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, RouterLink, RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';
import { filter } from 'rxjs/operators';
import { LoginService } from './services/login.service';
import { UserRole } from './models/signup';
import { ChatComponent } from './chat/chat.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, FormsModule, CommonModule, RouterLink, ChatComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'cropdealfrontend';
  isLoggedIn = false;
  loginButton : boolean = true;
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  activeSection: string = '';
  hideNavbar = false;



  constructor(private router: Router, private authService: AuthService,
    private loginService : LoginService
  ) {this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.hideNavbar = ['/login', '/signup'].includes(event.urlAfterRedirects);
      }
    });
     this.router.events.pipe(
    filter(event => event instanceof NavigationEnd)
  ).subscribe(() => {
    // Immediately update login and role status on every navigation end event
    this.isLoggedIn = this.authService.isLoggedIn();

    const roleStr = this.authService.getUserRole();
    if (roleStr && roleStr in UserRole) {
      this.currentUserRole = roleStr as UserRole;
    } else {
      this.currentUserRole = null;
    }
  });
  }

  ngOnInit() {
    this.authService.currentRole$.subscribe(role => {
    this.currentUserRole = role as UserRole;
  });

  this.authService.initUserRole();

  // Add this line to sync login status on app load:
  this.authService.initLoginStatus();

  
    const token = localStorage.getItem('token');
  const role = localStorage.getItem('role');

  this.isLoggedIn = !!token;
  
     const roleStr = this.loginService.getUserRole(); 
  if (roleStr in UserRole) {
    this.currentUserRole = roleStr as UserRole;
  } else {
    this.currentUserRole = null; 
  }
   
    this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
    });


    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.isLoggedIn = this.authService.isLoggedIn();
      });
    
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
    this.authService.logout();
    localStorage.clear();
    this.router.navigate(['/home']);
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }
  setActiveSection(section: string): void {
    this.activeSection = section;
  }
}
