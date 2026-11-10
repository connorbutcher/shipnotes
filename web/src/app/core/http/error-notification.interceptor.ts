import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../notifications/notification.service';
import { SKIP_ERROR_NOTIFICATION } from './http-context';
import { describeHttpError } from './problem-details';

/**
 * Shows a toast for every failed API request, then rethrows so callers can react.
 * Requests sent with `withoutErrorNotification()` are passed through untouched.
 */
export const errorNotificationInterceptor: HttpInterceptorFn = (request, next) => {
  if (request.context.get(SKIP_ERROR_NOTIFICATION)) {
    return next(request);
  }

  const notifications = inject(NotificationService);

  return next(request).pipe(
    catchError((error: unknown) => {
      notifications.error(describeHttpError(error));
      return throwError(() => error);
    }),
  );
};
