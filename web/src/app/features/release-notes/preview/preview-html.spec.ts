import { toPreviewHtml } from './preview-html';

describe('toPreviewHtml', () => {
  const mediaUrl = '/api/v1/media/01933b5e-0000-7000-8000-000000000001/shot.png';

  it('normalises editor HTML the same way a save does', () => {
    expect(toPreviewHtml('<p>Faster&nbsp;search</p>')).toBe('<p>Faster search</p>');
    expect(toPreviewHtml('<p><br></p>')).toBe('');
  });

  it('keeps uploaded media images', () => {
    const html = `<p>Look</p><p><img src="${mediaUrl}" alt="Screenshot"></p>`;
    expect(toPreviewHtml(html)).toBe(html);
  });

  it('drops inline and external images, as the API does', () => {
    const html =
      '<p>Look<img src="data:image/png;base64,iVBORw0KGgo="><img src="https://example.com/a.png"></p>';
    expect(toPreviewHtml(html)).toBe('<p>Look</p>');
  });
});
