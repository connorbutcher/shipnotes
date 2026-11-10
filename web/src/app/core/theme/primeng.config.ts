import { EnvironmentProviders } from '@angular/core';
import { providePrimeNG } from 'primeng/config';
import { environment } from '../../../environments/environment';
import { ShipNotesPreset } from './shipnotes-preset';

/** PrimeNG theme and global options. Dark mode is opt-in via the `app-dark` class. */
export function provideShipNotesPrimeNG(): EnvironmentProviders {
  return providePrimeNG({
    ripple: true,
    license: environment.primeNgLicenseKey || undefined,
    theme: {
      preset: ShipNotesPreset,
      options: { darkModeSelector: '.app-dark', cssLayer: false },
    },
  });
}
