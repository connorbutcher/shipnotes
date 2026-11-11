import { TestBed } from '@angular/core/testing';
import { SaveReleaseNoteRequest } from '../../../../core/models/release-note.model';
import { ReleaseNoteForm } from './release-note-form';

describe('ReleaseNoteForm', () => {
  function setup() {
    const fixture = TestBed.createComponent(ReleaseNoteForm);
    const saved: SaveReleaseNoteRequest[] = [];
    const drafts: SaveReleaseNoteRequest[] = [];
    fixture.componentInstance.saved.subscribe((value) => saved.push(value));
    fixture.componentInstance.draftChanged.subscribe((value) => drafts.push(value));
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    const type = (name: string, value: string) => {
      const input = element.querySelector<HTMLInputElement>(`input[formControlName=${name}]`)!;
      input.value = value;
      input.dispatchEvent(new Event('input'));
      fixture.detectChanges();
    };
    const submit = () => {
      element.querySelector('form')!.dispatchEvent(new Event('submit'));
      fixture.detectChanges();
    };
    return { fixture, element, saved, drafts, type, submit };
  }

  it('rejects versions that are not semver', () => {
    const { element, saved, type, submit } = setup();
    type('version', '1.2');
    type('title', 'Hello');
    submit();
    expect(saved).toEqual([]);
    expect(element.textContent).toContain('Use semantic versioning');
  });

  it('flags pre-release versions', () => {
    const { element, type } = setup();
    type('version', '2.0.0-beta.1');
    expect(element.textContent).toContain('Pre-release');
  });

  it('emits the trimmed request', () => {
    const { saved, type, submit } = setup();
    type('version', ' 1.2.0 ');
    type('title', ' Faster ');
    submit();
    expect(saved).toEqual([
      { version: '1.2.0', title: 'Faster', summaryHtml: '', sections: [], isPublished: false },
    ]);
  });

  it('loads an existing note into one editor, with a block per section', async () => {
    const fixture = TestBed.createComponent(ReleaseNoteForm);
    const saved: SaveReleaseNoteRequest[] = [];
    fixture.componentInstance.saved.subscribe((value) => saved.push(value));
    fixture.componentRef.setInput('releaseNote', {
      id: 'n1',
      applicationId: 'a1',
      version: '2.0.0',
      isPreRelease: false,
      title: 'Big',
      summaryHtml: '<p>Intro</p>',
      sections: [
        { category: 'breaking', contentHtml: '<p>B</p>' },
        { category: 'fixed', contentHtml: '<p>F</p>' },
      ],
      isPublished: true,
      publishedAt: null,
      createdAt: '',
      updatedAt: '',
    });
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    await vi.waitFor(() => expect(element.querySelector('.ql-editor > p')).not.toBeNull());

    expect(element.querySelectorAll('quill-editor').length).toBe(1);
    const lines = [...element.querySelectorAll('.ql-editor > p')];
    expect(lines.map((p) => [p.className, p.textContent])).toEqual([
      ['', 'Intro'],
      ['note-section--breaking', 'B'],
      ['note-section--fixed', 'F'],
    ]);

    element.querySelector('form')!.dispatchEvent(new Event('submit'));
    expect(saved[0]).toMatchObject({
      summaryHtml: '<p>Intro</p>',
      sections: [
        { category: 'breaking', contentHtml: '<p>B</p>' },
        { category: 'fixed', contentHtml: '<p>F</p>' },
      ],
    });
  });

  it('offers every category in the toolbar section picker', async () => {
    const { fixture, element } = setup();
    await fixture.whenStable();

    const options = [...element.querySelectorAll('select.ql-category option')];
    expect(options.map((o) => o.textContent?.trim())).toEqual([
      'Summary',
      'Breaking changes',
      'Added',
      'Changed',
      'Deprecated',
      'Removed',
      'Fixed',
      'Security',
    ]);
  });

  it('emits the draft as the user types, for the live preview', () => {
    const { drafts, type } = setup();
    type('version', '1.3.0');
    type('title', ' Dark mode ');
    expect(drafts.at(-1)).toEqual({
      version: '1.3.0',
      title: 'Dark mode',
      summaryHtml: '',
      sections: [],
      isPublished: false,
    });
  });
});
