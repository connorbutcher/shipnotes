import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Card } from 'primeng/card';
import { finalize } from 'rxjs';
import { SaveApplicationRequest } from '../../../../core/models/application.model';
import { ApplicationsStore } from '../../../../core/state/applications-store';
import { ApplicationForm } from '../../components/application-form/application-form';

@Component({
  selector: 'app-new-application-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [Card, ApplicationForm],
  templateUrl: './new-application-page.html',
  styleUrl: './new-application-page.scss',
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
