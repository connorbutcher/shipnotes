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

  it('loads the sections of an existing note', () => {
    const fixture = TestBed.createComponent(ReleaseNoteForm);
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

    const tags = [...element.querySelectorAll('app-release-note-section app-category-tag')];
    expect(tags.map((t) => t.textContent?.trim())).toEqual(['Breaking changes', 'Fixed']);
    expect(element.querySelector('app-category-picker')?.textContent).not.toContain('Fixed');
  });

  it('adds a section from the category picker', () => {
    const { fixture, element } = setup();
    const addSecurity = [
      ...element.querySelectorAll<HTMLButtonElement>('app-category-picker button'),
    ].find((b) => b.textContent?.includes('Security'))!;

    addSecurity.click();
    fixture.detectChanges();

    expect(element.querySelectorAll('app-release-note-section').length).toBe(1);
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
