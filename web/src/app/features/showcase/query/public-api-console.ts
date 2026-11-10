import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, of } from 'rxjs';
import { API_BASE_URL } from '../../../core/api/api-config';
import { withoutErrorNotification } from '../../../core/http/http-context';
import { ShowcaseSession } from '../../../core/models/showcase.model';
import { QueryRequest, buildQueryPath, publicBasePath } from './query-request';
import { QueryResult, reasonPhrase } from './query-result';

const API_KEY_HEADER = 'X-Api-Key';

/**
 * Calls the public release notes API exactly as a consuming application would, and reports the
 * status, timing and body of every response, including errors, instead of raising toasts.
 */
@Injectable({ providedIn: 'root' })
export class PublicApiConsole {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  /** The URL a request goes to, relative to this UI when the API is on the same origin. */
  urlFor(session: ShowcaseSession, request: QueryRequest): string {
    return `${this.baseUrl}${publicBasePath(session.application.id)}${buildQueryPath(request)}`;
  }

  send(session: ShowcaseSession, request: QueryRequest): Observable<QueryResult> {
    const url = this.urlFor(session, request);
    const headers = request.sendApiKey
      ? new HttpHeaders({ [API_KEY_HEADER]: session.apiKey })
      : new HttpHeaders();
    const started = performance.now();
    const elapsed = () => Math.round(performance.now() - started);

    return this.http
      .get<unknown>(url, { headers, observe: 'response', context: withoutErrorNotification() })
      .pipe(
        map((response): QueryResult => ({
          method: 'GET',
          url,
          status: response.status,
          statusText: reasonPhrase(response.status),
          ok: true,
          durationMs: elapsed(),
          body: response.body,
        })),
        catchError((error: HttpErrorResponse) =>
          of<QueryResult>({
            method: 'GET',
            url,
            status: error.status,
            statusText: reasonPhrase(error.status),
            ok: false,
            durationMs: elapsed(),
            body: error.error ?? null,
          }),
        ),
      );
  }
}
