import Quill from 'quill';
import type { ClassAttributor as ClassAttributorType, Scope as ScopeType } from 'parchment';
import type Keyboard from 'quill/modules/keyboard';
import {
  RELEASE_NOTE_CATEGORIES,
  ReleaseNoteCategory,
  parseCategory,
} from '../../../../core/models/release-note-category';

/** The Quill line format that puts a line in a category section, e.g. `{ category: 'fixed' }`. */
export const CATEGORY_FORMAT = 'category';

/**
 * Class prefix for lines in a section. Quill adds a dash before the value, giving
 * `note-section--added`: the same modifier class the saved note puts on each section.
 */
const CATEGORY_CLASS_KEY = 'note-section-';

/** Matches the class Quill adds to a line in a section. */
export const CATEGORY_CLASS_PATTERN = /^note-section--[a-z]+$/;

type KeyBinding = Exclude<Parameters<Keyboard['addBinding']>[0], string | number>;

interface Parchment {
  ClassAttributor: typeof ClassAttributorType;
  Scope: typeof ScopeType;
}

let registered = false;

/**
 * Registers the `category` line format with Quill. A block-level class attributor, like Quill's
 * own `align`, so any kind of line (paragraph, heading, list item, quote) can sit in a section,
 * and undo, copy and paste keep it. Safe to call more than once.
 */
export function registerCategoryFormat(): void {
  if (registered) {
    return;
  }

  const { ClassAttributor, Scope } = Quill.import('parchment') as Parchment;
  const attributor = new ClassAttributor(CATEGORY_FORMAT, CATEGORY_CLASS_KEY, {
    scope: Scope.BLOCK,
    whitelist: RELEASE_NOTE_CATEGORIES.map((c) => c.value),
  });
  Quill.register(attributor, true);
  registered = true;
}

/** The category a line's formats put it in, or null for summary text. */
export function lineCategory(formats: Record<string, unknown>): ReleaseNoteCategory | null {
  const value = formats[CATEGORY_FORMAT];
  return typeof value === 'string' ? parseCategory(value) : null;
}

/** Pressing Enter on an empty line in a section ends the section, as Quill does for lists. */
export const exitSectionOnEmptyEnter: KeyBinding = {
  key: 'Enter',
  collapsed: true,
  empty: true,
  format: [CATEGORY_FORMAT],
  handler(range) {
    this.quill.formatLine(range.index, 1, CATEGORY_FORMAT, false, Quill.sources.USER);
    return false;
  },
};
