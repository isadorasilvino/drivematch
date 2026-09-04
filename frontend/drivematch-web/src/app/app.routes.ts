import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { InstructorHomeComponent } from './features/instructor/instructor-home.component';
import { StudentHomeComponent } from './features/student/student-home.component';
import { RegisterComponent } from './features/auth/register/register.component';

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
    path: 'student',
    component: StudentHomeComponent,
    canActivate: [
      authGuard,
      roleGuard(['Student']),
    ],
  },
  {
    path: 'instructor',
    component: InstructorHomeComponent,
    canActivate: [
      authGuard,
      roleGuard(['Instructor']),
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
