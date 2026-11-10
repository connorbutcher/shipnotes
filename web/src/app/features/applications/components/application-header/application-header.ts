import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Application } from '../../../../core/models/application.model';
import { AppIcon } from '../../../../shared/ui/app-icon/app-icon';

@Component({
  selector: 'app-application-header',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [AppIcon],
  template: `
    <app-icon [name]="application().name" [iconUrl]="application().iconUrl" [size]="48" />
    <div>
      <h1>{{ application().name }}</h1>
      <p class="muted">API key {{ application().apiKeyPrefix }}…</p>
    </div>
  `,
  styles: `
    :host {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    h1 {
      margin: 0;
    }
    p {
      margin: 0.15rem 0 0;
      font-family: var(--font-mono);
      font-size: 0.85rem;
    }
  `,
})
export class ApplicationHeader {
  readonly application = input.required<Application>();
}
