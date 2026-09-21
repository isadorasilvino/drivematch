import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import {
    faArrowRight,
    faCalendarDays,
    faCheck,
    faCircleCheck,
    faClock,
    faCodeBranch,
    faGraduationCap,
    faLocationDot,
    faLock,
    faMagnifyingGlass,
    faQrcode,
    faShieldHalved,
    faStar,
    faUserCheck,
    faUsers,
} from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [
        RouterLink,
        FontAwesomeModule,
    ],
    templateUrl: './home.component.html',
    styleUrl: './home.component.scss',
})
export class HomeComponent {
    protected readonly faArrowRight = faArrowRight;
    protected readonly faCalendarDays = faCalendarDays;
    protected readonly faCheck = faCheck;
    protected readonly faCircleCheck = faCircleCheck;
    protected readonly faClock = faClock;
    protected readonly faCodeBranch = faCodeBranch;
    protected readonly faGraduationCap = faGraduationCap;
    protected readonly faLocationDot = faLocationDot;
    protected readonly faLock = faLock;
    protected readonly faMagnifyingGlass = faMagnifyingGlass;
    protected readonly faQrcode = faQrcode;
    protected readonly faShieldHalved = faShieldHalved;
    protected readonly faStar = faStar;
    protected readonly faUserCheck = faUserCheck;
    protected readonly faUsers = faUsers;
}