import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Message } from 'primeng/message';
import { AppIcon } from '../../../../shared/ui/app-icon/app-icon';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';
import { QueryResult, interpretResult } from '../../query/query-result';
import { NotePreview } from '../note-preview/note-preview';

/** Shows a response the way a consuming app would render it: notes with images, or the error. */
@Component({
  selector: 'app-rendered-response',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Message, AppIcon, EmptyState, NotePreview],
  templateUrl: './rendered-response.html',
  styleUrl: './rendered-response.scss',
})
export class RenderedResponse {
  readonly result = input.required<QueryResult>();

  protected readonly view = computed(() => interpretResult(this.result()));
}
