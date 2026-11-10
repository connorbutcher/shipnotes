/** An uploaded media file. `url` is what goes into release note HTML. */
export interface Media {
  id: string;
  url: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
}
