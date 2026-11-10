import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Tag } from 'primeng/tag';
import { PublicReleaseNote } from '../../../../core/models/public-release-note.model';
import { CategoryTag } from '../../../../shared/ui/category-tag/category-tag';

/**
 * One release note rendered the way a consuming app would show it: the summary, then a
 * heading and content for each category section. The HTML was sanitised by
 * the API when the note was saved; Angular sanitises it again when binding to innerHTML.
 */
@Component({
  selector: 'app-note-preview',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, Tag, CategoryTag],
  templateUrl: './note-preview.html',
  styleUrl: './note-preview.scss',
})
export class NotePreview {
  readonly note = input.required<PublicReleaseNote>();
}
