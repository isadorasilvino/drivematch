import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

import { API_BASE_URL } from '../services/api.config';
import { AuthStorageService } from './auth-storage.service';

export interface LoginRequest {
  email: string;
  password: string;
}

export type UserRole = 'Student' | 'Instructor';

export interface LoginResponse {
  userId: string;
  name: string;
  email: string;
  role: UserRole;
  token: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  role: UserRole;
}

export interface RegisterResponse {
  userId: string;
  name: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly authStorage = inject(AuthStorageService);

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${API_BASE_URL}/api/auth/login`, request)
      .pipe(
        tap((response) => this.authStorage.saveSession(response)),
      );
  }

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(
      `${API_BASE_URL}/api/users/`,
      request,
    );
  }
}