import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { inject } from '@angular/core';

export const roleGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): boolean => {
  const expectedRole = route.data['expectedRole'];
  const actualRole = localStorage.getItem('role'); 
  const router = inject(Router);

  console.log('Expected Role:', expectedRole, '| Actual Role:', actualRole);

  if (expectedRole?.toLowerCase() === actualRole?.toLowerCase()) {
    return true;
  } else {
    router.navigate(['/unauthorized']); 
    return false;
  }
};
