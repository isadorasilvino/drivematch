import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { studentProfileGuard } from './core/guards/student-profile.guard';
import { instructorProfileGuard } from './core/guards/instructor-profile.guard';

import { ProfileComponent as InstructorProfileComponent } from './features/instructor/profile/profile.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { AvailabilityComponent } from './features/instructor/availability/availability.component';
import { StudentHomeComponent } from './features/student/student-home.component';
import { ProfileComponent } from './features/student/profile/profile.component';
import { InstructorSearchComponent } from './features/student/instructors/instructor-search.component';
import { InstructorHomeComponent } from './features/instructor/instructor-home.component';
import { InstructorAvailabilityComponent } from './features/student/instructor-availability/instructor-availability.component';

import {
  StudentLessonRequestsComponent,
} from './features/student/lesson-requests/lesson-requests.component';

import {
  InstructorLessonRequestsComponent,
} from './features/instructor/lesson-requests/lesson-requests.component';

import {
  StudentLessonsComponent,
} from './features/student/lessons/lessons.component';

import {
  InstructorLessonsComponent,
} from './features/instructor/lessons/lessons.component';

import { LessonCheckInComponent } from './features/student/lesson-check-in/lesson-check-in.component';
export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  {
    path: 'student/profile',
    component: ProfileComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
    ],
  },
  {
    path: 'student',
    component: StudentHomeComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
      studentProfileGuard,
    ],
  },
  {
    path: 'instructor/availability',
    component: AvailabilityComponent,
    canActivate: [
      authGuard,
      roleGuard(['Instructor']),
      instructorProfileGuard,
    ],
  },
  {
    path: 'instructor/profile',
    component: InstructorProfileComponent,
    canActivate: [
      authGuard,
      roleGuard(['Instructor']),
    ],
  },
  {
    path: 'instructor',
    component: InstructorHomeComponent,
    canActivate: [
      authGuard,
      roleGuard(['Instructor']),
      instructorProfileGuard,
    ],
  },
  {
    path: 'student/instructors',
    component: InstructorSearchComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
      studentProfileGuard,
    ],
  },
  {
    path: 'student/instructors/:instructorProfileId/availability',
    component: InstructorAvailabilityComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
      studentProfileGuard,
    ],
  },
  {
    path: 'student/lesson-requests',
    component: StudentLessonRequestsComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
      studentProfileGuard,
    ],
  },
  {
    path: 'instructor/lesson-requests',
    component: InstructorLessonRequestsComponent,
    canActivate: [
      authGuard,
      roleGuard(['Instructor']),
      instructorProfileGuard,
    ],
  },
  {
    path: 'student/lessons/check-in',
    component: LessonCheckInComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
      studentProfileGuard,
    ],
  },
  {
    path: 'student/lessons',
    component: StudentLessonsComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
      studentProfileGuard,
    ],
  },
  {
    path: 'instructor/lessons',
    component: InstructorLessonsComponent,
    canActivate: [
      authGuard,
      roleGuard(['Instructor']),
      instructorProfileGuard,
    ],
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];