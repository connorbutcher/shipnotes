export interface ReleaseNoteSummary {
  id: string;
  version: string;
  isPreRelease: boolean;
  title: string;
  isPublished: boolean;
  publishedAt: string | null;
  updatedAt: string;
}

export interface ReleaseNote extends ReleaseNoteSummary {
  applicationId: string;
  contentHtml: string;
  createdAt: string;
}

export interface SaveReleaseNoteRequest {
  version: string;
  title: string;
  contentHtml: string;
  isPublished: boolean;
}
