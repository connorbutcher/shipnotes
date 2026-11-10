import { DOCUMENT } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  computed,
  inject,
  input,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, switchMap, tap } from 'rxjs';
import { Card } from 'primeng/card';
import { ShowcaseSession } from '../../../../core/models/showcase.model';
import { PublicApiConsole } from '../../query/public-api-console';
import { QueryPreset, applyPreset } from '../../query/query-presets';
import { DEFAULT_QUERY, QueryRequest } from '../../query/query-request';
import { QueryResult } from '../../query/query-result';
import { QueryBuilder } from '../query-builder/query-builder';
import { QueryPresets } from '../query-presets/query-presets';
import { QueryResponse } from '../query-response/query-response';
import { RequestPreview } from '../request-preview/request-preview';

/** Interactive client for the public API: pick a scenario or build a request, send it, inspect the result. */
@Component({
  selector: 'app-query-console',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Card, QueryBuilder, QueryPresets, QueryResponse, RequestPreview],
  templateUrl: './query-console.html',
  styleUrl: './query-console.scss',
})
export class QueryConsole {
  private readonly api = inject(PublicApiConsole);
  private readonly origin = inject(DOCUMENT).location.origin;
  private readonly requests = new Subject<QueryRequest>();

  readonly session = input.required<ShowcaseSession>();
  readonly versions = input<string[]>([]);

  protected readonly request = signal<QueryRequest>(DEFAULT_QUERY);
  protected readonly activePreset = signal<string | null>(null);
  protected readonly result = signal<QueryResult | null>(null);
  protected readonly sending = signal(false);

  protected readonly url = computed(() => {
    const url = this.api.urlFor(this.session(), this.request());
    return url.startsWith('/') ? this.origin + url : url;
  });

  constructor() {
    // switchMap so a slow earlier response never overwrites a newer one.
    this.requests
      .pipe(
        tap(() => this.sending.set(true)),
        switchMap((request) => this.api.send(this.session(), request)),
        takeUntilDestroyed(inject(DestroyRef)),
      )
      .subscribe((result) => {
        this.result.set(result);
        this.sending.set(false);
      });
  }

  protected edit(request: QueryRequest): void {
    this.request.set(request);
    this.activePreset.set(null);
  }

  protected choose(preset: QueryPreset): void {
    this.request.set(applyPreset(DEFAULT_QUERY, preset));
    this.activePreset.set(preset.id);
    this.send();
  }

  protected send(): void {
    this.requests.next(this.request());
  }
}
