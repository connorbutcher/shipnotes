import { ReleaseNoteCategory } from './release-note-category';

/** One category of change within a published note. */
export interface PublicReleaseNoteSection {
  category: ReleaseNoteCategory;
  /** Display name, e.g. "Breaking changes" or "Fixed". */
  label: string;
  contentHtml: string;
}

/** A release note as consuming applications receive it from the public API. */
export interface PublicReleaseNote {
  version: string;
  isPreRelease: boolean;
  title: string;
  summaryHtml: string;
  /** In display order; narrowed to the requested categories when the query filters by category. */
  sections: PublicReleaseNoteSection[];
  /** The summary and sections composed into one HTML fragment, for apps that don't need the structure. */
  contentHtml: string;
  publishedAt: string;
}

/** Application details from the public API. */
export interface PublicApplication {
  name: string;
  iconUrl: string | null;
}
