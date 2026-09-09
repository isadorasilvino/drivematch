import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
    Component,
    inject,
    OnInit,
    signal,
} from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import {
    LessonRequestListItem,
    LessonRequestService,
} from '../../../core/lesson-request/lesson-request.service';
import {
    LessonRequestCardComponent,
} from '../../../shared/components/lesson-request-card/lesson-request-card.component';

@Component({
    selector: 'app-instructor-lesson-requests',
    standalone: true,
    imports: [
        CommonModule,
        LessonRequestCardComponent,
    ],
    templateUrl: './lesson-requests.component.html',
    styleUrl: './lesson-requests.component.scss',
})
export class InstructorLessonRequestsComponent implements OnInit {
    private readonly router = inject(Router);
    private readonly lessonRequestService =
        inject(LessonRequestService);

    readonly requests = signal<LessonRequestListItem[]>([]);
    readonly isLoading = signal(true);
    readonly errorMessage = signal<string | null>(null);
    readonly actionRequestId = signal<string | null>(null);

    ngOnInit(): void {
        this.loadRequests();
    }

    loadRequests(): void {
        this.isLoading.set(true);
        this.errorMessage.set(null);

        this.lessonRequestService
            .getReceived()
            .subscribe({
                next: (requests) => {
                    this.requests.set(requests);
                    this.isLoading.set(false);
                },

                error: (_error: HttpErrorResponse) => {
                    this.requests.set([]);
                    this.isLoading.set(false);
                    this.errorMessage.set(
                        'Não foi possível carregar as solicitações recebidas.',
                    );
                },
            });
    }

    accept(request: LessonRequestListItem): void {
        if (
            request.status !== 'Pending' ||
            this.actionRequestId()
        ) {
            return;
        }

        this.errorMessage.set(null);
        this.actionRequestId.set(request.lessonRequestId);

        this.lessonRequestService
            .accept(request.lessonRequestId)
            .pipe(
                finalize(() => this.actionRequestId.set(null)),
            )
            .subscribe({
                next: () => {
                    this.updateStatus(
                        request.lessonRequestId,
                        'Accepted',
                    );
                },

                error: (error: HttpErrorResponse) => {
                    this.errorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível aceitar a solicitação.',
                        ),
                    );
                },
            });
    }

    reject(request: LessonRequestListItem): void {
        if (
            request.status !== 'Pending' ||
            this.actionRequestId()
        ) {
            return;
        }

        this.errorMessage.set(null);
        this.actionRequestId.set(request.lessonRequestId);

        this.lessonRequestService
            .reject(request.lessonRequestId)
            .pipe(
                finalize(() => this.actionRequestId.set(null)),
            )
            .subscribe({
                next: () => {
                    this.updateStatus(
                        request.lessonRequestId,
                        'Rejected',
                    );
                },

                error: (error: HttpErrorResponse) => {
                    this.errorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível recusar a solicitação.',
                        ),
                    );
                },
            });
    }

    isProcessing(lessonRequestId: string): boolean {
        return this.actionRequestId() === lessonRequestId;
    }

    goHome(): void {
        void this.router.navigate(['/instructor']);
    }

    private updateStatus(
        lessonRequestId: string,
        status: LessonRequestListItem['status'],
    ): void {
        this.requests.update((requests) =>
            requests.map((request) =>
                request.lessonRequestId === lessonRequestId
                    ? {
                        ...request,
                        status,
                        updatedAt: new Date().toISOString(),
                    }
                    : request,
            ),
        );
    }

    private getActionError(
        error: HttpErrorResponse,
        fallback: string,
    ): string {
        if (typeof error.error?.error === 'string') {
            return error.error.error;
        }

        return fallback;
    }
}