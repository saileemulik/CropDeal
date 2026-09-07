import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private snackBar: MatSnackBar) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        // Only show error if it's not handled by component
        if (!this.isValidationError(error)) {
          this.handleError(error);
        }
        return throwError(() => error);
      })
    );
  }

  private isValidationError(error: HttpErrorResponse): boolean {
    return error.status === 400 && error.error?.errors;
  }

  private handleError(error: HttpErrorResponse): void {
    let errorMsg = 'An unexpected error occurred';

    if (Array.isArray(error.error)) {
      error.error.forEach((err: any) => {
        this.showError(err.description || 'Validation error');
      });
      return;
    }



    if (error.error?.message) {
      errorMsg = error.error.message;
    } else {
      switch (error.status) {
        case 0:
          errorMsg = 'Network error - please check your connection';
          break;
        case 400:
          errorMsg = error.error?.message || 'Invalid request';
          break;
        case 401:
          errorMsg = 'Authentication required - please login';
          break;
        case 403:
          errorMsg = 'Access denied - insufficient permissions';
          break;
        case 404:
          errorMsg = 'Resource not found';
          break;
        case 408:
          errorMsg = 'Request timeout - please try again';
          break;
        case 500:
          errorMsg = error.error?.detail || 'Server error occurred';
          break;
        case 503:
          errorMsg = 'Service unavailable - please try again later';
          break;
        default:
          errorMsg = `Error ${error.status}: ${error.statusText || 'Unknown error'}`;
      }
    }

    this.showError(errorMsg);
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 8000,
      panelClass: ['snackbar', 'snackbar-error'],
      horizontalPosition: 'center',
      verticalPosition: 'top'
    });
  }
}
