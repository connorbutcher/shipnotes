import { Clipboard } from '@angular/cdk/clipboard';
import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { Button } from 'primeng/button';

const RESET_MS = 2000;

/** Copies text to the clipboard and briefly confirms it. */
@Component({
  selector: 'app-copy-button',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Button],
  templateUrl: './copy-button.html',
  styleUrl: './copy-button.scss',
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
