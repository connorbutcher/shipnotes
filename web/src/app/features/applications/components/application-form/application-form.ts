import { ChangeDetectionStrategy, Component, effect, inject, input, output } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Application, SaveApplicationRequest } from '../../../../core/models/application.model';
import { AppIcon } from '../../../../shared/ui/app-icon/app-icon';

const NAME_MAX_LENGTH = 100;
const URL_PATTERN = /^https?:\/\/\S+$/i;

/** Create/edit form for an application's name and icon URL. */
@Component({
  selector: 'app-application-form',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, AppIcon],
  templateUrl: './application-form.html',
  styleUrl: './application-form.scss',
})
export class ApplicationForm {
  private readonly fb = inject(NonNullableFormBuilder);

  readonly application = input<Application | null>(null);
  readonly submitLabel = input('Save');
  readonly saving = input(false);
  readonly saved = output<SaveApplicationRequest>();

  protected readonly nameMaxLength = NAME_MAX_LENGTH;

  protected readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(NAME_MAX_LENGTH)]],
    iconUrl: ['', Validators.pattern(URL_PATTERN)],
  });

  protected readonly preview = toSignal(this.form.valueChanges, {
    initialValue: this.form.getRawValue(),
  });

  constructor() {
    effect(() => {
      const application = this.application();
      if (application) {
        this.form.reset({ name: application.name, iconUrl: application.iconUrl ?? '' });
      }
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, iconUrl } = this.form.getRawValue();
    this.saved.emit({ name: name.trim(), iconUrl: iconUrl.trim() || null });
  }
}
