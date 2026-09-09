import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
    Component,
    inject,
    OnInit,
    signal,
} from '@angular/core';
import { Router } from '@angular/router';

import {
    LessonRequestListItem,
    LessonRequestService,
} from '../../../core/lesson-request/lesson-request.service';
import {
    LessonRequestCardComponent,
} from '../../../shared/components/lesson-request-card/lesson-request-card.component';

@Component({
    selector: 'app-student-lesson-requests',
    standalone: true,
    imports: [
        CommonModule,
        LessonRequestCardComponent,
    ],
    templateUrl: './lesson-requests.component.html',
    styleUrl: './lesson-requests.component.scss',
})
export class StudentLessonRequestsComponent implements OnInit {
    private readonly router = inject(Router);
    private readonly lessonRequestService =
        inject(LessonRequestService);

    readonly requests = signal<LessonRequestListItem[]>([]);
    readonly isLoading = signal(true);
    readonly errorMessage = signal<string | null>(null);

    ngOnInit(): void {
        this.loadRequests();
    }

    loadRequests(): void {
        this.isLoading.set(true);
        this.errorMessage.set(null);

        this.lessonRequestService
            .getMine()
            .subscribe({
                next: (requests) => {
                    this.requests.set(requests);
                    this.isLoading.set(false);
                },

                error: (_error: HttpErrorResponse) => {
                    this.requests.set([]);
                    this.isLoading.set(false);
                    this.errorMessage.set(
                        'Não foi possível carregar suas solicitações.',
                    );
                },
            });
    }

    findInstructors(): void {
        void this.router.navigate(['/student/instructors']);
    }

    goHome(): void {
        void this.router.navigate(['/student']);
    }
}