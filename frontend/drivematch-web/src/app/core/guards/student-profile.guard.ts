import { HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';

import { StudentProfileService } from '../student/student-profile.service';

export const studentProfileGuard: CanActivateFn = () => {
  const studentProfileService = inject(StudentProfileService);
  const router = inject(Router);

  return studentProfileService.getProfile().pipe(
    map(() => true),

    catchError((error: HttpErrorResponse) => {
      if (error.status === 404) {
        return of(
          router.createUrlTree(['/student/profile']),
        );
      }

      return of(
        router.createUrlTree(['/login']),
      );
    }),
  );
};