import { Signal, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRouteSnapshot, NavigationEnd, Router } from '@angular/router';
import { filter, map } from 'rxjs';

/** Route data key: set to true on routes whose page needs the full content width. */
export const WIDE_LAYOUT = 'wideLayout';

/** Whether the deepest active route asks for a wide layout. Call in an injection context. */
export function injectWideLayout(): Signal<boolean> {
  const router = inject(Router);
  const isWide = () => isWideRoute(router.routerState.snapshot.root);
  return toSignal(
    router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      map(isWide),
    ),
    { initialValue: isWide() },
  );
}

function isWideRoute(route: ActivatedRouteSnapshot): boolean {
  let deepest = route;
  while (deepest.firstChild) {
    deepest = deepest.firstChild;
  }
  return deepest.data[WIDE_LAYOUT] === true;
}
