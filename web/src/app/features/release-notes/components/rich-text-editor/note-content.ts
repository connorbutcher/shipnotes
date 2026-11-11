import type Quill from 'quill';
import { Delta } from 'quill';
import {
  ReleaseNoteCategory,
  categoryOption,
  parseCategory,
} from '../../../../core/models/release-note-category';
import { normalizeEditorHtml } from '../../../../shared/utils/editor-html';
import { CATEGORY_CLASS_PATTERN, CATEGORY_FORMAT, lineCategory } from './category-format';

/** The class on every section block in a saved note, and on its heading. */
const SECTION_CLASS = 'note-section';
const TITLE_CLASS = 'note-section__title';

/** A run of consecutive lines in the same section (or in none), by document position. */
interface LineRun {
  category: ReleaseNoteCategory | null;
  index: number;
  length: number;
}

/**
 * The editor's document as the note's HTML. Each run of lines in a section is wrapped in
 * `<section class="note-section note-section--fixed">` with an
 * `<h2 class="note-section__title">` heading, so apps can style each kind of change.
 * Everything else is written as is, in the order it was written.
 */
export function readNoteHtml(quill: Quill): string {
  return lineRuns(quill.getContents())
    .map((run) => {
      const html = normalizeEditorHtml(
        withoutCategoryClasses(quill.getSemanticHTML(run.index, run.length)),
      );
      return run.category && html ? sectionHtml(run.category, html) : html;
    })
    .join('');
}

/** Builds the editor document for a note's HTML: each section's lines are put back in its category. */
export function toNoteDelta(quill: Quill, html: string): Delta {
  const body = new DOMParser().parseFromString(html, 'text/html').body;
  let delta = new Delta();
  let loose = '';
  const flushLoose = () => {
    if (loose.trim() !== '') {
      delta = delta.concat(inCategory(quill.clipboard.convert({ html: loose }), null));
    }
    loose = '';
  };

  for (const node of Array.from(body.childNodes)) {
    const category = node instanceof Element ? sectionCategory(node) : null;
    if (category && node instanceof Element) {
      flushLoose();
      node.querySelectorAll(`:scope > .${TITLE_CLASS}`).forEach((title) => title.remove());
      delta = delta.concat(inCategory(quill.clipboard.convert({ html: node.innerHTML }), category));
    } else {
      loose += node instanceof Element ? node.outerHTML : (node.textContent ?? '');
    }
  }
  flushLoose();

  return delta;
}

function sectionHtml(category: ReleaseNoteCategory, html: string): string {
  return (
    `<section class="${SECTION_CLASS} ${SECTION_CLASS}--${category}">` +
    `<h2 class="${TITLE_CLASS}">${categoryOption(category).label}</h2>${html}</section>`
  );
}

/** The category of a saved section block, or null for any other element. */
function sectionCategory(element: Element): ReleaseNoteCategory | null {
  if (element.tagName !== 'SECTION' || !element.classList.contains(SECTION_CLASS)) {
    return null;
  }
  const modifier = [...element.classList].find((name) => name.startsWith(`${SECTION_CLASS}--`));
  return parseCategory(modifier?.slice(SECTION_CLASS.length + 2));
}

function lineRuns(document: Delta): LineRun[] {
  const runs: LineRun[] = [];
  let index = 0;

  document.eachLine((line, formats) => {
    const category = lineCategory(formats);
    const length = line.length() + 1;
    const last = runs.at(-1);
    if (last?.category === category) {
      last.length += length;
    } else {
      runs.push({ category, index, length });
    }
    index += length;
  });

  return runs;
}

/** Puts every line of the delta in the category (or takes it out of any, for null). */
function inCategory(delta: Delta, category: ReleaseNoteCategory | null): Delta {
  const result = new Delta();
  delta.eachLine((line, formats) => {
    line.ops.forEach((op) => result.push(op));
    const lineFormats = { ...formats };
    delete lineFormats[CATEGORY_FORMAT];
    result.insert('\n', category ? { ...lineFormats, [CATEGORY_FORMAT]: category } : lineFormats);
  });
  return result;
}

/** The per-line classes are editor markup: the saved note wraps each section in one element instead. */
function withoutCategoryClasses(html: string): string {
  if (!html.includes(`${SECTION_CLASS}--`)) {
    return html;
  }

  const body = new DOMParser().parseFromString(html, 'text/html').body;
  body.querySelectorAll('[class]').forEach((element) => {
    [...element.classList]
      .filter((name) => CATEGORY_CLASS_PATTERN.test(name))
      .forEach((name) => element.classList.remove(name));
    if (element.classList.length === 0) {
      element.removeAttribute('class');
    }
  });
  return body.innerHTML;
}
