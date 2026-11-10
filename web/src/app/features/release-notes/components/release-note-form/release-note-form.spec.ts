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
    return { element, saved, drafts, type, submit };
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
      { version: '1.2.0', title: 'Faster', contentHtml: '', isPublished: false },
    ]);
  });

  it('emits the draft as the user types, for the live preview', () => {
    const { drafts, type } = setup();
    type('version', '1.3.0');
    type('title', ' Dark mode ');
    expect(drafts.at(-1)).toEqual({
      version: '1.3.0',
      title: 'Dark mode',
      contentHtml: '',
      isPublished: false,
    });
  });
});
