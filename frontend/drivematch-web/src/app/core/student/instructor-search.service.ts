import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../services/api.config';
import { ExperienceLevel } from './student-profile.service';

export interface InstructorSearchFilters {
  city: string;
  state: string;
  experienceLevel: ExperienceLevel;
  usesStudentVehicle: boolean;
  maxPricePerLesson?: number | null;
}

export interface InstructorSearchResult {
  instructorProfileId: string;
  userId: string;
  name: string;
  description: string;
  experienceYears: number;
  city: string;
  state: string;
  pricePerLesson: number;
  currency: string;
  acceptsBeginners: boolean;
  acceptsExperiencedStudents: boolean;
  acceptsStudentVehicle: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class InstructorSearchService {
  private readonly http = inject(HttpClient);

  search(
    filters: InstructorSearchFilters,
  ): Observable<InstructorSearchResult[]> {
    let params = new HttpParams()
      .set('city', filters.city.trim())
      .set('state', filters.state)
      .set('experienceLevel', filters.experienceLevel)
      .set(
        'usesStudentVehicle',
        filters.usesStudentVehicle.toString(),
      );

    if (
      filters.maxPricePerLesson !== null &&
      filters.maxPricePerLesson !== undefined
    ) {
      params = params.set(
        'maxPricePerLesson',
        filters.maxPricePerLesson.toString(),
      );
    }

    return this.http.get<InstructorSearchResult[]>(
      `${API_BASE_URL}/api/instructors/search`,
      { params },
    );
  }
}