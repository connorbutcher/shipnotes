import { ChangeDetectionStrategy, Component, computed, input, linkedSignal } from '@angular/core';

/** An application's icon, falling back to its initials when no icon URL is set or it fails to load. */
@Component({
  selector: 'app-icon',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[style.--icon-size.px]': 'size()' },
  template: `
    @if (iconUrl() && !failed()) {
      <img [src]="iconUrl()" [alt]="''" (error)="failed.set(true)" />
    } @else {
      <span class="initials" aria-hidden="true">{{ initials() }}</span>
    }
  `,
  styleUrl: './app-icon.scss',
})
export class AppIcon {
  readonly name = input.required<string>();
  readonly iconUrl = input<string | null>(null);
  readonly size = input(32);

  protected readonly failed = linkedSignal({ source: this.iconUrl, computation: () => false });

  protected readonly initials = computed(() =>
    this.name()
      .split(/\s+/)
      .filter(Boolean)
      .slice(0, 2)
      .map((word) => word[0].toUpperCase())
      .join(''),
  );
}
