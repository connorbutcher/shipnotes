import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export type BadgeTone = 'success' | 'neutral' | 'warning';

@Component({
  selector: 'app-status-badge',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class]': 'tone()' },
  template: `<ng-content />`,
  styleUrl: './status-badge.scss',
})
export class StatusBadge {
  readonly tone = input<BadgeTone>('neutral');
}
