import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { AuthStorageService } from '../auth/auth-storage.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authStorage = inject(AuthStorageService);
  const token = authStorage.getToken();

  if (!token) {
    return next(request);
  }

  const authenticatedRequest = request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
    },
  });

  return next(authenticatedRequest);
};