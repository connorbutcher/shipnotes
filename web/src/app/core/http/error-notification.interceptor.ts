import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../notifications/notification.service';
import { describeHttpError } from './problem-details';

/** Shows a toast for every failed API request, then rethrows so callers can react. */
export const errorNotificationInterceptor: HttpInterceptorFn = (request, next) => {
  const notifications = inject(NotificationService);

  return next(request).pipe(
    catchError((error: unknown) => {
      notifications.error(describeHttpError(error));
      return throwError(() => error);
    }),
  );
};
