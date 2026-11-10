import { Routes } from '@angular/router';
import { WIDE_LAYOUT } from '../../core/routing/wide-layout';

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
    data: { [WIDE_LAYOUT]: true },
    loadComponent: () =>
      import('./pages/release-note-editor-page/release-note-editor-page').then(
        (m) => m.ReleaseNoteEditorPage,
      ),
  },
  {
    path: ':releaseNoteId',
    title: 'Edit release notes',
    data: { [WIDE_LAYOUT]: true },
    loadComponent: () =>
      import('./pages/release-note-editor-page/release-note-editor-page').then(
        (m) => m.ReleaseNoteEditorPage,
      ),
  },
];
