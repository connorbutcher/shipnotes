import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { ReleaseNoteCategory, categoryOption } from '../../../../core/models/release-note-category';

/** One button per category that can still be added to the note. */
@Component({
  selector: 'app-category-picker',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ButtonDirective, ButtonIcon, ButtonLabel],
  templateUrl: './category-picker.html',
  styleUrl: './category-picker.scss',
})
export class CategoryPicker {
  readonly available = input.required<readonly ReleaseNoteCategory[]>();
  readonly picked = output<ReleaseNoteCategory>();

  protected readonly options = computed(() => this.available().map(categoryOption));
}
