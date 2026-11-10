import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Media } from '../models/media.model';
import { API_BASE_URL } from './api-config';

@Injectable({ providedIn: 'root' })
export class MediaApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  /** Uploads an image for an application's release notes. */
  upload(applicationId: string, file: File): Observable<Media> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<Media>(
      `${this.baseUrl}/api/v1/applications/${applicationId}/media`,
      form,
    );
  }
}
