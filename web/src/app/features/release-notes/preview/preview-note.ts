import { PublicReleaseNote } from '../../../core/models/public-release-note.model';
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
  return {
    version,
    isPreRelease: isPreRelease(version),
    title: draft.title.trim(),
    contentHtml: toPreviewHtml(draft.contentHtml),
    publishedAt: publishedAt ?? now.toISOString(),
  };
}

/** Whether the draft has anything worth previewing yet. */
export function hasPreviewContent(note: PublicReleaseNote): boolean {
  return !!(note.version || note.title || note.contentHtml);
}
