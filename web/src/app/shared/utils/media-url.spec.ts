import { isMediaUrl } from './media-url';

describe('isMediaUrl', () => {
  const id = '01933b5e-0000-7000-8000-000000000001';

  it('accepts absolute and relative ShipNotes media URLs', () => {
    expect(isMediaUrl(`https://notes.example.com/api/v1/media/${id}/shot.png`)).toBe(true);
    expect(isMediaUrl(`/api/v1/media/${id}`)).toBe(true);
  });

  it('rejects inline and external images', () => {
    expect(isMediaUrl('data:image/png;base64,iVBORw0KGgo=')).toBe(false);
    expect(isMediaUrl('blob:http://localhost/123')).toBe(false);
    expect(isMediaUrl('https://example.com/image.png')).toBe(false);
    expect(isMediaUrl(null)).toBe(false);
  });
});
