import { DestructiveActionComponent } from '../../../shared/components/destructive-action/destructive-action.component';
import { HttpErrorResponse } from '@angular/common/http';
import {
    Component,
    inject,
    OnInit,
    signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import {
    LessonListItem,
    LessonService,
} from '../../../core/lesson/lesson.service';
import {
    LessonCardComponent,
} from '../../../shared/components/lesson-card/lesson-card.component';

@Component({
    selector: 'app-student-lessons',
    standalone: true,
    imports: [
        FormsModule,
        LessonCardComponent,
        DestructiveActionComponent,
    ],
    templateUrl: './lessons.component.html',
    styleUrl: './lessons.component.scss',
})
export class StudentLessonsComponent implements OnInit {
    private readonly router = inject(Router);
    private readonly lessonService = inject(LessonService);

    readonly lessons = signal<LessonListItem[]>([]);
    readonly isLoading = signal(true);
    readonly errorMessage = signal<string | null>(null);

    readonly actionLessonId = signal<string | null>(null);
    readonly actionErrorMessage = signal<string | null>(null);

    private readonly checkInTokens = new Map<string, string>();

    ngOnInit(): void {
        this.loadLessons();
    }

    loadLessons(): void {
        this.isLoading.set(true);
        this.errorMessage.set(null);

        this.lessonService
            .getStudentLessons()
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

    checkInToken(lessonId: string): string {
        return this.checkInTokens.get(lessonId) ?? '';
    }

    updateCheckInToken(
        lessonId: string,
        value: string,
    ): void {
        this.checkInTokens.set(
            lessonId,
            value.trim(),
        );
    }

    confirmCheckIn(lesson: LessonListItem): void {
        const token = this.checkInToken(lesson.lessonId).trim();

        if (!token) {
            this.actionErrorMessage.set(
                'Informe o código de check-in fornecido pelo instrutor.',
            );

            return;
        }

        this.actionLessonId.set(lesson.lessonId);
        this.actionErrorMessage.set(null);

        this.lessonService
            .confirmCheckIn(
                lesson.lessonId,
                token,
            )
            .pipe(
                finalize(() => {
                    this.actionLessonId.set(null);
                }),
            )
            .subscribe({
                next: (result) => {
                    this.lessons.update((lessons) =>
                        lessons.map((currentLesson) =>
                            currentLesson.lessonId === result.lessonId
                                ? {
                                    ...currentLesson,
                                    status: result.status,
                                    checkInAt: result.checkInAt,
                                    startedAt: result.startedAt,
                                }
                                : currentLesson,
                        ),
                    );

                    this.checkInTokens.delete(lesson.lessonId);
                },

                error: (error: HttpErrorResponse) => {
                    this.actionErrorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível confirmar o check-in.',
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

        this.actionLessonId.set(lesson.lessonId);
        this.actionErrorMessage.set(null);

        this.lessonService
            .cancelLesson(lesson.lessonId)
            .pipe(
                finalize(() => {
                    this.actionLessonId.set(null);
                }),
            )
            .subscribe({
                next: (result) => {
                    this.lessons.update((lessons) =>
                        lessons.map((currentLesson) =>
                            currentLesson.lessonId === result.lessonId
                                ? {
                                    ...currentLesson,
                                    status: result.status,
                                    cancelledAt:
                                        result.status === 'Cancelled'
                                            ? new Date().toISOString()
                                            : currentLesson.cancelledAt,
                                }
                                : currentLesson,
                        ),
                    );
                },

                error: (error: HttpErrorResponse) => {
                    this.actionErrorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível cancelar a aula.',
                        ),
                    );
                },
            });
    }
    isProcessing(lessonId: string): boolean {
        return this.actionLessonId() === lessonId;
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