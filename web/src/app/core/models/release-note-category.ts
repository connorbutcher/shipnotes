/**
 * The kinds of change the editor can mark a block of a note as. These only drive the editor's
 * section picker and the classes it writes into the note's HTML; the API stores that HTML as is.
 */
export type ReleaseNoteCategory =
  'breaking' | 'added' | 'changed' | 'deprecated' | 'removed' | 'fixed' | 'security';

export interface ReleaseNoteCategoryOption {
  value: ReleaseNoteCategory;
  label: string;
}

/** Every category, in the order the section picker lists them. */
export const RELEASE_NOTE_CATEGORIES: readonly ReleaseNoteCategoryOption[] = [
  { value: 'breaking', label: 'Breaking changes' },
  { value: 'added', label: 'Added' },
  { value: 'changed', label: 'Changed' },
  { value: 'deprecated', label: 'Deprecated' },
  { value: 'removed', label: 'Removed' },
  { value: 'fixed', label: 'Fixed' },
  { value: 'security', label: 'Security' },
];

export function categoryOption(category: ReleaseNoteCategory): ReleaseNoteCategoryOption {
  return RELEASE_NOTE_CATEGORIES.find((c) => c.value === category) ?? RELEASE_NOTE_CATEGORIES[0];
}

/** The category with this value, or null when it isn't one. */
export function parseCategory(value: string | null | undefined): ReleaseNoteCategory | null {
  return RELEASE_NOTE_CATEGORIES.find((c) => c.value === value)?.value ?? null;
}
