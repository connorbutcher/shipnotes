import { Routes } from '@angular/router';

export const SHOWCASE_ROUTES: Routes = [
  {
    path: '',
    title: 'Showcase',
    loadComponent: () => import('./pages/showcase-page/showcase-page').then((m) => m.ShowcasePage),
  },
];
