import Quill from 'quill';
import { registerCategoryFormat } from './category-format';
import { readNoteHtml, toNoteDelta } from './note-content';

describe('note HTML in the editor', () => {
  function setup(html?: string) {
    registerCategoryFormat();
    const host = document.createElement('div');
    document.body.appendChild(host);
    const quill = new Quill(host);
    if (html !== undefined) {
      quill.setContents(toNoteDelta(quill, html), 'silent');
    }
    return quill;
  }

  const section = (category: string, label: string, html: string) =>
    `<section class="note-section note-section--${category}">` +
    `<h2 class="note-section__title">${label}</h2>${html}</section>`;

  it('loads each section as lines in its category, without its heading', () => {
    const quill = setup(
      '<p>Intro</p>' +
        section('fixed', 'Fixed', '<p>Crash on start</p>') +
        '<p>Outro</p>' +
        section('added', 'Added', '<ul><li>Week view</li></ul>'),
    );

    const lines = [...quill.root.children].map((el) => `${el.tagName} ${el.className}`.trim());
    expect(lines).toEqual(['P', 'P note-section--fixed', 'P', 'OL']);
    expect(quill.root.querySelector('li')?.className).toBe('note-section--added');
    expect(quill.root.textContent).not.toContain('Fixed');
  });

  it('writes the document back as one HTML document, in the order it was written', () => {
    const quill = setup(
      '<p>Intro</p>' +
        section('fixed', 'Fixed', '<p>Crash on start</p>') +
        '<p>Outro</p>' +
        section('fixed', 'Fixed', '<p>Second fix</p>'),
    );

    expect(readNoteHtml(quill)).toBe(
      '<p>Intro</p>' +
        section('fixed', 'Fixed', '<p>Crash on start</p>') +
        '<p>Outro</p>' +
        section('fixed', 'Fixed', '<p>Second fix</p>'),
    );
  });

  it('wraps lines put in a section from the toolbar', () => {
    const quill = setup();
    quill.setText('Intro\nOne\nTwo\n');
    quill.formatLine(6, 8, 'category', 'breaking');

    expect(readNoteHtml(quill)).toBe(
      '<p>Intro</p>' + section('breaking', 'Breaking changes', '<p>One</p><p>Two</p>'),
    );
  });

  it('ignores categories it does not know', () => {
    const quill = setup();
    quill.setText('Text\n');
    quill.formatLine(0, 1, 'category', 'nonsense');

    expect(readNoteHtml(quill)).toBe('<p>Text</p>');
  });

  it('treats sections it does not recognise as ordinary content', () => {
    const quill = setup('<section class="other"><p>Hi</p></section>');

    expect(readNoteHtml(quill)).toBe('<p>Hi</p>');
  });
});
