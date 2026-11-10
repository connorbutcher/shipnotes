/**
 * Quill's semantic HTML output encodes every space as a non-breaking space,
 * which stops text wrapping in consuming apps. Convert them back to normal spaces
 * and treat an empty editor (null, or only an empty paragraph) as empty.
 */
export function normalizeEditorHtml(html: string | null): string {
  if (!html) {
    return '';
  }
  const normalized = html.replace(/&nbsp;|\u00a0/g, ' ');
  return /^<p>(<br>)?\s*<\/p>$/.test(normalized.trim()) ? '' : normalized;
}
