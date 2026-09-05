import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import {
  InstructorProfileRequest,
  InstructorProfileService,
  InstructorProfileStatus,
} from '../../../core/instructor/instructor-profile.service';

@Component({
  selector: 'app-instructor-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  private readonly instructorProfileService = inject(InstructorProfileService);
  private readonly router = inject(Router);

  description = '';
  experienceYears: number | null = null;
  city = '';
  state = '';
  pricePerLesson: number | null = null;

  acceptsBeginners: boolean | null = null;
  acceptsExperiencedStudents: boolean | null = null;
  acceptsStudentVehicle: boolean | null = null;

  readonly isLoading = signal(true);
  readonly isSaving = signal(false);
  readonly isEditing = signal(false);
  readonly errorMessage = signal('');
  readonly profileStatus = signal<InstructorProfileStatus>('Draft');
  readonly isChangingStatus = signal(false);
  readonly statusMessage = signal('');

  readonly states = [
    { value: 'AC', label: 'Acre' },
    { value: 'AL', label: 'Alagoas' },
    { value: 'AP', label: 'Amapá' },
    { value: 'AM', label: 'Amazonas' },
    { value: 'BA', label: 'Bahia' },
    { value: 'CE', label: 'Ceará' },
    { value: 'DF', label: 'Distrito Federal' },
    { value: 'ES', label: 'Espírito Santo' },
    { value: 'GO', label: 'Goiás' },
    { value: 'MA', label: 'Maranhão' },
    { value: 'MT', label: 'Mato Grosso' },
    { value: 'MS', label: 'Mato Grosso do Sul' },
    { value: 'MG', label: 'Minas Gerais' },
    { value: 'PA', label: 'Pará' },
    { value: 'PB', label: 'Paraíba' },
    { value: 'PR', label: 'Paraná' },
    { value: 'PE', label: 'Pernambuco' },
    { value: 'PI', label: 'Piauí' },
    { value: 'RJ', label: 'Rio de Janeiro' },
    { value: 'RN', label: 'Rio Grande do Norte' },
    { value: 'RS', label: 'Rio Grande do Sul' },
    { value: 'RO', label: 'Rondônia' },
    { value: 'RR', label: 'Roraima' },
    { value: 'SC', label: 'Santa Catarina' },
    { value: 'SP', label: 'São Paulo' },
    { value: 'SE', label: 'Sergipe' },
    { value: 'TO', label: 'Tocantins' },
  ];

  ngOnInit(): void {
    this.loadProfile();
  }

  selectAcceptsBeginners(value: boolean): void {
    this.acceptsBeginners = value;
  }

  selectAcceptsExperiencedStudents(value: boolean): void {
    this.acceptsExperiencedStudents = value;
  }

  selectAcceptsStudentVehicle(value: boolean): void {
    this.acceptsStudentVehicle = value;
  }

  save(): void {
    this.errorMessage.set('');

    if (
      !this.description.trim() ||
      this.experienceYears === null ||
      !this.city.trim() ||
      !this.state ||
      this.pricePerLesson === null ||
      this.acceptsBeginners === null ||
      this.acceptsExperiencedStudents === null ||
      this.acceptsStudentVehicle === null
    ) {
      this.errorMessage.set(
        'Preencha todas as informações para continuar.',
      );
      return;
    }

    if (this.experienceYears < 0) {
      this.errorMessage.set(
        'Os anos de experiência não podem ser negativos.',
      );
      return;
    }

    if (this.pricePerLesson <= 0) {
      this.errorMessage.set(
        'Informe um valor válido para a aula.',
      );
      return;
    }

    const request: InstructorProfileRequest = {
      description: this.description.trim(),
      experienceYears: this.experienceYears,
      city: this.city.trim(),
      state: this.state,
      pricePerLesson: this.pricePerLesson,
      acceptsBeginners: this.acceptsBeginners,
      acceptsExperiencedStudents: this.acceptsExperiencedStudents,
      acceptsStudentVehicle: this.acceptsStudentVehicle,
    };

    this.isSaving.set(true);

    const operation = this.isEditing()
      ? this.instructorProfileService.updateProfile(request)
      : this.instructorProfileService.createProfile(request);

    operation.subscribe({
      next: () => {
        this.isSaving.set(false);
        void this.router.navigate(['/instructor']);
      },
      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);

        this.errorMessage.set(
          error.error?.error ??
          'Não foi possível salvar seu perfil. Tente novamente.',
        );
      },
    });
  }

  changeProfileStatus(isActive: boolean): void {
    this.errorMessage.set('');
    this.statusMessage.set('');
    this.isChangingStatus.set(true);

    this.instructorProfileService.changeStatus(isActive).subscribe({
      next: (result) => {
        this.profileStatus.set(result.status);
        this.isChangingStatus.set(false);

        this.statusMessage.set(
          isActive
            ? 'Seu perfil está ativo e pode ser encontrado pelos alunos.'
            : 'Seu perfil foi desativado e não aparecerá para novos alunos.',
        );
      },

      error: (error: HttpErrorResponse) => {
        this.isChangingStatus.set(false);

        this.errorMessage.set(
          error.error?.error ??
          'Não foi possível alterar o status do seu perfil. Tente novamente.',
        );
      },
    });
  }

  goToAvailability(): void {
    void this.router.navigate(['/instructor/availability']);
  }

  private loadProfile(): void {
    this.instructorProfileService.getProfile().subscribe({
      next: (profile) => {
        this.description = profile.description;
        this.experienceYears = profile.experienceYears;
        this.city = profile.city;
        this.state = profile.state;
        this.pricePerLesson = profile.pricePerLesson;
        this.acceptsBeginners = profile.acceptsBeginners;
        this.acceptsExperiencedStudents =
          profile.acceptsExperiencedStudents;
        this.acceptsStudentVehicle = profile.acceptsStudentVehicle;
        this.profileStatus.set(profile.status);

        this.isEditing.set(true);
        this.isLoading.set(false);
      },

      error: (error: HttpErrorResponse) => {
        if (error.status === 404) {
          this.isEditing.set(false);
          this.isLoading.set(false);
          return;
        }

        this.errorMessage.set(
          'Não foi possível carregar seu perfil. Tente novamente.',
        );
        this.isLoading.set(false);
      },
    });
  }
}