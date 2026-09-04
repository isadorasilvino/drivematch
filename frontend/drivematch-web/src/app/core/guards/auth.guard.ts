import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthStorageService } from '../auth/auth-storage.service';

export const authGuard: CanActivateFn = () => {
  const authStorage = inject(AuthStorageService);
  const router = inject(Router);

  if (authStorage.getToken()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};