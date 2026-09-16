import {
    Component,
    input,
    output,
} from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrashCan } from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-destructive-action',
    standalone: true,
    imports: [FontAwesomeModule],
    templateUrl: './destructive-action.component.html',
    styleUrl: './destructive-action.component.scss',
})
export class DestructiveActionComponent {
    readonly label = input.required<string>();
    readonly disabled = input(false);
    readonly triggered = output<void>();

    protected readonly faTrashCan = faTrashCan;

    trigger(): void {
        if (!this.disabled()) {
            this.triggered.emit();
        }
    }
}