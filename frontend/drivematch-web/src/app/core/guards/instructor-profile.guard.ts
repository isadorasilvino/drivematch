import { HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';

import { InstructorProfileService } from '../instructor/instructor-profile.service';

export const instructorProfileGuard: CanActivateFn = () => {
  const instructorProfileService = inject(InstructorProfileService);
  const router = inject(Router);

  return instructorProfileService.getProfile().pipe(
    map(() => true),

    catchError((error: HttpErrorResponse) => {
      if (error.status === 404) {
        return of(
          router.createUrlTree(['/instructor/profile']),
        );
      }

      return of(
        router.createUrlTree(['/login']),
      );
    }),
  );
};