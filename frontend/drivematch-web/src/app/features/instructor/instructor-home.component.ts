import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
    faCalendarDays,
    faCalendarPlus,
    faClock,
    faListCheck,
    faUser,
    faUsers,
} from '@fortawesome/free-solid-svg-icons';
import { forkJoin } from 'rxjs';

import {
    AuthStorageService,
} from '../../core/auth/auth-storage.service';

import {
    InstructorProfileService,
    InstructorProfileStatus,
} from '../../core/instructor/instructor-profile.service';

import {
    LessonListItem,
    LessonService,
} from '../../core/lesson/lesson.service';

import {
    LessonRequestListItem,
    LessonRequestService,
} from '../../core/lesson-request/lesson-request.service';

import {
    DashboardActionComponent,
} from '../../shared/components/dashboard-action/dashboard-action.component';

import {
    DashboardStatComponent,
} from '../../shared/components/dashboard-stat/dashboard-stat.component';

@Component({
    selector: 'app-instructor-home',
    standalone: true,
    imports: [
        RouterLink,
        FontAwesomeModule,
        DashboardActionComponent,
        DashboardStatComponent,
    ],
    templateUrl: './instructor-home.component.html',
    styleUrl: './instructor-home.component.scss',
})
export class InstructorHomeComponent implements OnInit {
    private readonly authStorage = inject(AuthStorageService);
    private readonly lessonService = inject(LessonService);
    private readonly lessonRequestService = inject(LessonRequestService);
    private readonly instructorProfileService =
        inject(InstructorProfileService);

    protected readonly faCalendarDays = faCalendarDays;
    protected readonly faCalendarPlus = faCalendarPlus;
    protected readonly faClock = faClock;
    protected readonly faListCheck = faListCheck;
    protected readonly faUser = faUser;
    protected readonly faUsers = faUsers;

    readonly isLoading = signal(true);
    readonly hasError = signal(false);

    readonly profileStatus =
        signal<InstructorProfileStatus>('Draft');

    readonly upcomingLessons = signal<LessonListItem[]>([]);
    readonly lessonRequests = signal<LessonRequestListItem[]>([]);

    readonly instructorName =
        signal(this.authStorage.getSession()?.name ?? 'Instrutor');

    ngOnInit(): void {
        this.loadDashboard();
    }

    get pendingRequestsCount(): number {
        return this.lessonRequests().filter(
            (request) => request.status === 'Pending',
        ).length;
    }

    get upcomingLessonsCount(): number {
        return this.upcomingLessons().filter(
            (lesson) => this.isUpcomingLesson(lesson),
        ).length;
    }

    get completedLessonsCount(): number {
        return this.upcomingLessons().filter(
            (lesson) => lesson.status === 'Completed',
        ).length;
    }

    get nextLesson(): LessonListItem | null {
        const lessons = this.upcomingLessons()
            .filter((lesson) => this.isUpcomingLesson(lesson))
            .sort(
                (first, second) =>
                    this.getLessonTimestamp(first) -
                    this.getLessonTimestamp(second),
            );

        return lessons[0] ?? null;
    }

    get isProfileActive(): boolean {
        return this.profileStatus() === 'Active';
    }

    get profileStatusLabel(): string {
        switch (this.profileStatus()) {
            case 'Active':
                return 'Perfil ativo';

            case 'Inactive':
                return 'Perfil desativado';

            default:
                return 'Perfil não ativado';
        }
    }

    formatLessonDate(lesson: LessonListItem): string {
        const date = this.parseDate(lesson.scheduledDate);

        return new Intl.DateTimeFormat('pt-BR', {
            weekday: 'long',
            day: '2-digit',
            month: 'long',
        }).format(date);
    }

    formatLessonTime(lesson: LessonListItem): string {
        return `${lesson.startTime.slice(0, 5)} às ${lesson.endTime.slice(0, 5)}`;
    }

    private loadDashboard(): void {
        this.isLoading.set(true);
        this.hasError.set(false);

        forkJoin({
            lessons: this.lessonService.getInstructorLessons(),
            requests: this.lessonRequestService.getReceived(),
            profile: this.instructorProfileService.getProfile(),
        }).subscribe({
            next: ({ lessons, requests, profile }) => {
                this.upcomingLessons.set(lessons);
                this.lessonRequests.set(requests);
                this.profileStatus.set(profile.status);

                this.isLoading.set(false);
            },

            error: () => {
                this.hasError.set(true);
                this.isLoading.set(false);
            },
        });
    }

    private isUpcomingLesson(lesson: LessonListItem): boolean {
        if (
            lesson.status === 'Completed' ||
            lesson.status === 'Cancelled' ||
            lesson.status === 'NotAttended'
        ) {
            return false;
        }

        return this.getLessonTimestamp(lesson) >= Date.now();
    }

    private getLessonTimestamp(lesson: LessonListItem): number {
        return new Date(
            `${lesson.scheduledDate}T${lesson.startTime}`,
        ).getTime();
    }

    private parseDate(value: string): Date {
        const [year, month, day] = value
            .slice(0, 10)
            .split('-')
            .map(Number);

        return new Date(year, month - 1, day);
    }
}