import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Button } from 'primeng/button';
import { Message } from 'primeng/message';
import { CopyButton } from '../copy-button/copy-button';

/** Shows a newly issued API key with a reminder that it will not be shown again. */
@Component({
  selector: 'app-api-key-reveal',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Message, Button, CopyButton],
  templateUrl: './api-key-reveal.html',
  styleUrl: './api-key-reveal.scss',
})
export class ApiKeyReveal {
  readonly apiKey = input.required<string>();
  readonly dismissed = output();
}
