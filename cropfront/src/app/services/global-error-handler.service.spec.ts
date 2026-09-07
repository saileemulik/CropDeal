import { TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GlobalErrorHandlerService } from './global-error-handler.service';

describe('GlobalErrorHandlerService', () => {
  let service: GlobalErrorHandlerService;
  let mockSnackBar: jasmine.SpyObj<MatSnackBar>;

  beforeEach(() => {
    const snackBarSpy = jasmine.createSpyObj('MatSnackBar', ['open']);

    TestBed.configureTestingModule({
      providers: [
        GlobalErrorHandlerService,
        { provide: MatSnackBar, useValue: snackBarSpy }
      ]
    });
    
    service = TestBed.inject(GlobalErrorHandlerService);
    mockSnackBar = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should handle error with message', () => {
    const error = new Error('Test error message');
    spyOn(console, 'error');

    service.handleError(error);

    expect(mockSnackBar.open).toHaveBeenCalledWith(
      'Test error message',
      'Close',
      jasmine.objectContaining({
        duration: 5000,
        panelClass: ['snackbar', 'snackbar-error']
      })
    );
    expect(console.error).toHaveBeenCalled();
  });

  it('should handle string error', () => {
    const error = 'String error message';
    spyOn(console, 'error');

    service.handleError(error);

    expect(mockSnackBar.open).toHaveBeenCalledWith(
      'String error message',
      'Close',
      jasmine.objectContaining({
        duration: 5000
      })
    );
  });

  it('should handle unknown error', () => {
    const error = null;
    spyOn(console, 'error');

    service.handleError(error);

    expect(mockSnackBar.open).toHaveBeenCalledWith(
      'An unexpected error occurred',
      'Close',
      jasmine.objectContaining({
        duration: 5000
      })
    );
  });
});