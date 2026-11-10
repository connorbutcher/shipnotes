import { Routes } from '@angular/router';

export const APPLICATION_ROUTES: Routes = [
  {
    path: 'new',
    title: 'New application',
    loadComponent: () =>
      import('./pages/new-application-page/new-application-page').then((m) => m.NewApplicationPage),
  },
  {
    path: ':applicationId',
    loadComponent: () =>
      import('./pages/application-page/application-page').then((m) => m.ApplicationPage),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'release-notes' },
      {
        path: 'release-notes',
        loadChildren: () =>
          import('../release-notes/release-notes.routes').then((m) => m.RELEASE_NOTES_ROUTES),
      },
      {
        path: 'integration',
        title: 'Integration',
        loadComponent: () =>
          import('./pages/integration-page/integration-page').then((m) => m.IntegrationPage),
      },
      {
        path: 'settings',
        title: 'Settings',
        loadComponent: () =>
          import('./pages/application-settings-page/application-settings-page').then(
            (m) => m.ApplicationSettingsPage,
          ),
      },
    ],
  },
];
