import { ProblemDetails } from '../../../core/models/problem-details.model';
import {
  PublicApplication,
  PublicReleaseNote,
} from '../../../core/models/public-release-note.model';

/** The outcome of one console request, successful or not. */
export interface QueryResult {
  method: 'GET';
  url: string;
  status: number;
  statusText: string;
  ok: boolean;
  durationMs: number;
  body: unknown;
}

/** How a response body is shown on the Rendered tab. */
export type RenderedView =
  | { kind: 'notes'; notes: PublicReleaseNote[] }
  | { kind: 'application'; application: PublicApplication }
  | { kind: 'problem'; message: string }
  | { kind: 'empty' };

function isNote(value: unknown): value is PublicReleaseNote {
  return typeof value === 'object' && value !== null && 'sections' in value && 'version' in value;
}

function isApplication(value: unknown): value is PublicApplication {
  return typeof value === 'object' && value !== null && 'name' in value;
}

/** Works out what a consuming app would render for this response. */
export function interpretResult(result: QueryResult): RenderedView {
  const { body } = result;

  if (!result.ok) {
    const problem = (body ?? {}) as ProblemDetails;
    const fieldError = problem.errors ? Object.values(problem.errors).flat()[0] : undefined;
    return {
      kind: 'problem',
      message:
        fieldError ??
        problem.detail ??
        problem.title ??
        (result.status === 0 ? 'Unable to reach the ShipNotes API.' : result.statusText),
    };
  }

  if (Array.isArray(body)) {
    const notes = body.filter(isNote);
    return notes.length ? { kind: 'notes', notes } : { kind: 'empty' };
  }

  if (isNote(body)) {
    return { kind: 'notes', notes: [body] };
  }

  return isApplication(body) ? { kind: 'application', application: body } : { kind: 'empty' };
}

/** PrimeNG tag severity for an HTTP status. */
export function statusSeverity(status: number): 'success' | 'warn' | 'danger' {
  if (status >= 200 && status < 300) {
    return 'success';
  }
  return status >= 400 && status < 500 ? 'warn' : 'danger';
}

const REASON_PHRASES: Record<number, string> = {
  0: 'Network error',
  200: 'OK',
  400: 'Bad Request',
  401: 'Unauthorized',
  403: 'Forbidden',
  404: 'Not Found',
  429: 'Too Many Requests',
  500: 'Internal Server Error',
};

/** The standard reason phrase for a status (fetch over HTTP/2 doesn't report one). */
export function reasonPhrase(status: number): string {
  return REASON_PHRASES[status] ?? '';
}
