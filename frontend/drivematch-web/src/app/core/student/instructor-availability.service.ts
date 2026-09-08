import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../services/api.config';

export interface AvailableSlot {
  startTime: string;
  endTime: string;
}

@Injectable({
  providedIn: 'root',
})
export class InstructorAvailabilityService {
  private readonly http = inject(HttpClient);

  getAvailableSlots(
    instructorProfileId: string,
    date: string,
  ): Observable<AvailableSlot[]> {
    const params = new HttpParams()
      .set('date', date);

    return this.http.get<AvailableSlot[]>(
      `${API_BASE_URL}/api/instructors/${instructorProfileId}/available-slots`,
      { params },
    );
  }
}