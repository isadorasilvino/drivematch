import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../services/api.config';

export type ExperienceLevel = 'Beginner' | 'Experienced';

export interface StudentProfileRequest {
  city: string;
  state: string;
  experienceLevel: ExperienceLevel;
  ownsVehicle: boolean;
  hasOwnVehicleForLessons: boolean;
}

export interface StudentProfileResponse {
  studentProfileId: string;
  userId: string;
  city: string;
  state: string;
  experienceLevel: ExperienceLevel;
  ownsVehicle: boolean;
  hasOwnVehicleForLessons: boolean;
  updatedAt?: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class StudentProfileService {
  private readonly http = inject(HttpClient);

  getProfile(): Observable<StudentProfileResponse> {
    return this.http.get<StudentProfileResponse>(
      `${API_BASE_URL}/api/students/profile`,
    );
  }

  createProfile(
    request: StudentProfileRequest,
  ): Observable<StudentProfileResponse> {
    return this.http.post<StudentProfileResponse>(
      `${API_BASE_URL}/api/students/profile`,
      request,
    );
  }

  updateProfile(
    request: StudentProfileRequest,
  ): Observable<StudentProfileResponse> {
    return this.http.put<StudentProfileResponse>(
      `${API_BASE_URL}/api/students/profile`,
      request,
    );
  }
}