import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { instructorProfileGuard } from './core/guards/instructor-profile.guard';
import { roleGuard } from './core/guards/role.guard';
import { studentProfileGuard } from './core/guards/student-profile.guard';

import {
  AuthenticatedLayoutComponent,
} from './shared/layouts/authenticated-layout/authenticated-layout.component';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home.component').then(
        (m) => m.HomeComponent,
      ),
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(
        (m) => m.LoginComponent,
      ),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(
        (m) => m.RegisterComponent,
      ),
  },
  {
    path: 'student',
    component: AuthenticatedLayoutComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
    ],
    children: [
      {
        path: '',
        loadComponent: () =>
          import(
            './features/student/student-home.component'
          ).then(
            (m) => m.StudentHomeComponent,
          ),
        canActivate: [
          studentProfileGuard,
        ],
      },
      {
        path: 'profile',
        loadComponent: () =>
          import(
            './features/student/profile/profile.component'
          ).then(
            (m) => m.ProfileComponent,
          ),
      },
      {
        path: 'instructors',
        loadComponent: () =>
          import(
            './features/student/instructors/instructor-search.component'
          ).then(
            (m) => m.InstructorSearchComponent,
          ),
        canActivate: [
          studentProfileGuard,
        ],
      },
      {
        path: 'instructors/:instructorProfileId/availability',
        loadComponent: () =>
          import(
            './features/student/instructor-availability/instructor-availability.component'
          ).then(
            (m) => m.InstructorAvailabilityComponent,
          ),
        canActivate: [
          studentProfileGuard,
        ],
      },
      {
        path: 'lesson-requests',
        loadComponent: () =>
          import(
            './features/student/lesson-requests/lesson-requests.component'
          ).then(
            (m) => m.StudentLessonRequestsComponent,
          ),
        canActivate: [
          studentProfileGuard,
        ],
      },
      {
        path: 'lessons/check-in',
        loadComponent: () =>
          import(
            './features/student/lesson-check-in/lesson-check-in.component'
          ).then(
            (m) => m.LessonCheckInComponent,
          ),
        canActivate: [
          studentProfileGuard,
        ],
      },
      {
        path: 'lessons',
        loadComponent: () =>
          import(
            './features/student/lessons/lessons.component'
          ).then(
            (m) => m.StudentLessonsComponent,
          ),
        canActivate: [
          studentProfileGuard,
        ],
      },
    ],
  },
  {
    path: 'instructor',
    component: AuthenticatedLayoutComponent,
    canActivate: [
      authGuard,
      roleGuard(['Instructor']),
    ],
    children: [
      {
        path: '',
        loadComponent: () =>
          import(
            './features/instructor/instructor-home.component'
          ).then(
            (m) => m.InstructorHomeComponent,
          ),
        canActivate: [
          instructorProfileGuard,
        ],
      },
      {
        path: 'profile',
        loadComponent: () =>
          import(
            './features/instructor/profile/profile.component'
          ).then(
            (m) => m.ProfileComponent,
          ),
      },
      {
        path: 'availability',
        loadComponent: () =>
          import(
            './features/instructor/availability/availability.component'
          ).then(
            (m) => m.AvailabilityComponent,
          ),
        canActivate: [
          instructorProfileGuard,
        ],
      },
      {
        path: 'lesson-requests',
        loadComponent: () =>
          import(
            './features/instructor/lesson-requests/lesson-requests.component'
          ).then(
            (m) => m.InstructorLessonRequestsComponent,
          ),
        canActivate: [
          instructorProfileGuard,
        ],
      },
      {
        path: 'lessons',
        loadComponent: () =>
          import(
            './features/instructor/lessons/lessons.component'
          ).then(
            (m) => m.InstructorLessonsComponent,
          ),
        canActivate: [
          instructorProfileGuard,
        ],
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];