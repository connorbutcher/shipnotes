import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApplicationsStore } from '../../core/state/applications-store';
import { ApplicationNavItem } from '../application-nav-item/application-nav-item';

@Component({
  selector: 'app-side-panel',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, ApplicationNavItem],
  template: `
    <a class="brand" routerLink="/">ShipNotes</a>

    <div class="section-header">
      <span>Applications</span>
      <a class="button small" routerLink="/applications/new" aria-label="Add application">+ New</a>
    </div>

    <nav aria-label="Applications">
      @if (store.loading() && store.applications().length === 0) {
        <p class="hint">Loading…</p>
      } @else {
        @for (application of store.applications(); track application.id) {
          <app-application-nav-item [application]="application" />
        } @empty {
          <p class="hint">No applications yet.</p>
        }
      }
    </nav>
  `,
  styleUrl: './side-panel.scss',
})
export class SidePanel {
  protected readonly store = inject(ApplicationsStore);
}
