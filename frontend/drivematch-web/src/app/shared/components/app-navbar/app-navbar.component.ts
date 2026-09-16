import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
    faCalendarDays,
    faCarSide,
    faClock,
    faHouse,
    faRightFromBracket,
    faUser,
    faUsers,
} from '@fortawesome/free-solid-svg-icons';

import {
    AuthStorageService,
} from '../../../core/auth/auth-storage.service';
import { AuthService } from '../../../core/auth/auth.service';

interface NavigationItem {
    label: string;
    route: string;
    icon: typeof faHouse;
    exact?: boolean;
}

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [
        RouterLink,
        RouterLinkActive,
        FontAwesomeModule,
    ],
    templateUrl: './app-navbar.component.html',
    styleUrl: './app-navbar.component.scss',
})
export class AppNavbarComponent {
    private readonly router = inject(Router);
    private readonly authService = inject(AuthService);
    private readonly authStorage = inject(AuthStorageService);

    protected readonly faRightFromBracket = faRightFromBracket;

    protected readonly session = this.authStorage.getSession();

    protected readonly navigationItems: NavigationItem[] =
        this.buildNavigation();

    private buildNavigation(): NavigationItem[] {
        if (this.session?.role === 'Instructor') {
            return [
                {
                    label: 'Início',
                    route: '/instructor',
                    icon: faHouse,
                    exact: true,
                },
                {
                    label: 'Solicitações',
                    route: '/instructor/lesson-requests',
                    icon: faUsers,
                },
                {
                    label: 'Minhas aulas',
                    route: '/instructor/lessons',
                    icon: faCalendarDays,
                },
                {
                    label: 'Disponibilidade',
                    route: '/instructor/availability',
                    icon: faClock,
                },
                {
                    label: 'Perfil',
                    route: '/instructor/profile',
                    icon: faUser,
                },
            ];
        }

        return [
            {
                label: 'Início',
                route: '/student',
                icon: faHouse,
                exact: true,
            },
            {
                label: 'Instrutores',
                route: '/student/instructors',
                icon: faCarSide,
            },
            {
                label: 'Solicitações',
                route: '/student/lesson-requests',
                icon: faUsers,
            },
            {
                label: 'Minhas aulas',
                route: '/student/lessons',
                icon: faCalendarDays,
            },
            {
                label: 'Perfil',
                route: '/student/profile',
                icon: faUser,
            },
        ];
    }

    logout(): void {
        this.authService.logout();
        void this.router.navigate(['/login']);
    }
}