import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { ReleaseNotesApi } from '../../../../core/api/release-notes-api';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';
import { ReleaseNoteListItem } from '../../components/release-note-list-item/release-note-list-item';

@Component({
  selector: 'app-release-notes-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, EmptyState, ReleaseNoteListItem],
  template: `
    <div class="toolbar">
      <h2>Release notes</h2>
      <a class="button" routerLink="new">+ New release notes</a>
    </div>

    @if (notes.isLoading()) {
      <p class="muted">Loading…</p>
    } @else if (notes.hasValue()) {
      @if (notes.value().length) {
        <ul class="card list">
          @for (note of notes.value(); track note.id) {
            <li><app-release-note-list-item [note]="note" /></li>
          }
        </ul>
      } @else {
        <app-empty-state
          heading="No release notes yet"
          message="Write notes for your first version to make them available to your app."
        >
          <a class="button" routerLink="new">Write release notes</a>
        </app-empty-state>
      }
    }
  `,
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
