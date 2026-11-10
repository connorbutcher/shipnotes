import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface Tab {
  label: string;
  path: string;
}

@Component({
  selector: 'app-application-tabs',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav aria-label="Application sections">
      @for (tab of tabs; track tab.path) {
        <a [routerLink]="tab.path" routerLinkActive="active">{{ tab.label }}</a>
      }
    </nav>
  `,
  styleUrl: './application-tabs.scss',
})
export class ApplicationTabs {
  protected readonly tabs: readonly Tab[] = [
    { label: 'Release notes', path: 'release-notes' },
    { label: 'Integration', path: 'integration' },
    { label: 'Settings', path: 'settings' },
  ];
}
