import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  ElementRef,
  inject,
  OnInit,
  signal,
  viewChild,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChangePasswordComponent } from '../../../shared/components/change-password/change-password.component';
import { AuthStorageService } from '../../../core/auth/auth-storage.service';
import { forkJoin } from 'rxjs';

import {
  AccountService,
  MyAccountResponse,
} from '../../../core/account/account.service';

import {
  ExperienceLevel,
  StudentProfileRequest,
  StudentProfileService,
} from '../../../core/student/student-profile.service';

import {
  SectionComponent,
} from '../../../shared/components/section/section.component';

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
    SectionComponent,
    ChangePasswordComponent,
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  private readonly studentProfileService =
    inject(StudentProfileService);

  private readonly accountService =
    inject(AccountService);

  private readonly authStorage =
    inject(AuthStorageService);

  private readonly successMessageElement =
    viewChild<ElementRef<HTMLElement>>('successMessageElement');

  name = '';
  email = '';
  city = '';
  state = '';
  experienceLevel: ExperienceLevel | '' = '';
  ownsVehicle: boolean | null = null;
  hasOwnVehicleForLessons: boolean | null = null;

  readonly isLoading = signal(true);
  readonly isSaving = signal(false);
  readonly isEditing = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal('');

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
    this.loadAccount();
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
    this.successMessage.set('');

    const normalizedName = this.name.trim();
    const normalizedEmail = this.email.trim();

    if (!normalizedName || !normalizedEmail) {
      this.errorMessage.set(
        'Preencha seu nome e e-mail para continuar.',
      );
      return;
    }

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

    const profileRequest: StudentProfileRequest = {
      city: this.city.trim(),
      state: this.state,
      experienceLevel: this.experienceLevel,
      ownsVehicle: this.ownsVehicle,
      hasOwnVehicleForLessons:
        this.hasOwnVehicleForLessons,
    };

    const accountRequest = {
      name: normalizedName,
      email: normalizedEmail,
    };

    const profileOperation = this.isEditing()
      ? this.studentProfileService.updateProfile(
        profileRequest,
      )
      : this.studentProfileService.createProfile(
        profileRequest,
      );

    this.isSaving.set(true);

    forkJoin({
      account: this.accountService.updateMyAccount(
        accountRequest,
      ),
      profile: profileOperation,
    }).subscribe({
      next: ({ account }) => {
        this.name = account.name;
        this.email = account.email;

        this.authStorage.updateSessionAccount({
          name: account.name,
          email: account.email,
        });

        this.isEditing.set(true);
        this.isSaving.set(false);

        this.successMessage.set(
          'Alterações salvas com sucesso.',
        );

        setTimeout(() => {
          this.successMessageElement()
            ?.nativeElement
            .scrollIntoView({
              behavior: 'smooth',
              block: 'start',
            });
        });
      },

      error: (error: HttpErrorResponse) => {
        this.isSaving.set(false);

        if (error.status === 409) {
          this.errorMessage.set(
            error.error?.error ??
            'Este e-mail já está sendo utilizado por outra conta.',
          );
          return;
        }

        if (error.status === 401) {
          this.errorMessage.set(
            'Sua sessão expirou. Entre novamente para continuar.',
          );
          return;
        }

        this.errorMessage.set(
          error.error?.error ??
          'Não foi possível salvar suas alterações. Tente novamente.',
        );
      },
    });
  }

  private loadProfile(): void {
    this.studentProfileService.getProfile().subscribe({
      next: (profile) => {
        this.city = profile.city;
        this.state = profile.state;
        this.experienceLevel =
          profile.experienceLevel;
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

  private loadAccount(): void {
    this.accountService.getMyAccount().subscribe({
      next: (account: MyAccountResponse) => {
        this.name = account.name;
        this.email = account.email;
      },

      error: () => {
        this.errorMessage.set(
          'Não foi possível carregar os dados da sua conta.',
        );
      },
    });
  }
}