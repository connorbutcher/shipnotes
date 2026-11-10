import { normalizeEditorHtml } from '../../../shared/utils/editor-html';
import { isMediaUrl } from '../../../shared/utils/media-url';

/**
 * Editor HTML as the API will store it: normalised like a save, with images that are not
 * uploaded ShipNotes media removed (the API drops those too). Angular's own sanitiser strips
 * anything unsafe when the result is bound to innerHTML.
 */
export function toPreviewHtml(html: string): string {
  const normalized = normalizeEditorHtml(html);
  if (!/<img/i.test(normalized)) {
    return normalized;
  }

  const body = new DOMParser().parseFromString(normalized, 'text/html').body;
  body.querySelectorAll('img').forEach((image) => {
    if (!isMediaUrl(image.getAttribute('src'))) {
      image.remove();
    }
  });
  return body.innerHTML;
}
