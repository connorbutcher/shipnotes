import Quill from 'quill';
import { ReleaseNoteContent } from '../../../../core/models/release-note.model';
import { registerCategoryFormat } from './category-format';
import { readNoteContent, toNoteDelta } from './note-content';

describe('note content in the editor', () => {
  function setup(content?: ReleaseNoteContent) {
    registerCategoryFormat();
    const host = document.createElement('div');
    document.body.appendChild(host);
    const quill = new Quill(host);
    if (content) {
      quill.setContents(toNoteDelta(quill, content), 'silent');
    }
    return quill;
  }

  const note: ReleaseNoteContent = {
    summaryHtml: '<p>Big release</p>',
    sections: [
      { category: 'fixed', contentHtml: '<p>Crash on start</p>' },
      {
        category: 'added',
        contentHtml: '<h2>Calendar</h2><ul><li>Week view</li><li>Month view</li></ul>',
      },
    ],
  };

  it('loads the summary, then each section in display order, marked with its category', () => {
    const quill = setup(note);

    const lines = [...quill.root.children].map((el) => `${el.tagName} ${el.className}`.trim());
    expect(lines).toEqual(['P', 'H2 note-section--added', 'OL', 'P note-section--fixed']);
    expect([...quill.root.querySelectorAll('li')].map((li) => li.className)).toEqual([
      'note-section--added',
      'note-section--added',
    ]);
  });

  it('reads back the same summary and sections, without the editor classes', () => {
    const quill = setup(note);

    expect(readNoteContent(quill)).toEqual({
      summaryHtml: '<p>Big&nbsp;release</p>',
      sections: [
        {
          category: 'added',
          contentHtml: '<h2>Calendar</h2><ul><li>Week&nbsp;view</li><li>Month&nbsp;view</li></ul>',
        },
        { category: 'fixed', contentHtml: '<p>Crash&nbsp;on&nbsp;start</p>' },
      ],
    });
  });

  it('puts lines in a section with the category format, and joins repeated categories', () => {
    const quill = setup();
    quill.setText('Intro\nOne\nMiddle\nTwo\n');
    quill.formatLine(6, 1, 'category', 'fixed');
    quill.formatLine(17, 1, 'category', 'fixed');

    expect(readNoteContent(quill)).toEqual({
      summaryHtml: '<p>Intro</p><p>Middle</p>',
      sections: [{ category: 'fixed', contentHtml: '<p>One</p><p>Two</p>' }],
    });
  });

  it('ignores categories it does not know', () => {
    const quill = setup();
    quill.setText('Text\n');
    quill.formatLine(0, 1, 'category', 'nonsense');

    expect(readNoteContent(quill).sections).toEqual([]);
  });

  it('keeps a pasted section class, so copying between notes keeps the section', () => {
    const quill = setup();
    quill.clipboard.dangerouslyPasteHTML('<p class="note-section--security">Patched</p>');

    expect(readNoteContent(quill).sections).toEqual([
      { category: 'security', contentHtml: '<p>Patched</p>' },
    ]);
  });
});
