import {
    Component,
    input,
} from '@angular/core';

@Component({
    selector: 'app-page-layout',
    standalone: true,
    templateUrl: './page-layout.component.html',
    styleUrl: './page-layout.component.scss',
})
export class PageLayoutComponent {
    readonly eyebrow = input.required<string>();
    readonly title = input.required<string>();
    readonly description = input<string>();
    readonly showHeader = input(true);
}