import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { map, startWith, switchMap } from 'rxjs';
import { ReleaseNoteCategory } from '../../../../core/models/release-note-category';
import { SectionsForm, addSection, unusedCategories } from '../../forms/section-form';
import { CategoryPicker } from '../category-picker/category-picker';
import { ReleaseNoteSectionEditor } from '../release-note-section/release-note-section';
import { ImageUploader } from '../rich-text-editor/editor-image-upload';

/** The note's category sections, in display order, and buttons to add the categories not used yet. */
@Component({
  selector: 'app-release-note-sections',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CategoryPicker, ReleaseNoteSectionEditor],
  templateUrl: './release-note-sections.html',
  styleUrl: './release-note-sections.scss',
})
export class ReleaseNoteSections {
  readonly sections = input.required<SectionsForm>();
  readonly uploadImage = input<ImageUploader | null>(null);

  private readonly used = toSignal(
    toObservable(this.sections).pipe(
      switchMap((sections) =>
        sections.valueChanges.pipe(
          startWith(null),
          map(() => sections.controls.map((s) => s.controls.category.value)),
        ),
      ),
    ),
    { initialValue: [] as ReleaseNoteCategory[] },
  );

  protected readonly available = computed(() => unusedCategories(this.used()));

  protected add(category: ReleaseNoteCategory): void {
    addSection(this.sections(), category);
  }

  protected remove(index: number): void {
    this.sections().removeAt(index);
    this.sections().markAsDirty();
  }
}
