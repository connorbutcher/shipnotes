import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { ApplicationsStore } from '../../../../core/state/applications-store';
import { ApiKeyReveal } from '../../../../shared/ui/api-key-reveal/api-key-reveal';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';
import { ApplicationHeader } from '../../components/application-header/application-header';
import { ApplicationTabs } from '../../components/application-tabs/application-tabs';

/** Layout for a selected application: header, one-time API key banner, tabs and the active section. */
@Component({
  selector: 'app-application-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    RouterOutlet,
    RouterLink,
    ButtonDirective,
    ButtonIcon,
    ButtonLabel,
    ApplicationHeader,
    ApplicationTabs,
    ApiKeyReveal,
    EmptyState,
  ],
  templateUrl: './application-page.html',
  styleUrl: './application-page.scss',
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
