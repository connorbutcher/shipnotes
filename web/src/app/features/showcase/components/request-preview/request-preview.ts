import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { Button } from 'primeng/button';
import { CopyButton } from '../../../../shared/ui/copy-button/copy-button';
import { buildCurlCommand, maskApiKey } from '../../query/curl-command';

/** The exact HTTP request the console will send, with a curl command to run it yourself. */
@Component({
  selector: 'app-request-preview',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Button, CopyButton],
  templateUrl: './request-preview.html',
  styleUrl: './request-preview.scss',
})
export class RequestPreview {
  readonly url = input.required<string>();
  readonly apiKey = input.required<string>();
  readonly sendApiKey = input(true);

  protected readonly revealed = signal(false);

  protected readonly shownKey = computed(() =>
    this.revealed() ? this.apiKey() : maskApiKey(this.apiKey()),
  );

  protected readonly curl = computed(() =>
    buildCurlCommand(this.url(), this.sendApiKey() ? this.apiKey() : null),
  );
}
