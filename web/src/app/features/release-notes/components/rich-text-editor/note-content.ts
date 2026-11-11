import type Quill from 'quill';
import { Delta } from 'quill';
import {
  byCategoryOrder,
  ReleaseNoteCategory,
} from '../../../../core/models/release-note-category';
import { ReleaseNoteContent } from '../../../../core/models/release-note.model';
import { CATEGORY_CLASS_PATTERN, CATEGORY_FORMAT, lineCategory } from './category-format';

/** A run of consecutive lines in the same section (or in none), by document position. */
interface LineRun {
  category: ReleaseNoteCategory | null;
  index: number;
  length: number;
}

/**
 * Splits the editor's single document into the note's summary and category sections.
 * Lines outside any section form the summary. A category used in more than one place
 * is joined into one section, since a note has at most one section per category.
 */
export function readNoteContent(quill: Quill): ReleaseNoteContent {
  let summaryHtml = '';
  const sections = new Map<ReleaseNoteCategory, string>();

  for (const run of lineRuns(quill.getContents())) {
    const html = withoutCategoryClasses(quill.getSemanticHTML(run.index, run.length));
    if (run.category === null) {
      summaryHtml += html;
    } else {
      sections.set(run.category, (sections.get(run.category) ?? '') + html);
    }
  }

  return {
    summaryHtml,
    sections: [...sections].map(([category, contentHtml]) => ({ category, contentHtml })),
  };
}

/** Builds the editor document for a note: the summary, then each section in display order. */
export function toNoteDelta(quill: Quill, content: ReleaseNoteContent): Delta {
  const parts = [
    { category: null, html: content.summaryHtml },
    ...byCategoryOrder(content.sections, (s) => s.category).map((s) => ({
      category: s.category,
      html: s.contentHtml,
    })),
  ];

  return parts
    .filter((part) => part.html.trim() !== '')
    .reduce(
      (delta, part) =>
        delta.concat(inCategory(quill.clipboard.convert({ html: part.html }), part.category)),
      new Delta(),
    );
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

/** The section classes are editor markup: the API wraps each saved section in its own element. */
function withoutCategoryClasses(html: string): string {
  if (!html.includes('note-section--')) {
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
