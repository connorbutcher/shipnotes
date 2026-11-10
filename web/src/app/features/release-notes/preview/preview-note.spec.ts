import { SaveReleaseNoteRequest } from '../../../core/models/release-note.model';
import { hasPreviewContent, toPreviewNote } from './preview-note';

describe('toPreviewNote', () => {
  const now = new Date('2026-11-10T12:00:00Z');
  const draft: SaveReleaseNoteRequest = {
    version: ' 2.0.0-beta.1 ',
    title: ' Dark mode ',
    summaryHtml: '<p>Hello&nbsp;world</p>',
    sections: [
      { category: 'fixed', contentHtml: '<p>Crash on start</p>' },
      { category: 'added', contentHtml: '<p>Dark mode</p>' },
      { category: 'security', contentHtml: '<p><br></p>' },
    ],
    isPublished: false,
  };

  it('maps the draft to the public shape consumers receive', () => {
    expect(toPreviewNote(draft, null, now)).toEqual({
      version: '2.0.0-beta.1',
      isPreRelease: true,
      title: 'Dark mode',
      summaryHtml: '<p>Hello world</p>',
      sections: [
        { category: 'added', label: 'Added', contentHtml: '<p>Dark mode</p>' },
        { category: 'fixed', label: 'Fixed', contentHtml: '<p>Crash on start</p>' },
      ],
      contentHtml:
        '<p>Hello world</p>' +
        '<section data-category="added"><h2>Added</h2><p>Dark mode</p></section>' +
        '<section data-category="fixed"><h2>Fixed</h2><p>Crash on start</p></section>',
      publishedAt: now.toISOString(),
    });
  });

  it('keeps the existing publish date when the note was already published', () => {
    expect(toPreviewNote(draft, '2026-01-02T00:00:00Z', now).publishedAt).toBe(
      '2026-01-02T00:00:00Z',
    );
  });

  it('reports an empty draft as having nothing to preview', () => {
    const empty = toPreviewNote(
      { ...draft, version: '', title: '', summaryHtml: '', sections: [] },
      null,
      now,
    );
    expect(hasPreviewContent(empty)).toBe(false);
    expect(hasPreviewContent(toPreviewNote(draft, null, now))).toBe(true);
  });
});
