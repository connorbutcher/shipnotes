import { ChangeDetectionStrategy, Component, input, model } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MultiSelect } from 'primeng/multiselect';
import {
  RELEASE_NOTE_CATEGORIES,
  ReleaseNoteCategory,
} from '../../../../core/models/release-note-category';

/** A labelled picker for the `category` query option; nothing selected means every category. */
@Component({
  selector: 'app-category-field',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule, MultiSelect],
  templateUrl: './category-field.html',
  styleUrl: './category-field.scss',
})
export class CategoryField {
  readonly inputId = input.required<string>();
  readonly value = model<ReleaseNoteCategory[]>([]);

  protected readonly options = [...RELEASE_NOTE_CATEGORIES];
}
