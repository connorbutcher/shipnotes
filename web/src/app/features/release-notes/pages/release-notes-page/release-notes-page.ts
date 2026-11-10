import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { Card } from 'primeng/card';
import { ReleaseNotesApi } from '../../../../core/api/release-notes-api';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';
import { LoadingState } from '../../../../shared/ui/loading-state/loading-state';
import { ReleaseNotesTable } from '../../components/release-notes-table/release-notes-table';

@Component({
  selector: 'app-release-notes-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterLink,
    ButtonDirective,
    ButtonIcon,
    ButtonLabel,
    Card,
    EmptyState,
    LoadingState,
    ReleaseNotesTable,
  ],
  templateUrl: './release-notes-page.html',
  styleUrl: './release-notes-page.scss',
})
export class ReleaseNotesPage {
  private readonly api = inject(ReleaseNotesApi);

  readonly applicationId = input.required<string>();

  protected readonly notes = rxResource({
    params: () => this.applicationId(),
    stream: ({ params }) => this.api.list(params),
  });
}
