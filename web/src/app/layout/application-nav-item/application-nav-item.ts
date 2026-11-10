import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Application } from '../../core/models/application.model';
import { AppIcon } from '../../shared/ui/app-icon/app-icon';

@Component({
  selector: 'app-application-nav-item',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, RouterLinkActive, AppIcon],
  template: `
    <a [routerLink]="['/applications', application().id]" routerLinkActive="active">
      <app-icon [name]="application().name" [iconUrl]="application().iconUrl" [size]="28" />
      <span class="name">{{ application().name }}</span>
    </a>
  `,
  styleUrl: './application-nav-item.scss',
})
export class ApplicationNavItem {
  readonly application = input.required<Application>();
}
