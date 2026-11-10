import { FormArray, FormControl, FormGroup } from '@angular/forms';
import {
  RELEASE_NOTE_CATEGORIES,
  ReleaseNoteCategory,
} from '../../../core/models/release-note-category';
import { ReleaseNoteSection } from '../../../core/models/release-note.model';
import { normalizeEditorHtml } from '../../../shared/utils/editor-html';

/** Form controls for one category section of a release note. */
export type SectionForm = FormGroup<{
  category: FormControl<ReleaseNoteCategory>;
  contentHtml: FormControl<string>;
}>;

export type SectionsForm = FormArray<SectionForm>;

export function createSectionForm(section: ReleaseNoteSection): SectionForm {
  return new FormGroup({
    category: new FormControl(section.category, { nonNullable: true }),
    contentHtml: new FormControl(section.contentHtml, { nonNullable: true }),
  });
}

function orderOf(category: ReleaseNoteCategory): number {
  return RELEASE_NOTE_CATEGORIES.findIndex((c) => c.value === category);
}

/** Adds a section in display order, so the editor reads like the published note. Returns its index. */
export function addSection(sections: SectionsForm, category: ReleaseNoteCategory): number {
  const index = sections.controls.findIndex(
    (s) => orderOf(s.controls.category.value) > orderOf(category),
  );
  const at = index === -1 ? sections.length : index;
  sections.insert(at, createSectionForm({ category, contentHtml: '' }));
  sections.markAsDirty();
  return at;
}

/** Replaces every section, e.g. when an existing note is loaded. */
export function setSections(sections: SectionsForm, values: readonly ReleaseNoteSection[]): void {
  sections.clear({ emitEvent: false });
  for (const value of values) {
    sections.push(createSectionForm(value), { emitEvent: false });
  }
  sections.updateValueAndValidity();
}

/** Categories that don't have a section yet, in display order. */
export function unusedCategories(used: readonly ReleaseNoteCategory[]): ReleaseNoteCategory[] {
  return RELEASE_NOTE_CATEGORIES.map((c) => c.value).filter((c) => !used.includes(c));
}

/** The sections to save: editor HTML normalised, and sections left empty dropped. */
export function toSectionRequests(values: readonly ReleaseNoteSection[]): ReleaseNoteSection[] {
  return values
    .map((s) => ({ category: s.category, contentHtml: normalizeEditorHtml(s.contentHtml) }))
    .filter((s) => s.contentHtml !== '');
}
