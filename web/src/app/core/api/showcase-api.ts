import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ShowcaseSession } from '../models/showcase.model';
import { withoutErrorNotification } from '../http/http-context';
import { API_BASE_URL } from './api-config';

@Injectable({ providedIn: 'root' })
export class ShowcaseApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/api/v1/showcase`;

  /** Seeds the demo application if needed and issues it a new API key. */
  start(): Observable<ShowcaseSession> {
    // The page explains a disabled showcase itself, so no toast.
    return this.http.post<ShowcaseSession>(`${this.baseUrl}/session`, null, {
      context: withoutErrorNotification(),
    });
  }

  /** Deletes and re-seeds the demo application. */
  reset(): Observable<ShowcaseSession> {
    return this.http.post<ShowcaseSession>(`${this.baseUrl}/reset`, null);
  }
}
