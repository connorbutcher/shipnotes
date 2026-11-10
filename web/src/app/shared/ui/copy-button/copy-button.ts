import { Clipboard } from '@angular/cdk/clipboard';
import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';

const RESET_MS = 2000;

@Component({
  selector: 'app-copy-button',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<button type="button" class="button secondary small" (click)="copy()">
    {{ copied() ? 'Copied' : label() }}
  </button>`,
})
export class CopyButton {
  private readonly clipboard = inject(Clipboard);

  readonly text = input.required<string>();
  readonly label = input('Copy');

  protected readonly copied = signal(false);

  protected copy(): void {
    if (this.clipboard.copy(this.text())) {
      this.copied.set(true);
      setTimeout(() => this.copied.set(false), RESET_MS);
    }
  }
}
