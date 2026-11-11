import { byCategoryOrder } from '../../../core/models/release-note-category';
import { ReleaseNoteContent } from '../../../core/models/release-note.model';
import { normalizeEditorHtml } from '../../../shared/utils/editor-html';

/**
 * The editor's content as the API expects it: HTML normalised, sections left empty dropped,
 * and sections in display order.
 */
export function toContentRequest(content: ReleaseNoteContent): ReleaseNoteContent {
  const sections = content.sections
    .map((s) => ({ category: s.category, contentHtml: normalizeEditorHtml(s.contentHtml) }))
    .filter((s) => s.contentHtml !== '');

  return {
    summaryHtml: normalizeEditorHtml(content.summaryHtml),
    sections: byCategoryOrder(sections, (s) => s.category),
  };
}
