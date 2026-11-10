import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { ButtonDirective, ButtonIcon } from 'primeng/button';
import { categoryOption } from '../../../../core/models/release-note-category';
import { CategoryTag } from '../../../../shared/ui/category-tag/category-tag';
import { SectionForm } from '../../forms/section-form';
import { ImageUploader } from '../rich-text-editor/editor-image-upload';
import { RichTextEditor } from '../rich-text-editor/rich-text-editor';

/** Edits the rich text for one category of change, e.g. everything that was fixed. */
@Component({
  selector: 'app-release-note-section',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ButtonDirective, ButtonIcon, CategoryTag, RichTextEditor],
  templateUrl: './release-note-section.html',
  styleUrl: './release-note-section.scss',
})
export class ReleaseNoteSectionEditor {
  readonly section = input.required<SectionForm>();
  readonly uploadImage = input<ImageUploader | null>(null);
  readonly removed = output();

  protected readonly option = computed(() =>
    categoryOption(this.section().controls.category.value),
  );
}
