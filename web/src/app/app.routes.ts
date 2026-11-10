import { Routes } from '@angular/router';
import { Shell } from './layout/shell/shell';

export const routes: Routes = [
  {
    path: '',
    component: Shell,
    children: [
      {
        path: '',
        pathMatch: 'full',
        title: 'ShipNotes',
        loadComponent: () =>
          import('./features/applications/pages/welcome-page/welcome-page').then(
            (m) => m.WelcomePage,
          ),
      },
      {
        path: 'applications',
        loadChildren: () =>
          import('./features/applications/applications.routes').then((m) => m.APPLICATION_ROUTES),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
