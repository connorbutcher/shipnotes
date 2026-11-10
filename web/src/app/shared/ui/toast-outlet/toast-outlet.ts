import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { NotificationService } from '../../../core/notifications/notification.service';

@Component({
  selector: 'app-toast-outlet',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @for (toast of notifications.notifications(); track toast.id) {
      <div class="toast" [class]="toast.kind" role="alert">
        <span>{{ toast.message }}</span>
        <button type="button" aria-label="Dismiss" (click)="notifications.dismiss(toast.id)">
          ×
        </button>
      </div>
    }
  `,
  styleUrl: './toast-outlet.scss',
})
export class ToastOutlet {
  protected readonly notifications = inject(NotificationService);
}
