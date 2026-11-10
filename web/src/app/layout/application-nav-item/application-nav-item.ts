import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Ripple } from 'primeng/ripple';
import { Application } from '../../core/models/application.model';
import { AppIcon } from '../../shared/ui/app-icon/app-icon';

/** One application entry in the side panel menu. */
@Component({
  selector: 'app-application-nav-item',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, RouterLinkActive, Ripple, AppIcon],
  templateUrl: './application-nav-item.html',
  styleUrl: './application-nav-item.scss',
})
export class ApplicationNavItem {
  readonly application = input.required<Application>();
}
