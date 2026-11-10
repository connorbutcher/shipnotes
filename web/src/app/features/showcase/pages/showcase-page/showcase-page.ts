import { ChangeDetectionStrategy, Component, OnInit, computed, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { of } from 'rxjs';
import { Button } from 'primeng/button';
import { Message } from 'primeng/message';
import { ReleaseNotesApi } from '../../../../core/api/release-notes-api';
import { NotificationService } from '../../../../core/notifications/notification.service';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';
import { LoadingState } from '../../../../shared/ui/loading-state/loading-state';
import { QueryConsole } from '../../components/query-console/query-console';
import { ShowcaseHeader } from '../../components/showcase-header/showcase-header';
import { ShowcaseStore } from '../../state/showcase-store';

/** A seeded demo application and a live console for querying its release notes. */
@Component({
  selector: 'app-showcase-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [ShowcaseStore],
  imports: [Button, Message, EmptyState, LoadingState, QueryConsole, ShowcaseHeader],
  templateUrl: './showcase-page.html',
  styleUrl: './showcase-page.scss',
})
export class ShowcasePage implements OnInit {
  private readonly releaseNotesApi = inject(ReleaseNotesApi);
  private readonly confirm = inject(ConfirmService);
  private readonly notifications = inject(NotificationService);
  protected readonly store = inject(ShowcaseStore);

  /** Every demo version, drafts included, to pick from in the console. */
  private readonly notes = rxResource({
    params: () => ({ id: this.store.application()?.id, revision: this.store.revision() }),
    stream: ({ params }) => (params.id ? this.releaseNotesApi.list(params.id) : of([])),
  });

  protected readonly versions = computed(() =>
    this.notes.hasValue() ? this.notes.value().map((n) => n.version) : [],
  );

  ngOnInit(): void {
    this.store.start();
  }

  protected async reset(): Promise<void> {
    const confirmed = await this.confirm.confirm({
      header: 'Reset the demo?',
      message: 'Any changes you made to the demo application and its notes will be lost.',
      acceptLabel: 'Reset demo',
    });
    if (confirmed) {
      this.store.reset().subscribe(() => this.notifications.success('The demo has been reset.'));
    }
  }
}
