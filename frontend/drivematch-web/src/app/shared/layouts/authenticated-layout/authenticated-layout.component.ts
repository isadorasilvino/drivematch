import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import {
    AppNavbarComponent,
} from '../../components/app-navbar/app-navbar.component';

@Component({
    selector: 'app-authenticated-layout',
    standalone: true,
    imports: [
        RouterOutlet,
        AppNavbarComponent,
    ],
    templateUrl: './authenticated-layout.component.html',
    styleUrl: './authenticated-layout.component.scss',
})
export class AuthenticatedLayoutComponent {}