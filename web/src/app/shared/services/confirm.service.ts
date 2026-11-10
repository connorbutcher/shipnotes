import { Injectable, inject } from '@angular/core';
import { ConfirmationService } from 'primeng/api';

export interface ConfirmOptions {
  header: string;
  message: string;
  acceptLabel: string;
  /** Styles the accept button as destructive. */
  danger?: boolean;
}

/** Asks the user to confirm actions with the PrimeNG confirm dialog in the shell. */
@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private readonly confirmation = inject(ConfirmationService);

  confirm({ header, message, acceptLabel, danger = false }: ConfirmOptions): Promise<boolean> {
    return new Promise((resolve) => {
      this.confirmation.confirm({
        header,
        message,
        icon: 'pi pi-exclamation-triangle',
        acceptButtonProps: { label: acceptLabel, severity: danger ? 'danger' : 'primary' },
        rejectButtonProps: { label: 'Cancel', severity: 'secondary', outlined: true },
        accept: () => resolve(true),
        reject: () => resolve(false),
      });
    });
  }
}
