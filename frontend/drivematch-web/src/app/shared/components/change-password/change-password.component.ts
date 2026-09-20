import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
    AccountService,
    ChangePasswordRequest,
} from '../../../core/account/account.service';

import { SectionComponent } from '../section/section.component';

@Component({
    selector: 'app-change-password',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        SectionComponent,
    ],
    templateUrl: './change-password.component.html',
    styleUrl: './change-password.component.scss',
})
export class ChangePasswordComponent {
    private readonly accountService = inject(AccountService);

    currentPassword = '';
    newPassword = '';
    confirmPassword = '';

    readonly isSaving = signal(false);
    readonly errorMessage = signal('');
    readonly successMessage = signal('');

    readonly showCurrentPassword = signal(false);
    readonly showNewPassword = signal(false);
    readonly showConfirmPassword = signal(false);

    toggleCurrentPassword(): void {
        this.showCurrentPassword.update((value) => !value);
    }

    toggleNewPassword(): void {
        this.showNewPassword.update((value) => !value);
    }

    toggleConfirmPassword(): void {
        this.showConfirmPassword.update((value) => !value);
    }

    changePassword(): void {
        this.errorMessage.set('');
        this.successMessage.set('');

        if (
            !this.currentPassword ||
            !this.newPassword ||
            !this.confirmPassword
        ) {
            this.errorMessage.set(
                'Preencha todos os campos para alterar sua senha.',
            );
            return;
        }

        if (this.newPassword !== this.confirmPassword) {
            this.errorMessage.set(
                'A confirmação da nova senha não corresponde.',
            );
            return;
        }

        if (this.currentPassword === this.newPassword) {
            this.errorMessage.set(
                'A nova senha deve ser diferente da senha atual.',
            );
            return;
        }

        const request: ChangePasswordRequest = {
            currentPassword: this.currentPassword,
            newPassword: this.newPassword,
        };

        this.isSaving.set(true);

        this.accountService.changePassword(request).subscribe({
            next: () => {
                this.isSaving.set(false);

                this.currentPassword = '';
                this.newPassword = '';
                this.confirmPassword = '';

                this.successMessage.set(
                    'Senha alterada com sucesso.',
                );
            },

            error: (error: HttpErrorResponse) => {
                this.isSaving.set(false);

                if (error.status === 400) {
                    this.errorMessage.set(
                        error.error?.error ??
                        'A senha atual informada está incorreta.',
                    );
                    return;
                }

                if (error.status === 401) {
                    this.errorMessage.set(
                        'Sua sessão expirou. Entre novamente para alterar sua senha.',
                    );
                    return;
                }

                this.errorMessage.set(
                    error.error?.error ??
                    'Não foi possível alterar sua senha. Tente novamente.',
                );
            },
        });
    }
}