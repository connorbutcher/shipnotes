/** A release note as consuming applications receive it from the public API. */
export interface PublicReleaseNote {
  version: string;
  isPreRelease: boolean;
  title: string;
  contentHtml: string;
  publishedAt: string;
}

/** Application details from the public API. */
export interface PublicApplication {
  name: string;
  iconUrl: string | null;
}
