import { Component } from '@angular/core';

@Component({
    selector: 'app-student-home',
    standalone: true,
    template: `
        <main class="student-home">
            <div class="student-home__container">
                <header class="student-home__header">
                    <span class="student-home__eyebrow">
                        Área do aluno
                    </span>

                    <h1>Pronto para praticar?</h1>

                    <p>
                        Acompanhe suas próximas aulas e encontre
                        instrutores quando precisar.
                    </p>
                </header>
            </div>
        </main>
    `,
    styles: `
        :host {
            display: block;
            min-height: 100dvh;
            background: var(--dm-background);
        }

        .student-home {
            padding: var(--dm-space-8) var(--dm-space-5);
        }

        .student-home__container {
            width: 100%;
            max-width: var(--dm-content-max-width);
            margin: 0 auto;
        }

        .student-home__header {
            max-width: 42rem;
        }

        .student-home__eyebrow {
            color: var(--dm-primary);
            font-size: 0.8rem;
            font-weight: 700;
            letter-spacing: 0.08em;
            text-transform: uppercase;
        }

        h1 {
            margin:
                var(--dm-space-3)
                0
                var(--dm-space-3);
            color: var(--dm-text);
            font-size: clamp(2rem, 5vw, 3rem);
            line-height: 1.05;
        }

        p {
            margin: 0;
            color: var(--dm-text-secondary);
            line-height: 1.6;
        }

        @media (max-width: 40rem) {
            .student-home {
                padding:
                    var(--dm-space-6)
                    var(--dm-space-4);
            }
        }
    `,
})
export class StudentHomeComponent {}