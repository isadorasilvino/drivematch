import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export type LessonRequestStatus =
    | 'Pending'
    | 'Accepted'
    | 'Confirmed'
    | 'Rejected'
    | 'Cancelled'
    | 'Expired';

export interface CreateLessonRequestRequest {
    instructorProfileId: string;
    requestedDate: string;
    startTime: string;
    endTime: string;
    usesStudentVehicle: boolean;
    studentMessage: string | null;
}

export interface CreateLessonRequestResponse {
    lessonRequestId: string;
    studentId: string;
    instructorId: string;
    requestedDate: string;
    startTime: string;
    endTime: string;
    usesStudentVehicle: boolean;
    studentMessage: string | null;
    status: LessonRequestStatus;
}

export interface LessonRequestListItem {
    lessonRequestId: string;
    studentName: string;
    instructorName: string;
    requestedDate: string;
    startTime: string;
    endTime: string;
    usesStudentVehicle: boolean;
    studentMessage: string | null;
    status: LessonRequestStatus;
    createdAt: string;
    updatedAt: string | null;
}

export interface AcceptLessonRequestResponse {
    lessonRequestId: string;
    lessonRequestStatus: LessonRequestStatus;
    lessonId: string;
    lessonStatus: string;
    studentId: string;
    instructorId: string;
    scheduledDate: string;
    startTime: string;
    endTime: string;
}

export interface RejectLessonRequestResponse {
    lessonRequestId: string;
    status: LessonRequestStatus;
}

@Injectable({
    providedIn: 'root',
})
export class LessonRequestService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = 'http://localhost:8080/api/lessons';

    create(
        request: CreateLessonRequestRequest,
    ): Observable<CreateLessonRequestResponse> {
        return this.http.post<CreateLessonRequestResponse>(
            this.apiUrl,
            request,
        );
    }

    getMine(): Observable<LessonRequestListItem[]> {
        return this.http.get<LessonRequestListItem[]>(
            `${this.apiUrl}/mine`,
        );
    }

    getReceived(): Observable<LessonRequestListItem[]> {
        return this.http.get<LessonRequestListItem[]>(
            `${this.apiUrl}/received`,
        );
    }

    accept(
        lessonRequestId: string,
    ): Observable<AcceptLessonRequestResponse> {
        return this.http.patch<AcceptLessonRequestResponse>(
            `${this.apiUrl}/${lessonRequestId}/accept`,
            {},
        );
    }

    cancel(
        lessonRequestId: string,
    ): Observable<RejectLessonRequestResponse> {
        return this.http.patch<RejectLessonRequestResponse>(
            `${this.apiUrl}/${lessonRequestId}/request-cancel`,
            {},
        );
    }
    reject(
        lessonRequestId: string,
    ): Observable<RejectLessonRequestResponse> {
        return this.http.patch<RejectLessonRequestResponse>(
            `${this.apiUrl}/${lessonRequestId}/reject`,
            {},
        );
    }
}