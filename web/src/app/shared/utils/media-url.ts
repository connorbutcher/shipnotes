const MEDIA_URL =
  /^(?:https?:\/\/[^/?#]+)?\/api\/v1\/media\/[0-9a-f]{8}-(?:[0-9a-f]{4}-){3}[0-9a-f]{12}(?:\/[^/?#]*)?$/i;

/** Whether a URL points at media uploaded to ShipNotes (the only images release notes may contain). */
export function isMediaUrl(url: string | null | undefined): boolean {
  return !!url && MEDIA_URL.test(url.trim());
}
