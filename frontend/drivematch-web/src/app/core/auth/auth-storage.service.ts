import { Injectable } from '@angular/core';

import type { LoginResponse } from './auth.service';

const AUTH_STORAGE_KEY = 'drivematch.auth';

@Injectable({
  providedIn: 'root',
})
export class AuthStorageService {
  saveSession(session: LoginResponse): void {
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session));
  }

  getSession(): LoginResponse | null {
    const storedSession = localStorage.getItem(AUTH_STORAGE_KEY);

    if (!storedSession) {
      return null;
    }

    return JSON.parse(storedSession) as LoginResponse;
  }

  getToken(): string | null {
    return this.getSession()?.token ?? null;
  }

  clearSession(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
  }
}