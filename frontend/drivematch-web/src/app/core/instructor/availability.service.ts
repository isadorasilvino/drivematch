import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../services/api.config';

export type AvailabilityDay =
    | 'Sunday'
    | 'Monday'
    | 'Tuesday'
    | 'Wednesday'
    | 'Thursday'
    | 'Friday'
    | 'Saturday';

export interface AvailabilityResponse {
    availabilityId: string;
    instructorProfileId: string;
    dayOfWeek: AvailabilityDay;
    startTime: string;
    endTime: string;
    lessonDurationMinutes: number;
    breakDurationMinutes: number;
    isActive: boolean;
}

export interface AvailabilityRequest {
    dayOfWeek: AvailabilityDay;
    startTime: string;
    endTime: string;
    lessonDurationMinutes: number;
    breakDurationMinutes: number;
}

export interface ChangeAvailabilityStatusRequest {
    isActive: boolean;
}

@Injectable({
    providedIn: 'root',
})
export class AvailabilityService {
    private readonly http = inject(HttpClient);

    getMine(): Observable<AvailabilityResponse[]> {
        return this.http.get<AvailabilityResponse[]>(
            `${API_BASE_URL}/api/availabilities/`,
        );
    }

    create(
        request: AvailabilityRequest,
    ): Observable<AvailabilityResponse> {
        return this.http.post<AvailabilityResponse>(
            `${API_BASE_URL}/api/availabilities/`,
            request,
        );
    }

    update(
        availabilityId: string,
        request: AvailabilityRequest,
    ): Observable<AvailabilityResponse> {
        return this.http.put<AvailabilityResponse>(
            `${API_BASE_URL}/api/availabilities/${availabilityId}`,
            request,
        );
    }

    changeStatus(
        availabilityId: string,
        isActive: boolean,
    ): Observable<AvailabilityResponse> {
        const request: ChangeAvailabilityStatusRequest = {
            isActive,
        };

        return this.http.patch<AvailabilityResponse>(
            `${API_BASE_URL}/api/availabilities/${availabilityId}/status`,
            request,
        );
    }
}