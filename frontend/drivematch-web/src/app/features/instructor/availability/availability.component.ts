import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';


import {
  AvailabilityRequest,
  AvailabilityResponse,
  AvailabilityService,
  AvailabilityDay,
} from '../../../core/instructor/availability.service';

interface DayOption {
  value: AvailabilityDay;
  label: string;
}

@Component({
  selector: 'app-availability',
  imports: [
    CommonModule,
    FormsModule,
  ],
  templateUrl: './availability.component.html',
  styleUrl: './availability.component.scss',
})
export class AvailabilityComponent implements OnInit {
  private readonly availabilityService = inject(AvailabilityService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  readonly days: DayOption[] = [
    { value: 'Monday', label: 'Segunda-feira' },
    { value: 'Tuesday', label: 'Terça-feira' },
    { value: 'Wednesday', label: 'Quarta-feira' },
    { value: 'Thursday', label: 'Quinta-feira' },
    { value: 'Friday', label: 'Sexta-feira' },
    { value: 'Saturday', label: 'Sábado' },
    { value: 'Sunday', label: 'Domingo' },
  ];

  availabilities: AvailabilityResponse[] = [];

  dayOfWeek: AvailabilityDay = 'Monday';
  startTime = '';
  endTime = '';

  editingAvailabilityId: string | null = null;

  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadAvailabilities();
  }

  loadAvailabilities(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.availabilityService
      .getMine()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.changeDetectorRef.markForCheck();
        }),
      )
      .subscribe({
        next: (availabilities) => {
          this.availabilities =
            this.sortAvailabilities(availabilities);
        },
        error: () => {
          this.errorMessage =
            'Não foi possível carregar sua disponibilidade.';
        },
      });
  }

  save(): void {
    this.clearMessages();

    if (!this.startTime || !this.endTime) {
      this.errorMessage = 'Informe o horário inicial e final.';
      return;
    }

    if (this.startTime >= this.endTime) {
      this.errorMessage = 'O horário final deve ser maior que o horário inicial.';
      return;
    }

    const request: AvailabilityRequest = {
      dayOfWeek: this.dayOfWeek,
      startTime: this.normalizeTime(this.startTime),
      endTime: this.normalizeTime(this.endTime),
    };

    this.isSaving = true;

    const operation = this.editingAvailabilityId
      ? this.availabilityService.update(
        this.editingAvailabilityId,
        request,
      )
      : this.availabilityService.create(request);

    operation.subscribe({
      next: () => {
        this.successMessage = this.editingAvailabilityId
          ? 'Horário atualizado com sucesso.'
          : 'Horário adicionado com sucesso.';

        this.resetForm();
        this.loadAvailabilities();
        this.isSaving = false;
        this.changeDetectorRef.markForCheck();
        this.changeDetectorRef.markForCheck();

      },
      error: (error) => {
        this.errorMessage =
          error?.error?.error ??
          'Não foi possível salvar a disponibilidade.';

        this.isSaving = false;
        this.changeDetectorRef.markForCheck();
        this.changeDetectorRef.markForCheck();

      },
    });
  }

  edit(availability: AvailabilityResponse): void {
    this.clearMessages();

    this.editingAvailabilityId = availability.availabilityId;
    this.dayOfWeek = availability.dayOfWeek;
    this.startTime = this.formatTimeForInput(availability.startTime);
    this.endTime = this.formatTimeForInput(availability.endTime);

    window.scrollTo({
      top: 0,
      behavior: 'smooth',
    });
  }

  cancelEdit(): void {
    this.resetForm();
    this.clearMessages();
  }

  toggleStatus(availability: AvailabilityResponse): void {
    this.clearMessages();

    const newStatus = !availability.isActive;

    this.availabilityService
      .changeStatus(
        availability.availabilityId,
        newStatus,
      )
      .subscribe({
        next: () => {
          this.availabilities = this.sortAvailabilities(
            this.availabilities.map((item) =>
              item.availabilityId === availability.availabilityId
                ? {
                  ...item,
                  isActive: newStatus,
                }
                : item,
            ),
          );

          this.successMessage = newStatus
            ? 'Horário ativado.'
            : 'Horário desativado.';

          this.changeDetectorRef.detectChanges();
        },
        error: (error) => {
          this.errorMessage =
            error?.error?.error ??
            'Não foi possível alterar o status do horário.';

          this.changeDetectorRef.detectChanges();
        },
      });
  }

  getDayLabel(dayOfWeek: AvailabilityDay): string {
    return this.days.find((day) => day.value === dayOfWeek)?.label ?? '';
  }

  formatTime(time: string): string {
    return time.slice(0, 5);
  }

  trackByAvailabilityId(
    _index: number,
    availability: AvailabilityResponse,
  ): string {
    return availability.availabilityId;
  }

  private resetForm(): void {
    this.editingAvailabilityId = null;
    this.dayOfWeek = 'Monday';
    this.startTime = '';
    this.endTime = '';
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private normalizeTime(time: string): string {
    return time.length === 5
      ? `${time}:00`
      : time;
  }

  private formatTimeForInput(time: string): string {
    return time.slice(0, 5);
  }

  private sortAvailabilities(
    availabilities: AvailabilityResponse[],
  ): AvailabilityResponse[] {
    const dayOrder: AvailabilityDay[] = [
      'Monday',
      'Tuesday',
      'Wednesday',
      'Thursday',
      'Friday',
      'Saturday',
      'Sunday',
    ];

    return [...availabilities].sort((a, b) => {
      const dayComparison =
        dayOrder.indexOf(a.dayOfWeek) -
        dayOrder.indexOf(b.dayOfWeek);

      if (dayComparison !== 0) {
        return dayComparison;
      }

      return a.startTime.localeCompare(b.startTime);
    });
  }
}