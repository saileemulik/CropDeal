import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { UserRole } from '../models/signup';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private userSubject = new BehaviorSubject<any>(null);
  user$ = this.userSubject.asObservable();
  private currentRoleSubject = new BehaviorSubject<string | null>(null);
  public currentRole$ = this.currentRoleSubject.asObservable();

  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  isLoggedIn$ = this.isLoggedInSubject.asObservable();
  private baseUrl = 'http://localhost:5035/api/Auth';

  constructor(private router: Router) {
    // const user = localStorage.getItem('user');
    // this.userSubject.next(user ? JSON.parse(user) : null);
    try {
    const userStr = localStorage.getItem('user');

    // Check for null, undefined string, or empty
    if (userStr && userStr !== 'undefined') {
      this.userSubject.next(JSON.parse(userStr));
    } else {
      this.userSubject.next(null);
    }
  } catch (e) {
    console.error('Invalid user data in localStorage:', e);
    localStorage.removeItem('user');
    this.userSubject.next(null);
  }
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }

  login(token: string, user: any) {
    localStorage.setItem('token', token);
    this.userSubject.next(user);
    this.isLoggedInSubject.next(true);
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('user');
    this.userSubject.next(null);
    this.isLoggedInSubject.next(false);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getUser() {
    return this.userSubject.value;
  }

  setUserRole(role: string) {
    this.currentRoleSubject.next(role);
    localStorage.setItem('role', role);
  }

  getUserRole(): string | null {
    return localStorage.getItem('role');
  }

  initUserRole() {
    const role = this.getUserRole();
    if (role) {
      this.currentRoleSubject.next(role);
    }
  }

  loginWithGoogle(role: UserRole = UserRole.Farmer) {
    const returnUrl = window.location.origin + '/auth-callback';
    const url = `${this.baseUrl}/LoginWithGoogle?role=${role}&returnUrl=${encodeURIComponent(returnUrl)}`;
    window.location.href = url;
  }

  initLoginStatus() {
  this.isLoggedInSubject.next(this.hasToken());
}

  handleLoginCallback() {

    const storedUserRole = localStorage.getItem('userRole') as UserRole;

    switch (storedUserRole) {
      case UserRole.Admin:
        this.router.navigate(['/admindashboard']);
        break;
      case UserRole.Dealer:
        this.router.navigate(['/dealerdashboard']);
        break;
      case UserRole.Farmer:
      default:
        this.router.navigate(['/farmerdashboard']);
        break;
    }
  }

}
