import { HttpErrorResponse } from '@angular/common/http';
import { ProblemDetails } from '../models/problem-details.model';

/** Turns an HTTP error into a single human-readable message. */
export function describeHttpError(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Something went wrong.';
  }

  if (error.status === 0) {
    return 'Unable to reach the ShipNotes API.';
  }

  const problem = error.error as ProblemDetails | null;
  const firstFieldError = problem?.errors ? Object.values(problem.errors).flat()[0] : undefined;
  return (
    firstFieldError ?? problem?.detail ?? problem?.title ?? `Request failed (${error.status}).`
  );
}
