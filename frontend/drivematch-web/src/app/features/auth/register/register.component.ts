import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';

import {
  AuthService,
  UserRole,
} from '../../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    FontAwesomeModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  name = '';
  email = '';
  password = '';
  confirmPassword = '';

  readonly selectedRole = signal<UserRole | null>(null);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);
  readonly showConfirmPassword = signal(false);

  readonly faEye = faEye;
  readonly faEyeSlash = faEyeSlash;

  togglePasswordVisibility(): void {
    this.showPassword.update((value) => !value);
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword.update((value) => !value);
  }

  selectRole(role: UserRole): void {
    this.selectedRole.set(role);
    this.errorMessage.set(null);
  }

  register(): void {
    this.errorMessage.set(null);

    const role = this.selectedRole();

    if (!role) {
      this.errorMessage.set('Escolha se você é aluno ou instrutor.');
      return;
    }

    if (
      !this.name.trim() ||
      !this.email.trim() ||
      !this.password ||
      !this.confirmPassword
    ) {
      this.errorMessage.set('Preencha todos os campos.');
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.errorMessage.set('As senhas não coincidem.');
      return;
    }

    this.isLoading.set(true);

    const email = this.email.trim();
    const password = this.password;

    this.authService
      .register({
        name: this.name.trim(),
        email,
        password,
        role,
      })
      .pipe(
        switchMap(() =>
          this.authService.login({
            email,
            password,
          }),
        ),
      )
      .subscribe({
        next: (response) => {
          this.isLoading.set(false);

          if (response.role === 'Student') {
            void this.router.navigate(['/student']);
            return;
          }

          void this.router.navigate(['/instructor']);
        },
        error: (error: HttpErrorResponse) => {
          this.isLoading.set(false);

          if (error.status === 409) {
            this.errorMessage.set(
              'Já existe uma conta cadastrada com este e-mail.',
            );
            return;
          }

          this.errorMessage.set(
            'Não foi possível criar sua conta. Tente novamente.',
          );
        },
      });
  }
}