import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import {
    Component,
    inject,
    OnInit,
    signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import {
    AvailableSlot,
    InstructorAvailabilityService,
} from '../../../core/student/instructor-availability.service';

import {
    StudentProfileService,
} from '../../../core/student/student-profile.service';

import { LessonRequestService } from '../../../core/lesson-request/lesson-request.service';

@Component({
    selector: 'app-instructor-availability',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
    ],
    templateUrl: './instructor-availability.component.html',
    styleUrl: './instructor-availability.component.scss',
})
export class InstructorAvailabilityComponent implements OnInit {
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly availabilityService =
        inject(InstructorAvailabilityService);
    private readonly lessonRequestService =
        inject(LessonRequestService);
    private readonly studentProfileService =
        inject(StudentProfileService);

    readonly slots = signal<AvailableSlot[]>([]);
    readonly isLoading = signal(false);
    readonly hasSearched = signal(false);
    readonly errorMessage = signal<string | null>(null);
    readonly selectedSlot = signal<AvailableSlot | null>(null);
    readonly isRequesting = signal(false);
    readonly requestSuccess = signal(false);
    readonly canUseOwnVehicle = signal(false);
    readonly usesStudentVehicle = signal(false);
    readonly studentMessage = signal('');

    instructorProfileId = '';
    selectedDate = '';
    readonly minimumDate = this.toDateInputValue(new Date());

    ngOnInit(): void {
        this.instructorProfileId =
            this.route.snapshot.paramMap.get('instructorProfileId') ?? '';

        this.studentProfileService.getProfile().subscribe({
            next: (profile) => {
                this.canUseOwnVehicle.set(
                    profile.ownsVehicle &&
                    profile.hasOwnVehicleForLessons,
                );
            },
        });

        if (!this.instructorProfileId) {
            void this.router.navigate(['/student/instructors']);
            return;
        }

        this.selectedDate = this.minimumDate;
        this.loadSlots();
    }

    loadSlots(): void {
        this.errorMessage.set(null);
        this.slots.set([]);
        this.hasSearched.set(false);
        this.selectedSlot.set(null);
        this.requestSuccess.set(false);
        this.usesStudentVehicle.set(false);
        this.studentMessage.set('');

        if (!this.selectedDate) {
            this.errorMessage.set(
                'Selecione uma data para consultar os horários.',
            );
            return;
        }

        if (this.selectedDate < this.minimumDate) {
            this.errorMessage.set(
                'Selecione uma data a partir de hoje.',
            );
            return;
        }

        this.isLoading.set(true);

        this.availabilityService
            .getAvailableSlots(
                this.instructorProfileId,
                this.selectedDate,
            )
            .subscribe({
                next: (slots) => {
                    this.slots.set(slots);
                    this.hasSearched.set(true);
                    this.isLoading.set(false);
                },

                error: (error: HttpErrorResponse) => {
                    this.slots.set([]);
                    this.hasSearched.set(true);
                    this.isLoading.set(false);

                    if (error.status === 404) {
                        this.errorMessage.set(
                            'O perfil deste instrutor não foi encontrado.',
                        );
                        return;
                    }

                    if (error.status === 400) {
                        this.errorMessage.set(
                            'Este instrutor não está disponível no momento.',
                        );
                        return;
                    }

                    this.errorMessage.set(
                        'Não foi possível consultar os horários. Tente novamente.',
                    );
                },
            });
    }

    goBack(): void {
        void this.router.navigate(['/student/instructors']);
    }

    formatTime(time: string): string {
        return time.slice(0, 5);
    }

    formatSelectedDate(): string {
        if (!this.selectedDate) {
            return '';
        }

        const [year, month, day] =
            this.selectedDate.split('-').map(Number);

        const date = new Date(year, month - 1, day);

        return new Intl.DateTimeFormat('pt-BR', {
            weekday: 'long',
            day: '2-digit',
            month: 'long',
        }).format(date);
    }

    selectSlot(slot: AvailableSlot): void {
        this.selectedSlot.set(slot);
    }

    isSelected(slot: AvailableSlot): boolean {
        const selected = this.selectedSlot();

        return selected?.startTime === slot.startTime &&
            selected?.endTime === slot.endTime;
    }

    requestLesson(): void {
        const slot = this.selectedSlot();

        if (!slot || !this.selectedDate || this.isRequesting()) {
            return;
        }

        this.errorMessage.set(null);
        this.requestSuccess.set(false);
        this.isRequesting.set(true);

        this.lessonRequestService
            .create({
                instructorProfileId: this.instructorProfileId,
                requestedDate: this.selectedDate,
                startTime: slot.startTime,
                endTime: slot.endTime,
                usesStudentVehicle: this.usesStudentVehicle(),
                studentMessage:
                    this.studentMessage().trim() || null,
            })
            .pipe(
                finalize(() => this.isRequesting.set(false)),
            )
            .subscribe({
                next: () => {
                    this.requestSuccess.set(true);
                },

                error: (error: HttpErrorResponse) => {
                    const apiMessage =
                        typeof error.error?.error === 'string'
                            ? error.error.error
                            : null;

                    this.errorMessage.set(
                        apiMessage ??
                        'Não foi possível solicitar a aula. Tente novamente.',
                    );
                },
            });
    }

    goToMyRequests(): void {
        void this.router.navigate(['/student/lesson-requests']);
    }

    private toDateInputValue(date: Date): string {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }
}