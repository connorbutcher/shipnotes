import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, finalize, tap } from 'rxjs';
import { ShowcaseApi } from '../../../core/api/showcase-api';
import { describeHttpError } from '../../../core/http/problem-details';
import { ShowcaseSession } from '../../../core/models/showcase.model';
import { ApplicationsStore } from '../../../core/state/applications-store';

export type ShowcaseStatus = 'idle' | 'loading' | 'ready' | 'disabled' | 'failed';

/**
 * The showcase session for one visit to the page: the seeded demo application and its API key.
 * Provided by the showcase page, so each visit issues a fresh key.
 */
@Injectable()
export class ShowcaseStore {
  private readonly api = inject(ShowcaseApi);
  private readonly applications = inject(ApplicationsStore);

  private readonly sessionState = signal<ShowcaseSession | null>(null);
  private readonly statusState = signal<ShowcaseStatus>('idle');
  private readonly errorState = signal<string | null>(null);
  private readonly resettingState = signal(false);

  readonly session = this.sessionState.asReadonly();
  readonly status = this.statusState.asReadonly();
  readonly error = this.errorState.asReadonly();
  readonly resetting = this.resettingState.asReadonly();
  readonly application = computed(() => this.sessionState()?.application ?? null);

  /** Bumped whenever the demo is re-seeded, so dependent data reloads. */
  readonly revision = signal(0);

  start(): void {
    this.statusState.set('loading');
    this.api.start().subscribe({
      next: (session) => this.accept(session),
      error: (error: unknown) => {
        const disabled = error instanceof HttpErrorResponse && error.status === 404;
        this.errorState.set(disabled ? null : describeHttpError(error));
        this.statusState.set(disabled ? 'disabled' : 'failed');
      },
    });
  }

  reset(): Observable<ShowcaseSession> {
    this.resettingState.set(true);
    return this.api.reset().pipe(
      tap((session) => this.accept(session)),
      finalize(() => this.resettingState.set(false)),
    );
  }

  private accept(session: ShowcaseSession): void {
    this.sessionState.set(session);
    this.errorState.set(null);
    this.statusState.set('ready');
    this.revision.update((n) => n + 1);
    // The demo shows up in the side panel like any other application.
    this.applications.upsert(session.application);
  }
}
