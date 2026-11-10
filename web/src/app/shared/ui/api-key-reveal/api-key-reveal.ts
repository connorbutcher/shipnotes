import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { CopyButton } from '../copy-button/copy-button';

/** Shows a newly issued API key with a reminder that it will not be shown again. */
@Component({
  selector: 'app-api-key-reveal',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CopyButton],
  template: `
    <div class="banner" role="status">
      <div class="text">
        <strong>Copy your API key now.</strong>
        <span>For security it is stored hashed and won't be shown again.</span>
      </div>
      <code>{{ apiKey() }}</code>
      <div class="actions">
        <app-copy-button [text]="apiKey()" />
        <button type="button" class="button ghost small" (click)="dismissed.emit()">Done</button>
      </div>
    </div>
  `,
  styleUrl: './api-key-reveal.scss',
})
export class ApiKeyReveal {
  readonly apiKey = input.required<string>();
  readonly dismissed = output();
}
