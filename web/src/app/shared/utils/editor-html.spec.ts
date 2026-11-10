import { normalizeEditorHtml } from './editor-html';

describe('normalizeEditorHtml', () => {
  it('replaces non-breaking spaces with spaces', () => {
    expect(normalizeEditorHtml('<p>Fixed&nbsp;login&nbsp;bug</p>')).toBe('<p>Fixed login bug</p>');
  });

  it('treats an empty paragraph as empty content', () => {
    expect(normalizeEditorHtml('<p><br></p>')).toBe('');
  });

  it('treats a null value from a cleared editor as empty content', () => {
    expect(normalizeEditorHtml(null)).toBe('');
  });
});
