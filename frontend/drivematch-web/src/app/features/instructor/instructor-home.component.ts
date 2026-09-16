import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'app-instructor-home',
    standalone: true,
    template: `
        <main class="instructor-home">
            <section class="dm-card instructor-home__content">
                <span class="instructor-home__eyebrow">
                    Área do instrutor
                </span>

                <h1>Gerencie suas aulas</h1>

                <p>
                    Acompanhe as solicitações dos alunos,
                    organize sua disponibilidade e mantenha
                    seu perfil atualizado.
                </p>

                <div class="instructor-home__actions">
                    <button
                        type="button"
                        class="dm-button dm-button--primary"
                        (click)="viewRequests()">
                        Solicitações recebidas
                    </button>

                    <button
                        type="button"
                        class="dm-button dm-button--secondary"
                        (click)="viewLessons()">
                        Minhas aulas
                    </button>

                    <button
                        type="button"
                        class="dm-button dm-button--secondary"
                        (click)="manageAvailability()">
                        Minha disponibilidade
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

        .instructor-home {
            display: grid;
            min-height: 100dvh;
            place-items: center;
            padding: var(--dm-space-5);
            background: var(--dm-background);
        }

        .instructor-home__content {
            width: 100%;
            max-width: 42rem;
            padding: var(--dm-space-8);
        }

        .instructor-home__eyebrow {
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

        .instructor-home__actions {
            display: flex;
            flex-wrap: wrap;
            gap: var(--dm-space-3);
            margin-top: var(--dm-space-6);
        }
    `,
})
export class InstructorHomeComponent {
    private readonly router = inject(Router);

    viewRequests(): void {
        void this.router.navigate(['/instructor/lesson-requests']);
    }

    viewLessons(): void {
        void this.router.navigate(['/instructor/lessons']);
    }

    manageAvailability(): void {
        void this.router.navigate(['/instructor/availability']);
    }

    editProfile(): void {
        void this.router.navigate(['/instructor/profile']);
    }
}