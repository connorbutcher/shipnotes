import { ChangeDetectionStrategy, Component, computed, input, linkedSignal } from '@angular/core';
import { Avatar } from 'primeng/avatar';

/** An application's icon, falling back to its initials when no icon URL is set or it fails to load. */
@Component({
  selector: 'app-icon',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Avatar],
  templateUrl: './app-icon.html',
  styleUrl: './app-icon.scss',
})
export class AppIcon {
  readonly name = input.required<string>();
  readonly iconUrl = input<string | null>(null);
  readonly size = input(32);

  protected readonly failed = linkedSignal({ source: this.iconUrl, computation: () => false });
  protected readonly showImage = computed(() => !!this.iconUrl() && !this.failed());

  protected readonly avatarStyle = computed(() => ({
    width: `${this.size()}px`,
    height: `${this.size()}px`,
    fontSize: `${this.size() * 0.4}px`,
  }));

  protected readonly initials = computed(() =>
    this.name()
      .split(/\s+/)
      .filter(Boolean)
      .slice(0, 2)
      .map((word) => word[0].toUpperCase())
      .join(''),
  );
}
