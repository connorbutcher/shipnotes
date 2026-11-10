import { TestBed } from '@angular/core/testing';
import { SaveApplicationRequest } from '../../../../core/models/application.model';
import { ApplicationForm } from './application-form';

describe('ApplicationForm', () => {
  function setup() {
    const fixture = TestBed.createComponent(ApplicationForm);
    const saved: SaveApplicationRequest[] = [];
    fixture.componentInstance.saved.subscribe((value) => saved.push(value));
    fixture.detectChanges();
    const element = fixture.nativeElement as HTMLElement;
    const type = (selector: string, value: string) => {
      const input = element.querySelector<HTMLInputElement>(selector)!;
      input.value = value;
      input.dispatchEvent(new Event('input'));
    };
    const submit = () => {
      element.querySelector('form')!.dispatchEvent(new Event('submit'));
      fixture.detectChanges();
    };
    return { element, saved, type, submit };
  }

  it('emits trimmed values and nulls an empty icon URL', () => {
    const { saved, type, submit } = setup();
    type('input[formControlName=name]', '  Portal  ');
    submit();
    expect(saved).toEqual([{ name: 'Portal', iconUrl: null }]);
  });

  it('does not emit when the name is missing', () => {
    const { element, saved, submit } = setup();
    submit();
    expect(saved).toEqual([]);
    expect(element.textContent).toContain('Name is required.');
  });

  it('rejects an icon URL that is not http or https', () => {
    const { saved, type, submit } = setup();
    type('input[formControlName=name]', 'Portal');
    type('input[formControlName=iconUrl]', 'ftp://x');
    submit();
    expect(saved).toEqual([]);
  });
});
