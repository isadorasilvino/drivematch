import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

@Component({
    selector: 'app-dashboard-action',
    standalone: true,
    imports: [
        RouterLink,
        FontAwesomeModule,
    ],
    templateUrl: './dashboard-action.component.html',
    styleUrl: './dashboard-action.component.scss',
})
export class DashboardActionComponent {
    @Input({ required: true }) title = '';
    @Input() description = '';
    @Input({ required: true }) route = '';
    @Input({ required: true }) icon!: IconDefinition;
}