import { CommonModule } from '@angular/common';
import {
    Component,
    input,
} from '@angular/core';

import {
    LessonRequestListItem,
    LessonRequestStatus,
} from '../../../core/lesson-request/lesson-request.service';

@Component({
    selector: 'app-lesson-request-card',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './lesson-request-card.component.html',
    styleUrl: './lesson-request-card.component.scss',
})
export class LessonRequestCardComponent {
    readonly request = input.required<LessonRequestListItem>();
    readonly personLabel = input.required<string>();
    readonly personName = input.required<string>();

    statusLabel(status: LessonRequestStatus): string {
        const labels: Record<LessonRequestStatus, string> = {
            Pending: 'Pendente',
            Accepted: 'Aceita',
            Confirmed: 'Confirmada',
            Rejected: 'Recusada',
            Cancelled: 'Cancelada',
            Expired: 'Expirada',
        };

        return labels[status];
    }

    formatDate(date: string): string {
        const [year, month, day] = date.split('-').map(Number);

        return new Intl.DateTimeFormat('pt-BR', {
            day: '2-digit',
            month: 'long',
            year: 'numeric',
        }).format(new Date(year, month - 1, day));
    }

    formatTime(time: string): string {
        return time.slice(0, 5);
    }
}