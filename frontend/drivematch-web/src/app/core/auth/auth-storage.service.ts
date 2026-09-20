import { Injectable, signal } from '@angular/core';

import type { LoginResponse } from './auth.service';

const AUTH_STORAGE_KEY = 'drivematch.auth';
const LAST_EMAIL_STORAGE_KEY = 'drivematch.lastEmail';

@Injectable({
  providedIn: 'root',
})
export class AuthStorageService {
  private readonly sessionState =
    signal<LoginResponse | null>(this.readStoredSession());

  readonly session = this.sessionState.asReadonly();

  saveSession(session: LoginResponse): void {
    localStorage.setItem(
      AUTH_STORAGE_KEY,
      JSON.stringify(session),
    );

    this.sessionState.set(session);
    this.saveLastEmail(session.email);
  }

  updateSessionAccount(
    account: {
      name: string;
      email: string;
    },
  ): void {
    const currentSession = this.sessionState();

    if (!currentSession) {
      return;
    }

    const updatedSession: LoginResponse = {
      ...currentSession,
      name: account.name,
      email: account.email,
    };

    localStorage.setItem(
      AUTH_STORAGE_KEY,
      JSON.stringify(updatedSession),
    );

    this.sessionState.set(updatedSession);
    this.saveLastEmail(updatedSession.email);
  }

  getSession(): LoginResponse | null {
    const session = this.sessionState();

    if (!session) {
      return null;
    }

    if (this.isTokenExpired(session.token)) {
      this.clearSession();
      return null;
    }

    return session;
  }

  getToken(): string | null {
    return this.getSession()?.token ?? null;
  }

  clearSession(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    this.sessionState.set(null);
  }

  saveLastEmail(email: string): void {
    const normalizedEmail = email.trim().toLowerCase();

    if (!normalizedEmail) {
      return;
    }

    localStorage.setItem(
      LAST_EMAIL_STORAGE_KEY,
      normalizedEmail,
    );
  }

  getLastEmail(): string | null {
    return localStorage.getItem(LAST_EMAIL_STORAGE_KEY);
  }

  clearLastEmail(): void {
    localStorage.removeItem(LAST_EMAIL_STORAGE_KEY);
  }

  private readStoredSession(): LoginResponse | null {
    const storedSession =
      localStorage.getItem(AUTH_STORAGE_KEY);

    if (!storedSession) {
      return null;
    }

    try {
      const session =
        JSON.parse(storedSession) as LoginResponse;

      if (
        !session?.token ||
        this.isTokenExpired(session.token)
      ) {
        localStorage.removeItem(AUTH_STORAGE_KEY);
        return null;
      }

      return session;
    } catch {
      localStorage.removeItem(AUTH_STORAGE_KEY);
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    try {
      const parts = token.split('.');

      if (parts.length !== 3) {
        return true;
      }

      const payload = parts[1]
        .replace(/-/g, '+')
        .replace(/_/g, '/');

      const paddedPayload =
        payload +
        '='.repeat((4 - (payload.length % 4)) % 4);

      const decodedPayload = JSON.parse(
        atob(paddedPayload),
      ) as {
        exp?: number;
      };

      if (typeof decodedPayload.exp !== 'number') {
        return true;
      }

      return decodedPayload.exp * 1000 <= Date.now();
    } catch {
      return true;
    }
  }
}