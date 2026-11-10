import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Tag } from 'primeng/tag';
import { PublicReleaseNote } from '../../../../core/models/public-release-note.model';
import { EmptyState } from '../../../../shared/ui/empty-state/empty-state';
import { NotePreview } from '../../../../shared/ui/note-preview/note-preview';
import { hasPreviewContent } from '../../preview/preview-note';

/** Live preview of the note being edited, rendered as consuming apps will show it. */
@Component({
  selector: 'app-release-note-preview',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Tag, EmptyState, NotePreview],
  templateUrl: './release-note-preview.html',
  styleUrl: './release-note-preview.scss',
})
export class ReleaseNotePreview {
  readonly note = input.required<PublicReleaseNote>();
  /** Whether the note will be visible to consuming apps once saved. */
  readonly published = input(false);

  protected readonly hasContent = computed(() => hasPreviewContent(this.note()));
}
