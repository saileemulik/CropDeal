import { Injectable, ErrorHandler } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorHandlerService implements ErrorHandler {

  constructor(private snackBar: MatSnackBar) {}

  handleError(error: any): void {
    console.error('Global error caught:', error);
    
    let errorMessage = 'An unexpected error occurred';
    
    if (error?.message) {
      errorMessage = error.message;
    } else if (typeof error === 'string') {
      errorMessage = error;
    }

    // Show user-friendly error message
    this.snackBar.open(errorMessage, 'Close', {
      duration: 5000,
      panelClass: ['snackbar', 'snackbar-error'],
      horizontalPosition: 'center',
      verticalPosition: 'top'
    });

    // Log to external service in production
    this.logErrorToService(error);
  }

  private logErrorToService(error: any): void {
    // In production, you would send this to a logging service
    // like Sentry, LogRocket, or your own logging endpoint
    console.error('Error logged to service:', {
      message: error?.message || 'Unknown error',
      stack: error?.stack,
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      url: window.location.href
    });
  }
}