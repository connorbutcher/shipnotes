import { FormArray } from '@angular/forms';
import {
  SectionForm,
  addSection,
  setSections,
  toSectionRequests,
  unusedCategories,
} from './section-form';

describe('section form', () => {
  const categories = (sections: FormArray<SectionForm>) =>
    sections.controls.map((s) => s.controls.category.value);

  it('adds sections in display order', () => {
    const sections = new FormArray<SectionForm>([]);
    addSection(sections, 'fixed');
    addSection(sections, 'breaking');
    const index = addSection(sections, 'added');

    expect(categories(sections)).toEqual(['breaking', 'added', 'fixed']);
    expect(index).toBe(1);
  });

  it('replaces sections when a note is loaded', () => {
    const sections = new FormArray<SectionForm>([]);
    addSection(sections, 'fixed');
    setSections(sections, [{ category: 'security', contentHtml: '<p>S</p>' }]);

    expect(sections.getRawValue()).toEqual([{ category: 'security', contentHtml: '<p>S</p>' }]);
  });

  it('lists the categories still available', () => {
    expect(unusedCategories(['added', 'fixed', 'security'])).toEqual([
      'breaking',
      'changed',
      'deprecated',
      'removed',
    ]);
  });

  it('drops empty sections and normalises spaces', () => {
    expect(
      toSectionRequests([
        { category: 'added', contentHtml: '<p>New&nbsp;thing</p>' },
        { category: 'fixed', contentHtml: '<p><br></p>' },
      ]),
    ).toEqual([{ category: 'added', contentHtml: '<p>New thing</p>' }]);
  });
});
