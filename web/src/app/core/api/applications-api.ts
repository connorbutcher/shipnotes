import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Application,
  ApplicationWithApiKey,
  SaveApplicationRequest,
} from '../models/application.model';
import { API_BASE_URL } from './api-config';

@Injectable({ providedIn: 'root' })
export class ApplicationsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/api/v1/applications`;

  list(): Observable<Application[]> {
    return this.http.get<Application[]>(this.baseUrl);
  }

  create(request: SaveApplicationRequest): Observable<ApplicationWithApiKey> {
    return this.http.post<ApplicationWithApiKey>(this.baseUrl, request);
  }

  update(id: string, request: SaveApplicationRequest): Observable<Application> {
    return this.http.put<Application>(`${this.baseUrl}/${id}`, request);
  }

  regenerateApiKey(id: string): Observable<ApplicationWithApiKey> {
    return this.http.post<ApplicationWithApiKey>(`${this.baseUrl}/${id}/api-key`, null);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
