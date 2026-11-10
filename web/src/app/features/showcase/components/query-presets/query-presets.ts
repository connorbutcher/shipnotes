import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { Button } from 'primeng/button';
import { QUERY_PRESETS, QueryPreset } from '../../query/query-presets';

/** One-click example questions, including the error responses a consuming app should handle. */
@Component({
  selector: 'app-query-presets',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Button],
  templateUrl: './query-presets.html',
  styleUrl: './query-presets.scss',
})
export class QueryPresets {
  readonly activeId = input<string | null>(null);
  readonly selected = output<QueryPreset>();

  protected readonly presets = QUERY_PRESETS;
}
