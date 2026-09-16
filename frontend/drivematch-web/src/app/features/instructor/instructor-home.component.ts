import { Component } from '@angular/core';

@Component({
    selector: 'app-instructor-home',
    standalone: true,
    template: `
        <main class="instructor-home">
            <div class="instructor-home__container">
                <header class="instructor-home__header">
                    <span class="instructor-home__eyebrow">
                        Área do instrutor
                    </span>

                    <h1>Visão geral</h1>

                    <p>
                        Acompanhe suas aulas, solicitações
                        e atividades mais importantes.
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

        .instructor-home {
            padding: var(--dm-space-8) var(--dm-space-5);
        }

        .instructor-home__container {
            width: 100%;
            max-width: var(--dm-content-max-width);
            margin: 0 auto;
        }

        .instructor-home__header {
            max-width: 42rem;
        }

        .instructor-home__eyebrow {
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
            .instructor-home {
                padding:
                    var(--dm-space-6)
                    var(--dm-space-4);
            }
        }
    `,
})
export class InstructorHomeComponent {}