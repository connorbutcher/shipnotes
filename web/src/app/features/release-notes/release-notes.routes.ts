import { Routes } from '@angular/router';

export const RELEASE_NOTES_ROUTES: Routes = [
  {
    path: '',
    title: 'Release notes',
    loadComponent: () =>
      import('./pages/release-notes-page/release-notes-page').then((m) => m.ReleaseNotesPage),
  },
  {
    path: 'new',
    title: 'New release notes',
    loadComponent: () =>
      import('./pages/release-note-editor-page/release-note-editor-page').then(
        (m) => m.ReleaseNoteEditorPage,
      ),
  },
  {
    path: ':releaseNoteId',
    title: 'Edit release notes',
    loadComponent: () =>
      import('./pages/release-note-editor-page/release-note-editor-page').then(
        (m) => m.ReleaseNoteEditorPage,
      ),
  },
];
