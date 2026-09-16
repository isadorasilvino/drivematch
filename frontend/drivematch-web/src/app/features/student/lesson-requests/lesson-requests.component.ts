import { PageLayoutComponent } from '../../../shared/components/page-layout/page-layout.component';
import { DestructiveActionComponent } from '../../../shared/components/destructive-action/destructive-action.component';
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
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faTrashCan } from '@fortawesome/free-solid-svg-icons';

@Component({
    selector: 'app-student-lesson-requests',
    standalone: true,
    imports: [
        PageLayoutComponent,
        FontAwesomeModule,
        CommonModule,
        LessonRequestCardComponent,
        DestructiveActionComponent,
    ],
    templateUrl: './lesson-requests.component.html',
    styleUrl: './lesson-requests.component.scss',
})
export class StudentLessonRequestsComponent implements OnInit {
    protected readonly faTrashCan = faTrashCan;
    private readonly router = inject(Router);
    private readonly lessonRequestService =
        inject(LessonRequestService);

    readonly requests = signal<LessonRequestListItem[]>([]);
    readonly isLoading = signal(true);
    readonly errorMessage = signal<string | null>(null);

    readonly actionRequestId = signal<string | null>(null);
    readonly actionErrorMessage = signal<string | null>(null);

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

    cancelRequest(request: LessonRequestListItem): void {
        const confirmed = window.confirm(
            'Deseja cancelar esta solicitação de aula?',
        );

        if (!confirmed) {
            return;
        }

        this.actionRequestId.set(request.lessonRequestId);
        this.actionErrorMessage.set(null);

        this.lessonRequestService
            .cancel(request.lessonRequestId)
            .pipe(
                finalize(() => {
                    this.actionRequestId.set(null);
                }),
            )
            .subscribe({
                next: (result) => {
                    this.requests.update((requests) =>
                        requests.map((currentRequest) =>
                            currentRequest.lessonRequestId ===
                            result.lessonRequestId
                                ? {
                                    ...currentRequest,
                                    status: result.status,
                                }
                                : currentRequest,
                        ),
                    );
                },

                error: (error: HttpErrorResponse) => {
                    this.actionErrorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível cancelar a solicitação.',
                        ),
                    );
                },
            });
    }

    isProcessing(requestId: string): boolean {
        return this.actionRequestId() === requestId;
    }

    findInstructors(): void {
        void this.router.navigate(['/student/instructors']);
    }

    goHome(): void {
        void this.router.navigate(['/student']);
    }

    private getActionError(
        error: HttpErrorResponse,
        fallback: string,
    ): string {
        const apiError = error.error?.error;

        return typeof apiError === 'string' &&
            apiError.trim().length > 0
            ? apiError
            : fallback;
    }
}