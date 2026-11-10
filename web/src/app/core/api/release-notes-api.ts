import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ReleaseNote,
  ReleaseNoteSummary,
  SaveReleaseNoteRequest,
} from '../models/release-note.model';
import { API_BASE_URL } from './api-config';

@Injectable({ providedIn: 'root' })
export class ReleaseNotesApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  list(applicationId: string): Observable<ReleaseNoteSummary[]> {
    return this.http.get<ReleaseNoteSummary[]>(this.url(applicationId));
  }

  get(applicationId: string, id: string): Observable<ReleaseNote> {
    return this.http.get<ReleaseNote>(this.url(applicationId, id));
  }

  create(applicationId: string, request: SaveReleaseNoteRequest): Observable<ReleaseNote> {
    return this.http.post<ReleaseNote>(this.url(applicationId), request);
  }

  update(
    applicationId: string,
    id: string,
    request: SaveReleaseNoteRequest,
  ): Observable<ReleaseNote> {
    return this.http.put<ReleaseNote>(this.url(applicationId, id), request);
  }

  delete(applicationId: string, id: string): Observable<void> {
    return this.http.delete<void>(this.url(applicationId, id));
  }

  private url(applicationId: string, id?: string): string {
    const base = `${this.baseUrl}/api/v1/applications/${applicationId}/release-notes`;
    return id ? `${base}/${id}` : base;
  }
}
