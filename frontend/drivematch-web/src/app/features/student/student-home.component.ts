import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'app-student-home',
    standalone: true,
    template: `
        <main class="student-home">
            <section class="dm-card student-home__content">
                <span class="student-home__eyebrow">
                    Área do aluno
                </span>

                <h1>Pronto para praticar?</h1>

                <p>
                    Encontre instrutores compatíveis com seu perfil
                    e acompanhe suas solicitações de aula.
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
                        class="dm-button dm-button--secondary"
                        (click)="viewRequests()">
                        Minhas solicitações
                    </button>

                    <button
                        type="button"
                        class="dm-button dm-button--ghost"
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
            padding: var(--dm-space-5);
            background: var(--dm-background);
        }

        .student-home__content {
            width: 100%;
            max-width: 42rem;
            padding: var(--dm-space-8);
        }

        .student-home__eyebrow {
            color: var(--dm-primary);
            font-size: 0.8rem;
            font-weight: 700;
            letter-spacing: 0.08em;
            text-transform: uppercase;
        }

        h1 {
            margin: var(--dm-space-3) 0 var(--dm-space-3);
            color: var(--dm-text);
            font-size: clamp(2rem, 6vw, 3rem);
            line-height: 1.05;
        }

        p {
            margin: 0;
            color: var(--dm-text-secondary);
            line-height: 1.6;
        }

        .student-home__actions {
            display: flex;
            flex-wrap: wrap;
            gap: var(--dm-space-3);
            margin-top: var(--dm-space-6);
        }
    `,
})
export class StudentHomeComponent {
    private readonly router = inject(Router);

    findInstructors(): void {
        void this.router.navigate(['/student/instructors']);
    }

    viewRequests(): void {
        void this.router.navigate(['/student/lesson-requests']);
    }

    editProfile(): void {
        void this.router.navigate(['/student/profile']);
    }
}