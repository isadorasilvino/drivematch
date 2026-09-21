import { Component, Input } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

@Component({
    selector: 'app-dashboard-stat',
    standalone: true,
    imports: [
        FontAwesomeModule,
    ],
    templateUrl: './dashboard-stat.component.html',
    styleUrl: './dashboard-stat.component.scss',
})
export class DashboardStatComponent {
    @Input({ required: true }) label = '';
    @Input({ required: true }) value: string | number = 0;
    @Input({ required: true }) icon!: IconDefinition;
}