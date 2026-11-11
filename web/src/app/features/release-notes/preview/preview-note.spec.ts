import { hasPreviewContent, toPreviewNote } from './preview-note';

describe('toPreviewNote', () => {
  const now = new Date('2026-11-10T12:00:00Z');
  const draft = {
    version: ' 2.0.0-beta.1 ',
    title: ' Dark mode ',
    contentHtml: '<p>Hello&nbsp;world</p>',
    isPublished: false,
  };

  it('maps the draft to the public shape consumers receive', () => {
    expect(toPreviewNote(draft, null, now)).toEqual({
      version: '2.0.0-beta.1',
      isPreRelease: true,
      title: 'Dark mode',
      contentHtml: '<p>Hello world</p>',
      publishedAt: now.toISOString(),
    });
  });

  it('keeps the existing publish date when the note was already published', () => {
    expect(toPreviewNote(draft, '2026-01-02T00:00:00Z', now).publishedAt).toBe(
      '2026-01-02T00:00:00Z',
    );
  });

  it('reports an empty draft as having nothing to preview', () => {
    const empty = toPreviewNote({ ...draft, version: '', title: '', contentHtml: '' }, null, now);
    expect(hasPreviewContent(empty)).toBe(false);
    expect(hasPreviewContent(toPreviewNote(draft, null, now))).toBe(true);
  });
});
