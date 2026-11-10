import Quill from 'quill';
import { enableImageUpload } from './editor-image-upload';

describe('enableImageUpload', () => {
  const mediaUrl =
    'https://notes.example.com/api/v1/media/01933b5e-0000-7000-8000-000000000001/a.png';

  function setup() {
    const host = document.createElement('div');
    document.body.appendChild(host);
    const quill = new Quill(host);
    const uploaded: File[] = [];
    const disable = enableImageUpload(quill, () => async (file) => {
      uploaded.push(file);
      return mediaUrl;
    });
    return { quill, uploaded, disable };
  }

  it('drops inline and external images from pasted HTML', () => {
    const { quill } = setup();

    quill.clipboard.dangerouslyPasteHTML(
      '<p>Hi<img src="data:image/png;base64,iVBORw0KGgo="><img src="https://example.com/x.png"></p>',
    );

    expect(quill.root.querySelector('img')).toBeNull();
  });

  it('keeps pasted images that point at uploaded media', () => {
    const { quill } = setup();

    quill.clipboard.dangerouslyPasteHTML(`<p><img src="${mediaUrl}"></p>`);

    expect(quill.root.querySelector('img')?.getAttribute('src')).toBe(mediaUrl);
  });

  it('uploads pasted image files and inserts their URL instead of inlining them', async () => {
    const { quill, uploaded } = setup();
    const image = new File([new Uint8Array([0x89, 0x50])], 'shot.png', { type: 'image/png' });
    const text = new File(['hi'], 'notes.txt', { type: 'text/plain' });
    const event = new Event('paste', { bubbles: true, cancelable: true });
    Object.defineProperty(event, 'clipboardData', { value: { files: [image, text] } });

    quill.root.dispatchEvent(event);
    await vi.waitFor(() => expect(quill.root.querySelector('img')).not.toBeNull());

    expect(event.defaultPrevented).toBe(true);
    expect(uploaded).toEqual([image]);
    expect(quill.root.querySelector('img')?.getAttribute('src')).toBe(mediaUrl);
  });
});
