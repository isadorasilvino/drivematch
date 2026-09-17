import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { API_BASE_URL } from '../services/api.config';

export type LessonStatus =
    | 'Scheduled'
    | 'CheckIn'
    | 'InProgress'
    | 'Completed'
    | 'Cancelled'
    | 'NotAttended';

export interface LessonListItem {
    lessonId: string;
    lessonRequestId: string;
    studentName: string;
    instructorName: string;
    scheduledDate: string;
    startTime: string;
    endTime: string;
    status: LessonStatus;
    startedAt: string | null;
    checkInAt: string | null;
    completedAt: string | null;
    cancelledAt: string | null;
    createdAt: string;
}

interface LessonListResponse {
    lessons: LessonListItem[];
}

export interface StartLessonCheckInResult {
    lessonId: string;
    status: LessonStatus;
    checkInToken: string;
    checkInTokenExpiresAt: string;
}

export interface ConfirmLessonCheckInResult {
    lessonId: string;
    status: LessonStatus;
    checkInAt: string | null;
    startedAt: string | null;
}

export interface LessonActionResult {
    lessonId: string;
    status: LessonStatus;
}

@Injectable({
    providedIn: 'root',
})
export class LessonService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = `${API_BASE_URL}/api/lessons`;

    getStudentLessons(): Observable<LessonListItem[]> {
        return this.http
            .get<LessonListResponse>(`${this.apiUrl}/student`)
            .pipe(map((response) => response.lessons));
    }

    getInstructorLessons(): Observable<LessonListItem[]> {
        return this.http
            .get<LessonListResponse>(`${this.apiUrl}/instructor`)
            .pipe(map((response) => response.lessons));
    }

    startCheckIn(
        lessonId: string,
    ): Observable<StartLessonCheckInResult> {
        return this.http.patch<StartLessonCheckInResult>(
            `${this.apiUrl}/${lessonId}/check-in/start`,
            {},
        );
    }

    confirmCheckIn(
        lessonId: string,
        checkInToken: string,
    ): Observable<ConfirmLessonCheckInResult> {
        return this.http.patch<ConfirmLessonCheckInResult>(
            `${this.apiUrl}/${lessonId}/check-in/confirm`,
            {
                checkInToken,
            },
        );
    }

    completeLesson(
        lessonId: string,
    ): Observable<LessonActionResult> {
        return this.http.patch<LessonActionResult>(
            `${this.apiUrl}/${lessonId}/complete`,
            {},
        );
    }

    cancelLesson(
        lessonId: string,
    ): Observable<LessonActionResult> {
        return this.http.patch<LessonActionResult>(
            `${this.apiUrl}/${lessonId}/cancel`,
            {},
        );
    }

    markAsNotAttended(
        lessonId: string,
    ): Observable<LessonActionResult> {
        return this.http.patch<LessonActionResult>(
            `${this.apiUrl}/${lessonId}/not-attended`,
            {},
        );
    }
}