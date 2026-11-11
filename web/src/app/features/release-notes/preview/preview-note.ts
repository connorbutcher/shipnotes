import { byCategoryOrder, categoryOption } from '../../../core/models/release-note-category';
import {
  PublicReleaseNote,
  PublicReleaseNoteSection,
} from '../../../core/models/public-release-note.model';
import { SaveReleaseNoteRequest } from '../../../core/models/release-note.model';
import { isPreRelease } from '../../../shared/validators/semver.validator';
import { toPreviewHtml } from './preview-html';

/**
 * Maps the editor's unsaved values to the shape consuming apps receive from the public API,
 * so the preview renders exactly what will be served once the note is published.
 */
export function toPreviewNote(
  draft: SaveReleaseNoteRequest,
  publishedAt: string | null,
  now: Date = new Date(),
): PublicReleaseNote {
  const version = draft.version.trim();
  const summaryHtml = toPreviewHtml(draft.summaryHtml);
  const sections = toPreviewSections(draft.sections);
  return {
    version,
    isPreRelease: isPreRelease(version),
    title: draft.title.trim(),
    summaryHtml,
    sections,
    contentHtml: composeHtml(summaryHtml, sections),
    publishedAt: publishedAt ?? now.toISOString(),
  };
}

/** Whether the draft has anything worth previewing yet. */
export function hasPreviewContent(note: PublicReleaseNote): boolean {
  return !!(note.version || note.title || note.contentHtml);
}

/** Sections in display order with their labels; empty ones are dropped, as the API does. */
function toPreviewSections(
  sections: SaveReleaseNoteRequest['sections'],
): PublicReleaseNoteSection[] {
  return byCategoryOrder(sections, (s) => s.category)
    .map((s) => ({
      category: s.category,
      label: categoryOption(s.category).label,
      contentHtml: toPreviewHtml(s.contentHtml),
    }))
    .filter((s) => s.contentHtml !== '');
}

/** The single HTML fragment the API composes for apps that don't use the structure. */
function composeHtml(summaryHtml: string, sections: readonly PublicReleaseNoteSection[]): string {
  return (
    summaryHtml +
    sections
      .map(
        (s) =>
          `<section class="note-section note-section--${s.category}" data-category="${s.category}">` +
          `<h2 class="note-section__title">${s.label}</h2>${s.contentHtml}</section>`,
      )
      .join('')
  );
}
