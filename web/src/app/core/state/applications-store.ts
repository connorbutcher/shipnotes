import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, finalize, map, tap } from 'rxjs';
import { ApplicationsApi } from '../api/applications-api';
import { Application, SaveApplicationRequest } from '../models/application.model';

export interface IssuedApiKey {
  applicationId: string;
  apiKey: string;
}

/**
 * Holds the application list shown in the side panel and keeps it in sync
 * with create, update and delete operations.
 */
@Injectable({ providedIn: 'root' })
export class ApplicationsStore {
  private readonly api = inject(ApplicationsApi);

  private readonly state = signal<Application[]>([]);
  private readonly loadingState = signal(false);
  private readonly issuedKey = signal<IssuedApiKey | null>(null);

  readonly applications = this.state.asReadonly();
  readonly loading = this.loadingState.asReadonly();
  readonly isEmpty = computed(() => !this.loadingState() && this.state().length === 0);

  /** The most recently issued key; shown once, then cleared. */
  readonly lastIssuedKey = this.issuedKey.asReadonly();

  load(): void {
    this.loadingState.set(true);
    this.api
      .list()
      .pipe(finalize(() => this.loadingState.set(false)))
      .subscribe((applications) => this.state.set(applications));
  }

  byId(id: string): Application | undefined {
    return this.state().find((a) => a.id === id);
  }

  create(request: SaveApplicationRequest): Observable<Application> {
    return this.api.create(request).pipe(
      tap(({ application, apiKey }) => {
        this.upsert(application);
        this.issuedKey.set({ applicationId: application.id, apiKey });
      }),
      map(({ application }) => application),
    );
  }

  update(id: string, request: SaveApplicationRequest): Observable<Application> {
    return this.api.update(id, request).pipe(tap((application) => this.upsert(application)));
  }

  regenerateApiKey(id: string): Observable<Application> {
    return this.api.regenerateApiKey(id).pipe(
      tap(({ application, apiKey }) => {
        this.upsert(application);
        this.issuedKey.set({ applicationId: application.id, apiKey });
      }),
      map(({ application }) => application),
    );
  }

  delete(id: string): Observable<void> {
    return this.api
      .delete(id)
      .pipe(tap(() => this.state.update((apps) => apps.filter((a) => a.id !== id))));
  }

  clearIssuedKey(): void {
    this.issuedKey.set(null);
  }

  private upsert(application: Application): void {
    this.state.update((apps) =>
      [...apps.filter((a) => a.id !== application.id), application].sort((a, b) =>
        a.name.localeCompare(b.name),
      ),
    );
  }
}
