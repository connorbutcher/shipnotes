import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ReleaseNoteSummary } from '../../../../core/models/release-note.model';
import { StatusBadge } from '../../../../shared/ui/status-badge/status-badge';

@Component({
  selector: 'app-release-note-list-item',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, DatePipe, StatusBadge],
  template: `
    <a [routerLink]="[note().id]">
      <span class="version">{{ note().version }}</span>
      <span class="title">{{ note().title }}</span>
      <span class="badges">
        @if (note().isPreRelease) {
          <app-status-badge tone="warning">Pre-release</app-status-badge>
        }
        @if (note().isPublished) {
          <app-status-badge tone="success"
            >Published {{ note().publishedAt | date: 'mediumDate' }}</app-status-badge
          >
        } @else {
          <app-status-badge>Draft</app-status-badge>
        }
      </span>
    </a>
  `,
  styleUrl: './release-note-list-item.scss',
})
export class ReleaseNoteListItem {
  readonly note = input.required<ReleaseNoteSummary>();
}
