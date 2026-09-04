import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import {
  ExperienceLevel,
  StudentProfileRequest,
  StudentProfileService,
} from '../../../core/student/student-profile.service';

interface StateOption {
  code: string;
  name: string;
}

@Component({
  selector: 'app-student-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  private readonly studentProfileService =
    inject(StudentProfileService);

  private readonly router = inject(Router);

  city = '';
  state = '';
  experienceLevel: ExperienceLevel | '' = '';
  ownsVehicle: boolean | null = null;
  hasOwnVehicleForLessons: boolean | null = null;

  readonly isLoading = signal(true);
  readonly isSaving = signal(false);
  readonly isEditing = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly states: StateOption[] = [
    { code: 'AC', name: 'Acre' },
    { code: 'AL', name: 'Alagoas' },
    { code: 'AP', name: 'Amapá' },
    { code: 'AM', name: 'Amazonas' },
    { code: 'BA', name: 'Bahia' },
    { code: 'CE', name: 'Ceará' },
    { code: 'DF', name: 'Distrito Federal' },
    { code: 'ES', name: 'Espírito Santo' },
    { code: 'GO', name: 'Goiás' },
    { code: 'MA', name: 'Maranhão' },
    { code: 'MT', name: 'Mato Grosso' },
    { code: 'MS', name: 'Mato Grosso do Sul' },
    { code: 'MG', name: 'Minas Gerais' },
    { code: 'PA', name: 'Pará' },
    { code: 'PB', name: 'Paraíba' },
    { code: 'PR', name: 'Paraná' },
    { code: 'PE', name: 'Pernambuco' },
    { code: 'PI', name: 'Piauí' },
    { code: 'RJ', name: 'Rio de Janeiro' },
    { code: 'RN', name: 'Rio Grande do Norte' },
    { code: 'RS', name: 'Rio Grande do Sul' },
    { code: 'RO', name: 'Rondônia' },
    { code: 'RR', name: 'Roraima' },
    { code: 'SC', name: 'Santa Catarina' },
    { code: 'SP', name: 'São Paulo' },
    { code: 'SE', name: 'Sergipe' },
    { code: 'TO', name: 'Tocantins' },
  ];

  ngOnInit(): void {
    this.loadProfile();
  }

  selectExperienceLevel(
    experienceLevel: ExperienceLevel,
  ): void {
    this.experienceLevel = experienceLevel;
    this.errorMessage.set(null);
  }

  selectOwnsVehicle(value: boolean): void {
    this.ownsVehicle = value;

    if (!value) {
      this.hasOwnVehicleForLessons = false;
    }

    this.errorMessage.set(null);
  }

  selectVehicleForLessons(value: boolean): void {
    if (this.ownsVehicle !== true) {
      return;
    }

    this.hasOwnVehicleForLessons = value;
    this.errorMessage.set(null);
  }

  save(): void {
    this.errorMessage.set(null);

    if (
      !this.city.trim() ||
      !this.state ||
      !this.experienceLevel ||
      this.ownsVehicle === null ||
      this.hasOwnVehicleForLessons === null
    ) {
      this.errorMessage.set(
        'Preencha todas as informações para continuar.',
      );
      return;
    }

    const request: StudentProfileRequest = {
      city: this.city.trim(),
      state: this.state,
      experienceLevel: this.experienceLevel,
      ownsVehicle: this.ownsVehicle,
      hasOwnVehicleForLessons:
        this.hasOwnVehicleForLessons,
    };

    this.isSaving.set(true);

    const operation = this.isEditing()
      ? this.studentProfileService.updateProfile(request)
      : this.studentProfileService.createProfile(request);

    operation.subscribe({
      next: () => {
        this.isSaving.set(false);
        void this.router.navigate(['/student']);
      },

      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);

        if (error.status === 409) {
          this.errorMessage.set(
            'Seu perfil já existe. Atualize a página e tente novamente.',
          );
          return;
        }

        this.errorMessage.set(
          'Não foi possível salvar seu perfil. Tente novamente.',
        );
      },
    });
  }

  private loadProfile(): void {
    this.studentProfileService.getProfile().subscribe({
      next: (profile) => {
        this.city = profile.city;
        this.state = profile.state;
        this.experienceLevel = profile.experienceLevel;
        this.ownsVehicle = profile.ownsVehicle;
        this.hasOwnVehicleForLessons =
          profile.hasOwnVehicleForLessons;

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
          'Não foi possível carregar seu perfil.',
        );

        this.isLoading.set(false);
      },
    });
  }
}