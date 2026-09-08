import { CommonModule } from '@angular/common';
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

    readonly slots = signal<AvailableSlot[]>([]);
    readonly isLoading = signal(false);
    readonly hasSearched = signal(false);
    readonly errorMessage = signal<string | null>(null);
    readonly selectedSlot = signal<AvailableSlot | null>(null);


    instructorProfileId = '';
    selectedDate = '';
    readonly minimumDate = this.toDateInputValue(new Date());

    ngOnInit(): void {
        this.instructorProfileId =
            this.route.snapshot.paramMap.get('instructorProfileId') ?? '';

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

    private toDateInputValue(date: Date): string {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }
}