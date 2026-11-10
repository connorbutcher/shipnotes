import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { Tag } from 'primeng/tag';
import { ReleaseNoteCategory, categoryOption } from '../../../core/models/release-note-category';

/** A coloured tag naming a release note category, e.g. "Breaking changes". */
@Component({
  selector: 'app-category-tag',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Tag],
  templateUrl: './category-tag.html',
  styleUrl: './category-tag.scss',
})
export class CategoryTag {
  readonly category = input.required<ReleaseNoteCategory>();

  protected readonly option = computed(() => categoryOption(this.category()));
}
