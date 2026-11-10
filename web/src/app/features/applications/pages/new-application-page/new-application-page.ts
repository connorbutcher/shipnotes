import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { SaveApplicationRequest } from '../../../../core/models/application.model';
import { ApplicationsStore } from '../../../../core/state/applications-store';
import { ApplicationForm } from '../../components/application-form/application-form';

@Component({
  selector: 'app-new-application-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ApplicationForm],
  template: `
    <h1>New application</h1>
    <p class="muted">
      An API key is generated automatically. You'll see it once after creating the application.
    </p>
    <app-application-form
      submitLabel="Create application"
      [saving]="saving()"
      (saved)="create($event)"
    />
  `,
})
export class NewApplicationPage {
  private readonly store = inject(ApplicationsStore);
  private readonly router = inject(Router);

  protected readonly saving = signal(false);

  protected create(request: SaveApplicationRequest): void {
    this.saving.set(true);
    this.store
      .create(request)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe((application) => this.router.navigate(['/applications', application.id]));
  }
}
