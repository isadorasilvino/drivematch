import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { UserRole } from '../auth/auth.service';
import { API_BASE_URL } from '../services/api.config';

export interface MyAccountResponse {
  userId: string;
  name: string;
  email: string;
  role: UserRole;
}

export interface UpdateMyAccountRequest {
  name: string;
  email: string;
}

export interface UpdateMyAccountResponse {
  userId: string;
  name: string;
  email: string;
  role: UserRole;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private readonly http = inject(HttpClient);

  getMyAccount(): Observable<MyAccountResponse> {
    return this.http.get<MyAccountResponse>(
      `${API_BASE_URL}/api/users/me`,
    );
  }

  updateMyAccount(
    request: UpdateMyAccountRequest,
  ): Observable<UpdateMyAccountResponse> {
    return this.http.put<UpdateMyAccountResponse>(
      `${API_BASE_URL}/api/users/me`,
      request,
    );
  }

  changePassword(
    request: ChangePasswordRequest,
  ): Observable<void> {
    return this.http.put<void>(
      `${API_BASE_URL}/api/users/me/password`,
      request,
    );
  }
}