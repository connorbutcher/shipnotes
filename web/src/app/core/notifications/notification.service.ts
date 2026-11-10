import { Injectable, signal } from '@angular/core';

export type NotificationKind = 'success' | 'error';

export interface Notification {
  id: number;
  kind: NotificationKind;
  message: string;
}

const AUTO_DISMISS_MS = 5000;

/** Transient toast messages shown by the toast outlet. */
@Injectable({ providedIn: 'root' })
export class NotificationService {
  private nextId = 0;
  private readonly items = signal<Notification[]>([]);

  readonly notifications = this.items.asReadonly();

  success(message: string): void {
    this.push('success', message);
  }

  error(message: string): void {
    this.push('error', message);
  }

  dismiss(id: number): void {
    this.items.update((items) => items.filter((n) => n.id !== id));
  }

  private push(kind: NotificationKind, message: string): void {
    const id = this.nextId++;
    this.items.update((items) => [...items, { id, kind, message }]);
    setTimeout(() => this.dismiss(id), AUTO_DISMISS_MS);
  }
}
