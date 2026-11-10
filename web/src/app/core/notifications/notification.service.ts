import { Injectable, inject } from '@angular/core';
import { MessageService } from 'primeng/api';

const LIFE_MS = 5000;

/** Transient toast messages, rendered by the PrimeNG toast in the shell. */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly messages = inject(MessageService);

  success(message: string): void {
    this.messages.add({ severity: 'success', summary: 'Done', detail: message, life: LIFE_MS });
  }

  error(message: string): void {
    this.messages.add({ severity: 'error', summary: 'Error', detail: message, life: LIFE_MS });
  }
}
