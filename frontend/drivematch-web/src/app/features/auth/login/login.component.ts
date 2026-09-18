import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import {
  AfterViewInit,
  Component,
  ElementRef,
  ViewChild,
  inject,
  signal,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';

import { AuthService } from '../../../core/auth/auth.service';
import { AuthStorageService } from '../../../core/auth/auth-storage.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    FontAwesomeModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements AfterViewInit {
  private readonly authService = inject(AuthService);
  private readonly authStorage = inject(AuthStorageService);
  private readonly router = inject(Router);

  @ViewChild('emailInput')
  private emailInput?: ElementRef<HTMLInputElement>;

  email = '';
  password = '';

  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);
  readonly hasRememberedEmail = signal(false);

  readonly faEye = faEye;
  readonly faEyeSlash = faEyeSlash;

  constructor() {
    const session = this.authStorage.getSession();

    if (session) {
      void this.router.navigate([
        session.role === 'Student'
          ? '/student'
          : '/instructor',
      ]);

      return;
    }

    const lastEmail = this.authStorage.getLastEmail();

    if (lastEmail) {
      this.email = lastEmail;
      this.hasRememberedEmail.set(true);
    }
  }

  ngAfterViewInit(): void {
    if (!this.hasRememberedEmail()) {
      this.emailInput?.nativeElement.focus();
    }
  }

  login(): void {
    this.errorMessage.set(null);

    const normalizedEmail = this.email.trim().toLowerCase();

    if (!normalizedEmail || !this.password) {
      this.errorMessage.set('Informe e-mail e senha.');
      return;
    }

    this.isLoading.set(true);

    this.authService.login({
      email: normalizedEmail,
      password: this.password,
    }).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        this.password = '';

        if (response.role === 'Student') {
          void this.router.navigate(['/student']);
          return;
        }

        void this.router.navigate(['/instructor']);
      },

      error: (error: HttpErrorResponse) => {
        this.isLoading.set(false);
        this.password = '';

        if (error.status === 401) {
          this.errorMessage.set('E-mail ou senha inválidos.');
          return;
        }

        if (error.status === 403) {
          this.errorMessage.set('Esta conta está inativa.');
          return;
        }

        this.errorMessage.set(
          'Não foi possível conectar ao DriveMatch. Tente novamente.',
        );
      },
    });
  }

  useAnotherAccount(): void {
    this.authStorage.clearLastEmail();

    this.email = '';
    this.password = '';

    this.errorMessage.set(null);
    this.hasRememberedEmail.set(false);

    setTimeout(() => {
      this.emailInput?.nativeElement.focus();
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword.update((value) => !value);
  }
}