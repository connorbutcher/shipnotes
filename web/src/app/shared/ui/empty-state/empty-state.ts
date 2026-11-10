import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/** Centred placeholder for pages with nothing to show yet. */
@Component({
  selector: 'app-empty-state',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss',
})
export class EmptyState {
  readonly heading = input.required<string>();
  readonly message = input('');
  /** A PrimeIcons class, e.g. `pi-inbox`. */
  readonly icon = input('pi-inbox');
}
