import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import {
  InstructorSearchResult,
  InstructorSearchService,
} from '../../../core/student/instructor-search.service';

import {
  ExperienceLevel,
  StudentProfileService,
} from '../../../core/student/student-profile.service';

interface StateOption {
  code: string;
  name: string;
}

@Component({
  selector: 'app-instructor-search',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
  ],
  templateUrl: './instructor-search.component.html',
  styleUrl: './instructor-search.component.scss',
})
export class InstructorSearchComponent implements OnInit {
  private readonly instructorSearchService =
    inject(InstructorSearchService);

  private readonly studentProfileService =
    inject(StudentProfileService);

  private readonly router = inject(Router);

  city = '';
  state = '';
  experienceLevel: ExperienceLevel = 'Beginner';
  usesStudentVehicle = false;
  maxPricePerLesson: number | null = null;

  readonly instructors = signal<InstructorSearchResult[]>([]);
  readonly isLoadingProfile = signal(true);
  readonly isSearching = signal(false);
  readonly hasSearched = signal(false);
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

  search(): void {
    this.errorMessage.set(null);

    if (!this.city.trim() || !this.state) {
      this.errorMessage.set(
        'Informe a cidade e o estado para buscar instrutores.',
      );
      return;
    }

    if (
      this.maxPricePerLesson !== null &&
      this.maxPricePerLesson <= 0
    ) {
      this.errorMessage.set(
        'O valor máximo da aula deve ser maior que zero.',
      );
      return;
    }

    this.isSearching.set(true);

    this.instructorSearchService.search({
      city: this.city,
      state: this.state,
      experienceLevel: this.experienceLevel,
      usesStudentVehicle: this.usesStudentVehicle,
      maxPricePerLesson: this.maxPricePerLesson,
    }).subscribe({
      next: (instructors) => {
        this.instructors.set(instructors);
        this.hasSearched.set(true);
        this.isSearching.set(false);
      },

      error: () => {
        this.instructors.set([]);
        this.hasSearched.set(true);
        this.isSearching.set(false);

        this.errorMessage.set(
          'Não foi possível buscar instrutores. Tente novamente.',
        );
      },
    });
  }

  editProfile(): void {
    void this.router.navigate(['/student/profile']);
  }

  goBack(): void {
    void this.router.navigate(['/student']);
  }

  experienceLabel(
    instructor: InstructorSearchResult,
  ): string {
    if (
      instructor.acceptsBeginners &&
      instructor.acceptsExperiencedStudents
    ) {
      return 'Todos os níveis';
    }

    if (instructor.acceptsBeginners) {
      return 'Iniciantes';
    }

    return 'Experientes';
  }

  formatPrice(
    value: number,
    currency: string,
  ): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency,
    }).format(value);
  }

  viewAvailability(
    instructor: InstructorSearchResult,
  ): void {
    void this.router.navigate([
      '/student/instructors',
      instructor.instructorProfileId,
      'availability',
    ]);
  }

  private loadProfile(): void {
    this.studentProfileService.getProfile().subscribe({
      next: (profile) => {
        this.city = profile.city;
        this.state = profile.state;
        this.experienceLevel = profile.experienceLevel;
        this.usesStudentVehicle =
          profile.hasOwnVehicleForLessons;

        this.isLoadingProfile.set(false);

        this.search();
      },

      error: (error: HttpErrorResponse) => {
        this.isLoadingProfile.set(false);

        if (error.status === 404) {
          void this.router.navigate(['/student/profile']);
          return;
        }

        this.errorMessage.set(
          'Não foi possível carregar seu perfil.',
        );
      },
    });
  }
}