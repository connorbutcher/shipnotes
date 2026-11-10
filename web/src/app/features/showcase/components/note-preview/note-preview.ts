import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Tag } from 'primeng/tag';
import { PublicReleaseNote } from '../../../../core/models/public-release-note.model';

/**
 * One release note rendered the way a consuming app would show it. The HTML was sanitised by
 * the API when the note was saved; Angular sanitises it again when binding to innerHTML.
 */
@Component({
  selector: 'app-note-preview',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [DatePipe, Tag],
  templateUrl: './note-preview.html',
  styleUrl: './note-preview.scss',
})
export class NotePreview {
  readonly note = input.required<PublicReleaseNote>();
}
