import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ProgressSpinner } from 'primeng/progressspinner';

/** Spinner with an accessible label, shown while a page's data loads. */
@Component({
  selector: 'app-loading-state',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ProgressSpinner],
  templateUrl: './loading-state.html',
  styleUrl: './loading-state.scss',
})
export class LoadingState {
  readonly label = input('Loading…');
}
