import { TestBed } from '@angular/core/testing';
import { CategoryTag } from './category-tag';

describe('CategoryTag', () => {
  it('shows the category label', () => {
    const fixture = TestBed.createComponent(CategoryTag);
    fixture.componentRef.setInput('category', 'breaking');
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Breaking changes');
  });
});
