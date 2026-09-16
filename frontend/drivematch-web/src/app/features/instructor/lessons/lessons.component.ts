import { faUserXmark } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { DestructiveActionComponent } from '../../../shared/components/destructive-action/destructive-action.component';
import { PageLayoutComponent } from '../../../shared/components/page-layout/page-layout.component';
import { HttpErrorResponse } from '@angular/common/http';
import {
    Component,
    inject,
    OnInit,
    signal,
} from '@angular/core';
import { Router } from '@angular/router';
import QRCode from 'qrcode';
import { finalize } from 'rxjs';

import {
    LessonListItem,
    LessonService,
    StartLessonCheckInResult,
} from '../../../core/lesson/lesson.service';
import {
    LessonCardComponent,
} from '../../../shared/components/lesson-card/lesson-card.component';

interface ActiveCheckIn {
    lessonId: string;
    token: string;
    expiresAt: string;
    qrCodeDataUrl: string;
}

@Component({
    selector: 'app-instructor-lessons',
    standalone: true,
    imports: [
        LessonCardComponent,
        DestructiveActionComponent,
        FontAwesomeModule,
        PageLayoutComponent,
    ],
    templateUrl: './lessons.component.html',
    styleUrl: './lessons.component.scss',
})
export class InstructorLessonsComponent implements OnInit {
    protected readonly faUserXmark = faUserXmark;
    private readonly router = inject(Router);
    private readonly lessonService = inject(LessonService);

    readonly lessons = signal<LessonListItem[]>([]);
    readonly isLoading = signal(true);
    readonly errorMessage = signal<string | null>(null);

    readonly actionLessonId = signal<string | null>(null);
    readonly actionErrorMessage = signal<string | null>(null);
    readonly activeCheckIn = signal<ActiveCheckIn | null>(null);

    ngOnInit(): void {
        this.loadLessons();
    }

    loadLessons(): void {
        this.isLoading.set(true);
        this.errorMessage.set(null);

        this.lessonService
            .getInstructorLessons()
            .subscribe({
                next: (lessons) => {
                    this.lessons.set(lessons);
                    this.isLoading.set(false);
                },

                error: (_error: HttpErrorResponse) => {
                    this.lessons.set([]);
                    this.isLoading.set(false);
                    this.errorMessage.set(
                        'Não foi possível carregar suas aulas.',
                    );
                },
            });
    }

    startCheckIn(lesson: LessonListItem): void {
        this.actionLessonId.set(lesson.lessonId);
        this.actionErrorMessage.set(null);

        this.lessonService
            .startCheckIn(lesson.lessonId)
            .pipe(
                finalize(() => {
                    this.actionLessonId.set(null);
                }),
            )
            .subscribe({
                next: (result) => {
                    void this.applyStartedCheckIn(result);
                },

                error: (error: HttpErrorResponse) => {
                    this.actionErrorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível iniciar o check-in.',
                        ),
                    );
                },
            });
    }

    completeLesson(lesson: LessonListItem): void {
        this.actionLessonId.set(lesson.lessonId);
        this.actionErrorMessage.set(null);

        this.lessonService
            .completeLesson(lesson.lessonId)
            .pipe(
                finalize(() => {
                    this.actionLessonId.set(null);
                }),
            )
            .subscribe({
                next: (result) => {
                    this.updateLessonStatus(
                        lesson.lessonId,
                        result.status,
                    );

                    if (
                        this.activeCheckIn()?.lessonId ===
                        lesson.lessonId
                    ) {
                        this.activeCheckIn.set(null);
                    }
                },

                error: (error: HttpErrorResponse) => {
                    this.actionErrorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível concluir a aula.',
                        ),
                    );
                },
            });
    }

    cancelLesson(lesson: LessonListItem): void {
        const confirmed = window.confirm(
            'Deseja cancelar esta aula?',
        );

        if (!confirmed) {
            return;
        }

        this.runScheduledLessonAction(
            lesson,
            () => this.lessonService.cancelLesson(lesson.lessonId),
            'Não foi possível cancelar a aula.',
        );
    }

    markAsNotAttended(lesson: LessonListItem): void {
        const confirmed = window.confirm(
            'Confirmar que o aluno não compareceu à aula?',
        );

        if (!confirmed) {
            return;
        }

        this.runScheduledLessonAction(
            lesson,
            () => this.lessonService.markAsNotAttended(lesson.lessonId),
            'Não foi possível marcar a aula como não comparecida.',
        );
    }
    isProcessing(lessonId: string): boolean {
        return this.actionLessonId() === lessonId;
    }

    checkInFor(
        lessonId: string,
    ): ActiveCheckIn | null {
        const checkIn = this.activeCheckIn();

        return checkIn?.lessonId === lessonId
            ? checkIn
            : null;
    }

    async copyCheckInToken(token: string): Promise<void> {
        try {
            await navigator.clipboard.writeText(token);
        } catch {
            this.actionErrorMessage.set(
                'Não foi possível copiar o código automaticamente.',
            );
        }
    }

    formatExpiration(value: string): string {
        return new Intl.DateTimeFormat('pt-BR', {
            hour: '2-digit',
            minute: '2-digit',
        }).format(new Date(value));
    }

    goHome(): void {
        void this.router.navigate(['/instructor']);
    }

    private async applyStartedCheckIn(
        result: StartLessonCheckInResult,
    ): Promise<void> {
        this.updateLessonStatus(
            result.lessonId,
            result.status,
        );

        try {
            const checkInUrl = this.buildCheckInUrl(
                result.lessonId,
                result.checkInToken,
            );

            const qrCodeDataUrl = await QRCode.toDataURL(
                checkInUrl,
                {
                    width: 280,
                    margin: 2,
                    errorCorrectionLevel: 'M',
                },
            );

            this.activeCheckIn.set({
                lessonId: result.lessonId,
                token: result.checkInToken,
                expiresAt: result.checkInTokenExpiresAt,
                qrCodeDataUrl,
            });
        } catch {
            this.activeCheckIn.set({
                lessonId: result.lessonId,
                token: result.checkInToken,
                expiresAt: result.checkInTokenExpiresAt,
                qrCodeDataUrl: '',
            });

            this.actionErrorMessage.set(
                'O check-in foi iniciado, mas não foi possível gerar o QR Code. Use o código manual.',
            );
        }
    }

    private buildCheckInUrl(
        lessonId: string,
        token: string,
    ): string {
        const url = new URL(
            '/student/lessons/check-in',
            window.location.origin,
        );

        url.searchParams.set('lessonId', lessonId);
        url.searchParams.set('token', token);

        return url.toString();
    }

    private updateLessonStatus(
        lessonId: string,
        status: LessonListItem['status'],
    ): void {
        this.lessons.update((lessons) =>
            lessons.map((lesson) =>
                lesson.lessonId === lessonId
                    ? {
                        ...lesson,
                        status,
                    }
                    : lesson,
            ),
        );
    }

    private runScheduledLessonAction(
        lesson: LessonListItem,
        action: () => ReturnType<LessonService['cancelLesson']>,
        fallbackError: string,
    ): void {
        this.actionLessonId.set(lesson.lessonId);
        this.actionErrorMessage.set(null);

        action()
            .pipe(
                finalize(() => {
                    this.actionLessonId.set(null);
                }),
            )
            .subscribe({
                next: (result) => {
                    this.updateLessonStatus(
                        lesson.lessonId,
                        result.status,
                    );
                },

                error: (error: HttpErrorResponse) => {
                    this.actionErrorMessage.set(
                        this.getActionError(
                            error,
                            fallbackError,
                        ),
                    );
                },
            });
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