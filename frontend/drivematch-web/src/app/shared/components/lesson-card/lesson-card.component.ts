import { CommonModule } from '@angular/common';
import {
    Component,
    input,
} from '@angular/core';

import {
    LessonListItem,
    LessonStatus,
} from '../../../core/lesson/lesson.service';

@Component({
    selector: 'app-lesson-card',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './lesson-card.component.html',
    styleUrl: './lesson-card.component.scss',
})
export class LessonCardComponent {
    readonly lesson = input.required<LessonListItem>();
    readonly personLabel = input.required<string>();
    readonly personName = input.required<string>();

    statusLabel(status: LessonStatus): string {
        const labels: Record<LessonStatus, string> = {
            Scheduled: 'Agendada',
            CheckIn: 'Aguardando check-in',
            InProgress: 'Em andamento',
            Completed: 'Concluída',
            Cancelled: 'Cancelada',
            NotAttended: 'Não compareceu',
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