import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';


import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  email = '';
  password = '';

  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  login(): void {
    this.errorMessage.set(null);

    if (!this.email || !this.password) {
      this.errorMessage.set('Informe e-mail e senha.');
      return;
    }

    this.isLoading.set(true);

    this.authService.login({
      email: this.email,
      password: this.password,
    }).subscribe({
      next: (response) => {
        this.isLoading.set(false);

        if (response.role === 'Student') {
          void this.router.navigate(['/student']);
          return;
        }

        void this.router.navigate(['/instructor']);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('E-mail ou senha inválidos.');
      },
    });
  }
}