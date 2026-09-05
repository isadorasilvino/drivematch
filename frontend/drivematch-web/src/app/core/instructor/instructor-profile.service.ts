import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../services/api.config';

export type InstructorProfileStatus = 'Draft' | 'Active' | 'Inactive';

export interface InstructorProfileRequest {
    description: string;
    experienceYears: number;
    city: string;
    state: string;
    pricePerLesson: number;
    acceptsBeginners: boolean;
    acceptsExperiencedStudents: boolean;
    acceptsStudentVehicle: boolean;
}

export interface InstructorProfileResponse {
    instructorProfileId: string;
    userId: string;
    description: string;
    experienceYears: number;
    city: string;
    state: string;
    pricePerLesson: number;
    currency: string;
    acceptsBeginners: boolean;
    acceptsExperiencedStudents: boolean;
    acceptsStudentVehicle: boolean;
    status: InstructorProfileStatus;
    updatedAt?: string | null;
}

export interface ChangeInstructorProfileStatusResponse {
    instructorProfileId: string;
    status: InstructorProfileStatus;
}

@Injectable({
    providedIn: 'root',
})
export class InstructorProfileService {
    private readonly http = inject(HttpClient);

    getProfile(): Observable<InstructorProfileResponse> {
        return this.http.get<InstructorProfileResponse>(
            `${API_BASE_URL}/api/instructors/profile`,
        );
    }

    createProfile(
        request: InstructorProfileRequest,
    ): Observable<InstructorProfileResponse> {
        return this.http.post<InstructorProfileResponse>(
            `${API_BASE_URL}/api/instructors/profile`,
            request,
        );
    }

    updateProfile(
        request: InstructorProfileRequest,
    ): Observable<InstructorProfileResponse> {
        return this.http.put<InstructorProfileResponse>(
            `${API_BASE_URL}/api/instructors/profile`,
            request,
        );
    }

    changeStatus(
        isActive: boolean,
    ): Observable<ChangeInstructorProfileStatusResponse> {
        return this.http.patch<ChangeInstructorProfileStatusResponse>(
            `${API_BASE_URL}/api/instructors/profile/status`,
            { isActive },
        );
    }
}