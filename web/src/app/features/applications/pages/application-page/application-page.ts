import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ApplicationsStore } from '../../../../core/state/applications-store';
import { ApiKeyReveal } from '../../../../shared/ui/api-key-reveal/api-key-reveal';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';
import { ApplicationHeader } from '../../components/application-header/application-header';
import { ApplicationTabs } from '../../components/application-tabs/application-tabs';

/** Layout for a selected application: header, one-time API key banner, tabs and the active section. */
@Component({
  selector: 'app-application-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink, ApplicationHeader, ApplicationTabs, ApiKeyReveal, EmptyState],
  template: `
    @if (application(); as application) {
      <div class="page">
        <app-application-header [application]="application" />
        @if (issuedKey(); as key) {
          <app-api-key-reveal [apiKey]="key" (dismissed)="store.clearIssuedKey()" />
        }
        <app-application-tabs />
        <router-outlet />
      </div>
    } @else if (!store.loading()) {
      <app-empty-state heading="Application not found" message="It may have been deleted.">
        <a class="button secondary" routerLink="/">Back</a>
      </app-empty-state>
    }
  `,
  styles: `
    .page {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
      max-width: 60rem;
    }
  `,
})
export class ApplicationPage {
  protected readonly store = inject(ApplicationsStore);

  readonly applicationId = input.required<string>();

  protected readonly application = computed(() =>
    this.store.applications().find((a) => a.id === this.applicationId()),
  );

  protected readonly issuedKey = computed(() => {
    const issued = this.store.lastIssuedKey();
    return issued?.applicationId === this.applicationId() ? issued.apiKey : null;
  });
}
