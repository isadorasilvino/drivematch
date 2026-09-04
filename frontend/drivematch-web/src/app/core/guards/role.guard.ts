import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthStorageService } from '../auth/auth-storage.service';
import { UserRole } from '../auth/auth.service';

export const roleGuard = (allowedRoles: UserRole[]): CanActivateFn => {
  return () => {
    const authStorage = inject(AuthStorageService);
    const router = inject(Router);

    const session = authStorage.getSession();

    if (!session) {
      return router.createUrlTree(['/login']);
    }

    if (allowedRoles.includes(session.role)) {
      return true;
    }

    return router.createUrlTree(['/']);
  };
};