import { Injectable } from '@angular/core';

import type { LoginResponse } from './auth.service';

const AUTH_STORAGE_KEY = 'drivematch.auth';
const LAST_EMAIL_STORAGE_KEY = 'drivematch.lastEmail';

@Injectable({
  providedIn: 'root',
})
export class AuthStorageService {
  saveSession(session: LoginResponse): void {
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session));
    this.saveLastEmail(session.email);
  }

  getSession(): LoginResponse | null {
    const storedSession = localStorage.getItem(AUTH_STORAGE_KEY);

    if (!storedSession) {
      return null;
    }

    try {
      const session = JSON.parse(storedSession) as LoginResponse;

      if (!session?.token || this.isTokenExpired(session.token)) {
        this.clearSession();
        return null;
      }

      return session;
    } catch {
      this.clearSession();
      return null;
    }
  }

  getToken(): string | null {
    return this.getSession()?.token ?? null;
  }

  clearSession(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
  }

  saveLastEmail(email: string): void {
    const normalizedEmail = email.trim().toLowerCase();

    if (!normalizedEmail) {
      return;
    }

    localStorage.setItem(LAST_EMAIL_STORAGE_KEY, normalizedEmail);
  }

  getLastEmail(): string | null {
    return localStorage.getItem(LAST_EMAIL_STORAGE_KEY);
  }

  clearLastEmail(): void {
    localStorage.removeItem(LAST_EMAIL_STORAGE_KEY);
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
        payload + '='.repeat((4 - (payload.length % 4)) % 4);

      const decodedPayload = JSON.parse(atob(paddedPayload)) as {
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