import { ChangeDetectionStrategy, Component, computed, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Button } from 'primeng/button';
import { InputNumber } from 'primeng/inputnumber';
import { Select } from 'primeng/select';
import { ToggleSwitch } from 'primeng/toggleswitch';
import { ENDPOINTS, QueryField, QueryRequest, endpointById } from '../../query/query-request';
import { CategoryField } from '../category-field/category-field';
import { VersionField } from '../version-field/version-field';

const LIMIT_MAX = 100;

/** Edits a public API request: the endpoint and the options that endpoint accepts. */
@Component({
  selector: 'app-query-builder',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, Button, InputNumber, Select, ToggleSwitch, VersionField, CategoryField],
  templateUrl: './query-builder.html',
  styleUrl: './query-builder.scss',
})
export class QueryBuilder {
  readonly request = model.required<QueryRequest>();
  readonly versions = input<string[]>([]);
  readonly sending = input(false);
  readonly send = output();

  protected readonly limitMax = LIMIT_MAX;
  protected readonly endpointOptions = ENDPOINTS.map((e) => ({
    value: e.id,
    label: `GET ${e.path}`,
  }));

  protected readonly endpoint = computed(() => endpointById(this.request().endpoint));

  protected has(field: QueryField): boolean {
    return this.endpoint().fields.includes(field);
  }

  protected patch(changes: Partial<QueryRequest>): void {
    this.request.update((request) => ({ ...request, ...changes }));
  }
}
