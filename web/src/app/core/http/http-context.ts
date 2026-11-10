import { HttpContext, HttpContextToken } from '@angular/common/http';

/** When true, the error interceptor lets the caller handle a failed request without a toast. */
export const SKIP_ERROR_NOTIFICATION = new HttpContextToken<boolean>(() => false);

/** Context for requests whose errors are expected and shown by the caller, e.g. the showcase console. */
export function withoutErrorNotification(context = new HttpContext()): HttpContext {
  return context.set(SKIP_ERROR_NOTIFICATION, true);
}
