import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { instructorProfileGuard } from './core/guards/instructor-profile.guard';
import { roleGuard } from './core/guards/role.guard';
import { studentProfileGuard } from './core/guards/student-profile.guard';

import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';

import { AvailabilityComponent } from './features/instructor/availability/availability.component';
import { InstructorHomeComponent } from './features/instructor/instructor-home.component';
import {
    InstructorLessonRequestsComponent,
} from './features/instructor/lesson-requests/lesson-requests.component';
import {
    InstructorLessonsComponent,
} from './features/instructor/lessons/lessons.component';
import {
    ProfileComponent as InstructorProfileComponent,
} from './features/instructor/profile/profile.component';

import {
    InstructorAvailabilityComponent,
} from './features/student/instructor-availability/instructor-availability.component';
import {
    InstructorSearchComponent,
} from './features/student/instructors/instructor-search.component';
import {
    LessonCheckInComponent,
} from './features/student/lesson-check-in/lesson-check-in.component';
import {
    StudentLessonRequestsComponent,
} from './features/student/lesson-requests/lesson-requests.component';
import {
    StudentLessonsComponent,
} from './features/student/lessons/lessons.component';
import { ProfileComponent } from './features/student/profile/profile.component';
import { StudentHomeComponent } from './features/student/student-home.component';

import {
    AuthenticatedLayoutComponent,
} from './shared/layouts/authenticated-layout/authenticated-layout.component';

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
        component: AuthenticatedLayoutComponent,
        canActivate: [
            authGuard,
            roleGuard(['Student']),
        ],
        children: [
            {
                path: '',
                component: StudentHomeComponent,
                canActivate: [studentProfileGuard],
            },
            {
                path: 'profile',
                component: ProfileComponent,
            },
            {
                path: 'instructors',
                component: InstructorSearchComponent,
                canActivate: [studentProfileGuard],
            },
            {
                path: 'instructors/:instructorProfileId/availability',
                component: InstructorAvailabilityComponent,
                canActivate: [studentProfileGuard],
            },
            {
                path: 'lesson-requests',
                component: StudentLessonRequestsComponent,
                canActivate: [studentProfileGuard],
            },
            {
                path: 'lessons/check-in',
                component: LessonCheckInComponent,
                canActivate: [studentProfileGuard],
            },
            {
                path: 'lessons',
                component: StudentLessonsComponent,
                canActivate: [studentProfileGuard],
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
                component: InstructorHomeComponent,
                canActivate: [instructorProfileGuard],
            },
            {
                path: 'profile',
                component: InstructorProfileComponent,
            },
            {
                path: 'availability',
                component: AvailabilityComponent,
                canActivate: [instructorProfileGuard],
            },
            {
                path: 'lesson-requests',
                component: InstructorLessonRequestsComponent,
                canActivate: [instructorProfileGuard],
            },
            {
                path: 'lessons',
                component: InstructorLessonsComponent,
                canActivate: [instructorProfileGuard],
            },
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