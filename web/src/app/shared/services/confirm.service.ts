import { DOCUMENT, Injectable, inject } from '@angular/core';

/** Asks the user to confirm destructive actions. Wrapped so it can be replaced in tests. */
@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private readonly window = inject(DOCUMENT).defaultView;

  confirm(message: string): boolean {
    return this.window?.confirm(message) ?? false;
  }
}
