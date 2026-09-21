import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faCalendarCheck,
  faCalendarDays,
  faCarSide,
  faClock,
  faMagnifyingGlass,
  faUser,
} from '@fortawesome/free-solid-svg-icons';

import {
  AccountService,
} from '../../core/account/account.service';

import {
  LessonListItem,
  LessonService,
} from '../../core/lesson/lesson.service';

import {
  LessonRequestService,
} from '../../core/lesson-request/lesson-request.service';

import {
  DashboardActionComponent,
} from '../../shared/components/dashboard-action/dashboard-action.component';

import {
  DashboardStatComponent,
} from '../../shared/components/dashboard-stat/dashboard-stat.component';

@Component({
  selector: 'app-student-home',
  standalone: true,
  imports: [
    RouterLink,
    FontAwesomeModule,
    DashboardActionComponent,
    DashboardStatComponent,
  ],
  templateUrl: './student-home.component.html',
  styleUrl: './student-home.component.scss',
})
export class StudentHomeComponent implements OnInit {
  private readonly accountService =
    inject(AccountService);

  private readonly lessonService =
    inject(LessonService);

  private readonly lessonRequestService =
    inject(LessonRequestService);

  readonly isLoading = signal(true);
  readonly errorMessage = signal('');

  readonly name = signal('');
  readonly upcomingLessonsCount = signal(0);
  readonly pendingRequestsCount = signal(0);
  readonly completedLessonsCount = signal(0);

  readonly nextLesson =
    signal<LessonListItem | null>(null);

  protected readonly faCalendarCheck =
    faCalendarCheck;

  protected readonly faCalendarDays =
    faCalendarDays;

  protected readonly faCarSide =
    faCarSide;

  protected readonly faClock =
    faClock;

  protected readonly faMagnifyingGlass =
    faMagnifyingGlass;

  protected readonly faUser =
    faUser;

  ngOnInit(): void {
    this.loadDashboard();
  }

  formatDate(date: string): string {
    const parsedDate = this.parseDate(date);

    return new Intl.DateTimeFormat(
      'pt-BR',
      {
        day: '2-digit',
        month: 'long',
      },
    ).format(parsedDate);
  }

  formatTime(time: string): string {
    return time.slice(0, 5);
  }

  private loadDashboard(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    forkJoin({
      account: this.accountService.getMyAccount(),
      lessons: this.lessonService.getStudentLessons(),
      requests: this.lessonRequestService.getMine(),
    }).subscribe({
      next: ({
        account,
        lessons,
        requests,
      }) => {
        this.name.set(account.name);

        const upcomingLessons = lessons
          .filter((lesson) =>
            this.isUpcomingLesson(lesson),
          )
          .sort(
            (first, second) =>
              this.getLessonTimestamp(first) -
              this.getLessonTimestamp(second),
          );

        this.upcomingLessonsCount.set(
          upcomingLessons.length,
        );

        this.completedLessonsCount.set(
          lessons.filter(
            (lesson) =>
              lesson.status === 'Completed',
          ).length,
        );

        this.pendingRequestsCount.set(
          requests.filter(
            (request) =>
              request.status === 'Pending',
          ).length,
        );

        this.nextLesson.set(
          upcomingLessons[0] ?? null,
        );

        this.isLoading.set(false);
      },

      error: () => {
        this.errorMessage.set(
          'Não foi possível carregar o resumo da sua conta. Tente novamente.',
        );

        this.isLoading.set(false);
      },
    });
  }

  private isUpcomingLesson(
    lesson: LessonListItem,
  ): boolean {
    return (
      lesson.status === 'Scheduled' ||
      lesson.status === 'CheckIn' ||
      lesson.status === 'InProgress'
    );
  }

  private getLessonTimestamp(
    lesson: LessonListItem,
  ): number {
    return new Date(
      `${lesson.scheduledDate}T${lesson.startTime}`,
    ).getTime();
  }

  private parseDate(date: string): Date {
    const [year, month, day] =
      date.split('-').map(Number);

    return new Date(
      year,
      month - 1,
      day,
    );
  }
}