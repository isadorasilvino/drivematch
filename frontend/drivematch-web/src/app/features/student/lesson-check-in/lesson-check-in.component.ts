import { PageLayoutComponent } from '../../../shared/components/page-layout/page-layout.component';
import { HttpErrorResponse } from '@angular/common/http';
import {
    Component,
    inject,
    OnInit,
    signal,
} from '@angular/core';
import {
    ActivatedRoute,
    Router,
} from '@angular/router';
import { finalize } from 'rxjs';

import {
    ConfirmLessonCheckInResult,
    LessonService,
} from '../../../core/lesson/lesson.service';

type CheckInPageState =
    | 'ready'
    | 'processing'
    | 'success'
    | 'invalid';

@Component({
    selector: 'app-lesson-check-in',
    standalone: true,
    imports: [PageLayoutComponent],
    templateUrl: './lesson-check-in.component.html',
    styleUrl: './lesson-check-in.component.scss',
})
export class LessonCheckInComponent implements OnInit {
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly lessonService = inject(LessonService);

    readonly state = signal<CheckInPageState>('invalid');
    readonly errorMessage = signal<string | null>(null);
    readonly result = signal<ConfirmLessonCheckInResult | null>(
        null,
    );

    private lessonId: string | null = null;
    private checkInToken: string | null = null;

    ngOnInit(): void {
        const lessonId =
            this.route.snapshot.queryParamMap
                .get('lessonId')
                ?.trim() ?? '';

        const token =
            this.route.snapshot.queryParamMap
                .get('token')
                ?.trim() ?? '';

        if (!lessonId || !token) {
            this.state.set('invalid');
            this.errorMessage.set(
                'O QR Code é inválido ou está incompleto.',
            );

            return;
        }

        this.lessonId = lessonId;
        this.checkInToken = token;
        this.state.set('ready');
    }

    confirmCheckIn(): void {
        if (
            !this.lessonId ||
            !this.checkInToken ||
            this.state() === 'processing'
        ) {
            return;
        }

        this.state.set('processing');
        this.errorMessage.set(null);

        this.lessonService
            .confirmCheckIn(
                this.lessonId,
                this.checkInToken,
            )
            .pipe(
                finalize(() => {
                    if (this.state() === 'processing') {
                        this.state.set('ready');
                    }
                }),
            )
            .subscribe({
                next: (result) => {
                    this.result.set(result);
                    this.state.set('success');

                    this.lessonId = null;
                    this.checkInToken = null;
                },

                error: (error: HttpErrorResponse) => {
                    this.errorMessage.set(
                        this.getActionError(
                            error,
                            'Não foi possível confirmar o check-in.',
                        ),
                    );
                },
            });
    }

    goToLessons(): void {
        void this.router.navigate(['/student/lessons']);
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