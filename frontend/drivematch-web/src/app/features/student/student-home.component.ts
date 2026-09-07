import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-student-home',
  standalone: true,
  template: `
    <main class="student-home">
      <section class="student-home__content">
        <span class="student-home__eyebrow">
          Área do aluno
        </span>

        <h1>Pronto para praticar?</h1>

        <p>
          Encontre instrutores compatíveis com seu perfil
          e suas preferências.
        </p>

        <div class="student-home__actions">
          <button
            type="button"
            class="dm-button dm-button--primary"
            (click)="findInstructors()">
            Encontrar instrutores
          </button>

          <button
            type="button"
            class="dm-button"
            (click)="editProfile()">
            Editar perfil
          </button>
        </div>
      </section>
    </main>
  `,
  styles: `
    :host {
      display: block;
      min-height: 100dvh;
    }

    .student-home {
      display: grid;
      min-height: 100dvh;
      place-items: center;
      padding: 1.5rem;
      background: var(--dm-color-background);
    }

    .student-home__content {
      width: 100%;
      max-width: 42rem;
      padding: 2rem;
      border: 1px solid var(--dm-color-border);
      border-radius: 1.5rem;
      background: #fff;
    }

    .student-home__eyebrow {
      color: var(--dm-color-primary);
      font-size: 0.8rem;
      font-weight: 700;
      letter-spacing: 0.08em;
      text-transform: uppercase;
    }

    h1 {
      margin: 0.5rem 0 0.75rem;
      color: var(--dm-color-text);
      font-size: clamp(2rem, 6vw, 3rem);
      line-height: 1.05;
    }

    p {
      margin: 0;
      color: var(--dm-color-secondary);
      line-height: 1.6;
    }

    .student-home__actions {
      display: flex;
      flex-wrap: wrap;
      gap: 0.75rem;
      margin-top: 2rem;
    }
  `,
})
export class StudentHomeComponent {
  private readonly router = inject(Router);

  findInstructors(): void {
    void this.router.navigate(['/student/instructors']);
  }

  editProfile(): void {
    void this.router.navigate(['/student/profile']);
  }
}