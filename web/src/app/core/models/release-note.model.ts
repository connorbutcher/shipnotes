import { ReleaseNoteCategory } from './release-note-category';

export interface ReleaseNoteSummary {
  id: string;
  version: string;
  isPreRelease: boolean;
  title: string;
  /** Categories the note has sections for, in display order. */
  categories: ReleaseNoteCategory[];
  isPublished: boolean;
  publishedAt: string | null;
  updatedAt: string;
}

/** The rich text for one category of change, e.g. everything that was fixed. */
export interface ReleaseNoteSection {
  category: ReleaseNoteCategory;
  contentHtml: string;
}

export interface ReleaseNote extends Omit<ReleaseNoteSummary, 'categories'> {
  applicationId: string;
  /** Introductory text shown before the sections; may be empty. */
  summaryHtml: string;
  sections: ReleaseNoteSection[];
  createdAt: string;
}

export interface SaveReleaseNoteRequest {
  version: string;
  title: string;
  summaryHtml: string;
  /** At most one per category. Sections left empty are dropped by the API. */
  sections: ReleaseNoteSection[];
  isPublished: boolean;
}
