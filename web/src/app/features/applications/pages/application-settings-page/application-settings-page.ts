import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ButtonDirective, ButtonIcon, ButtonLabel } from 'primeng/button';
import { Card } from 'primeng/card';
import { Observable, finalize } from 'rxjs';
import { SaveApplicationRequest } from '../../../../core/models/application.model';
import { NotificationService } from '../../../../core/notifications/notification.service';
import { ApplicationsStore } from '../../../../core/state/applications-store';
import { ConfirmService } from '../../../../shared/services/confirm.service';
import { ApplicationForm } from '../../components/application-form/application-form';

@Component({
  selector: 'app-application-settings-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ButtonDirective, ButtonIcon, ButtonLabel, Card, ApplicationForm],
  templateUrl: './application-settings-page.html',
  styleUrl: './application-settings-page.scss',
})
export class ApplicationSettingsPage {
  private readonly store = inject(ApplicationsStore);
  private readonly router = inject(Router);
  private readonly notifications = inject(NotificationService);
  private readonly confirmation = inject(ConfirmService);

  readonly applicationId = input.required<string>();

  protected readonly application = computed(() => this.store.byId(this.applicationId()) ?? null);
  protected readonly busy = signal(false);

  protected save(request: SaveApplicationRequest): void {
    this.run(this.store.update(this.applicationId(), request), () =>
      this.notifications.success('Application saved.'),
    );
  }

  protected async regenerateKey(): Promise<void> {
    const confirmed = await this.confirmation.confirm({
      header: 'Regenerate API key',
      message: 'Apps using the current key will stop working immediately.',
      acceptLabel: 'Regenerate',
      danger: true,
    });
    if (confirmed) {
      this.run(this.store.regenerateApiKey(this.applicationId()), () =>
        this.notifications.success('New API key issued.'),
      );
    }
  }

  protected async delete(): Promise<void> {
    const name = this.application()?.name ?? 'this application';
    const confirmed = await this.confirmation.confirm({
      header: 'Delete application',
      message: `Delete ${name} and all of its release notes? This cannot be undone.`,
      acceptLabel: 'Delete',
      danger: true,
    });
    if (confirmed) {
      this.run(this.store.delete(this.applicationId()), () => {
        this.notifications.success(`${name} deleted.`);
        this.router.navigate(['/']);
      });
    }
  }

  private run<T>(operation: Observable<T>, onSuccess: () => void): void {
    this.busy.set(true);
    operation.pipe(finalize(() => this.busy.set(false))).subscribe(onSuccess);
  }
}
