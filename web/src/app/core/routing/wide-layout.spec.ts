import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { WIDE_LAYOUT, injectWideLayout } from './wide-layout';

describe('injectWideLayout', () => {
  it('follows the deepest route as the user navigates', async () => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter([
          { path: 'list', children: [] },
          { path: 'edit', children: [], data: { [WIDE_LAYOUT]: true } },
        ]),
      ],
    });
    const router = TestBed.inject(Router);
    const wide = TestBed.runInInjectionContext(() => injectWideLayout());

    await router.navigateByUrl('/edit');
    expect(wide()).toBe(true);

    await router.navigateByUrl('/list');
    expect(wide()).toBe(false);
  });
});
