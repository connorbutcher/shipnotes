import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { ApplicationsStore } from '../../../../core/state/applications-store';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-welcome-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, ButtonDirective, ButtonIcon, ButtonLabel, EmptyState],
  templateUrl: './welcome-page.html',
  styleUrl: './welcome-page.scss',
})
export class WelcomePage {
  protected readonly store = inject(ApplicationsStore);
}
