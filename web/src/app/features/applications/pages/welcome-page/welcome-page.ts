import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApplicationsStore } from '../../../../core/state/applications-store';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-welcome-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, EmptyState],
  template: `
    @if (store.isEmpty()) {
      <app-empty-state
        heading="Welcome to ShipNotes"
        message="Create an application to start writing release notes your apps can fetch with an API key."
      >
        <a class="button" routerLink="/applications/new">Create your first application</a>
      </app-empty-state>
    } @else {
      <app-empty-state
        heading="Select an application"
        message="Pick an application from the side panel to manage its release notes."
      />
    }
  `,
})
export class WelcomePage {
  protected readonly store = inject(ApplicationsStore);
}
