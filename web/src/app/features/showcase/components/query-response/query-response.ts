import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primeng/tabs';
import { Tag } from 'primeng/tag';
import { CopyButton } from '../../../../shared/ui/copy-button/copy-button';
import { QueryResult, statusSeverity } from '../../query/query-result';
import { RenderedResponse } from '../rendered-response/rendered-response';

/** A console response: status and timing, then the rendered notes and the raw JSON. */
@Component({
  selector: 'app-query-response',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Tabs, TabList, Tab, TabPanels, TabPanel, Tag, CopyButton, RenderedResponse],
  templateUrl: './query-response.html',
  styleUrl: './query-response.scss',
})
export class QueryResponse {
  readonly result = input.required<QueryResult>();

  protected readonly severity = computed(() => statusSeverity(this.result().status));
  protected readonly json = computed(() => JSON.stringify(this.result().body, null, 2) ?? '');
}
