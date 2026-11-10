import { TestBed } from '@angular/core/testing';
import { AppIcon } from './app-icon';

describe('AppIcon', () => {
  function render(name: string, iconUrl: string | null) {
    const fixture = TestBed.createComponent(AppIcon);
    fixture.componentRef.setInput('name', name);
    fixture.componentRef.setInput('iconUrl', iconUrl);
    fixture.detectChanges();
    return fixture.nativeElement as HTMLElement;
  }

  it('shows initials when there is no icon', () => {
    expect(render('Customer portal', null).textContent?.trim()).toBe('CP');
  });

  it('shows the icon image when a URL is set', () => {
    expect(
      render('Portal', 'https://example.com/i.png').querySelector('img')?.getAttribute('src'),
    ).toBe('https://example.com/i.png');
  });
});
